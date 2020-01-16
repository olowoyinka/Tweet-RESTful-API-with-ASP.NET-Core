using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetWebApi.EndPoint.v1;
using TweetWebApi.Pagination;

namespace TweetWebApi.Services
{
    public class UriService : IUriService
    {
        private readonly string _baseUri;

        public UriService(string baseUri)
        {
            _baseUri = baseUri;
        }

        public Uri GetAllPostUri(PaginationQuery pagination = null)
        {
            var uri = new Uri(_baseUri);

            if(pagination == null)
            {
                return uri;
            }

            var modifiedUri = QueryHelpers.AddQueryString(_baseUri, "pageNumber", pagination.PageNumber.ToString());
            modifiedUri = QueryHelpers.AddQueryString(modifiedUri, "pageSize", pagination.PageSize.ToString());

            return new Uri(modifiedUri);
        }

        public Uri GetPostUri(string postId)
        {
            return new Uri(_baseUri + APIRoute.Post.Get.Replace("{postId}", postId));
        }
    }
}
