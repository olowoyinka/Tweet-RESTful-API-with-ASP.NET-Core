using Refit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TweetWebApi.EndPoint.Request;
using TweetWebApi.EndPoint.Response;

namespace TweetWebApi.SDK
{
    [Headers("Authorization: Bearer")]
    public interface IPostAPI
    {
        [Get("/v1/Posts")]
        Task<ApiResponse<List<PostResponse>>> GetAll();

        [Get("/v1/Posts/{postId}")]
        Task<ApiResponse<PostResponse>> Get(Guid postId);

        [Post("/v1/Posts")]
        Task<ApiResponse<PostResponse>> Create([Body] PostRequest postRequest);

        [Put("/v1/Posts/{postId}")]
        Task<ApiResponse<PostResponse>> Update(Guid postId, [Body] PostRequest postRequest);

        [Delete("/v1/Posts/{postId}")]
        Task<ApiResponse<string>> Delete(Guid postId);
    }
}
