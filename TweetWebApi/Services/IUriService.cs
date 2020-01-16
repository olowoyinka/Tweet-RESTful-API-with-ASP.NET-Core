using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetWebApi.Pagination;

namespace TweetWebApi.Services
{
    public interface IUriService
    {
        Uri GetPostUri(string postId);

        Uri GetAllPostUri(PaginationQuery pagination = null);
    }
}
