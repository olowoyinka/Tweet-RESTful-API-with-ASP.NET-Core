using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TweetWebApi.Models;
using TweetWebApi.Pagination;
using TweetWebApi.Pagination.Filter;

namespace TweetWebApi.Services
{
    public interface IPostService
    {
        Task<List<Post>> GetPosts(GetFilterPost filter = null,PaginationFilter paginationFilter = null);

        Task<Post> GetPostById(Guid postId);

        Task<bool> CreatePost(Post post);

        Task<bool> UpdatePost(Post post);

        Task<bool> DeletePost(Guid postId);

        Task<bool> UserOwnsPostAsync(Guid postId, string getUserId);

        Task<List<Tag>> GetAllTagsAsync();

        Task<bool> CreateTagAsync(Tag tag);

        Task<Tag> GetTagByNameAsync(string tagName);

        Task<bool> DeleteTagAsync(string tagName);
    }
}
