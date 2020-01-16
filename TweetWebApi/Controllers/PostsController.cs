using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetWebApi.ApiKey;
using TweetWebApi.Cache;
using TweetWebApi.EndPoint.Request;
using TweetWebApi.EndPoint.Response;
using TweetWebApi.EndPoint.v1;
using TweetWebApi.Extensions;
using TweetWebApi.Models;
using TweetWebApi.Pagination;
using TweetWebApi.Pagination.Filter;
using TweetWebApi.Services;

namespace TweetWebApi.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    //[ApiKeyAuth]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly IMapper _mapper;
        private readonly IUriService _uriService;

        public PostsController(IPostService productService, IMapper mapper, IUriService uriService)
        {
            _postService = productService;
            _mapper = mapper;
            _uriService = uriService;
        }


        [HttpGet(APIRoute.Post.GetAll)]
        [Cached(600)]
        public async Task<IActionResult> GetAll([FromQuery] GetFilterPost getFilterPost ,[FromQuery]PaginationQuery paginationQuery)
        {
            var pagination = _mapper.Map<PaginationFilter>(paginationQuery);

            var filter = _mapper.Map<GetFilterPost>(getFilterPost);

            var posts = await _postService.GetPosts( filter, pagination);

            var postsResponse = _mapper.Map<List<PostResponse>>(posts);

            if(pagination == null || pagination.PageNumber < 1 || pagination.PageSize < 1)
            {
                return Ok(new PaginationResponse<PostResponse>(postsResponse));
            }

            var paginationResponse = PaginationReponseHelper.CreatePaginatedResponse(_uriService, pagination, postsResponse);
            
            return Ok(paginationResponse);
        }


        [HttpGet(APIRoute.Post.Get)]
        [Cached(600)]
        public async Task<IActionResult> Get([FromRoute]Guid postId)
        {
            var post = await _postService.GetPostById(postId);

            if (post == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<PostResponse>(post));
        }


        [HttpPost(APIRoute.Post.Create)]
        public async Task<IActionResult> Create([FromBody] PostRequest postRequest)
        {
            var post = new Post
            {
                Id = Guid.NewGuid(),
                Name = postRequest.Name,
                UserId = HttpContext.GetUserId(),
                Tags = postRequest.Tags.Select(x => new PostTag { PostId = Guid.NewGuid(), TagName = x }).ToList()
            };

            await _postService.CreatePost(post);

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var locationUri = baseUrl + "/" + APIRoute.Post.Get.Replace("{postId}", post.Id.ToString());

            return Created(locationUri, _mapper.Map<PostResponse>(post));
        }


        [HttpPut(APIRoute.Post.Get)]
        public async Task<IActionResult> Update([FromRoute]Guid postId, [FromBody] PostRequest postRequest)
        {
            var userOwnPost = await _postService.UserOwnsPostAsync(postId, HttpContext.GetUserId());

            if (!userOwnPost)
            {
                return BadRequest(error: new
                {
                    error = "You do not own this post"
                });
            }

            var post = await _postService.GetPostById(postId);

            post.Name = postRequest.Name;

            var updated = await _postService.UpdatePost(post);

            if (updated)
                return Ok(_mapper.Map<PostResponse>(post));

            return NotFound();
        }


        [HttpDelete(APIRoute.Post.Get)]
        public async Task<IActionResult> Delete([FromRoute]Guid postId)
        {
            var userOwnPost = await _postService.UserOwnsPostAsync(postId, HttpContext.GetUserId());

            if (!userOwnPost)
            {
                return BadRequest(error: new
                {
                    error = "You do not own this post"
                });
            }

            var post = await _postService.DeletePost(postId);

            if (post)
            {
                return NoContent();
            }

            return NotFound();
        }

    }
}