using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TweetWebApi.EndPoint.Request;
using TweetWebApi.EndPoint.Response;
using TweetWebApi.EndPoint.v1;
using TweetWebApi.Extensions;
using TweetWebApi.Models;
using TweetWebApi.Services;

namespace TweetWebApi.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    public class TagsController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly IMapper _mapper;

        public TagsController(IPostService postService, IMapper mapper)
        {
            _postService = postService;
            _mapper = mapper;
        }


        [HttpGet(APIRoute.Tag.GetAll)]
        [Authorize(Policy = "CompanyRequirement")]
        public async Task<IActionResult> GetAll()
        {
            var tags = await _postService.GetAllTagsAsync();

            return Ok(_mapper.Map<List<TagResponse>>(tags));
        }


        [HttpGet(APIRoute.Tag.Get)]
        public async Task<IActionResult> Get([FromRoute]string tagName)
        {
            var tag = await _postService.GetTagByNameAsync(tagName);

            if(tag == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<TagResponse>(tag));
        }


        [HttpPost(APIRoute.Tag.Create)]
        public async Task<IActionResult> Create([FromBody] PostRequest  request)
        {
            var newTag = new Tag
            {
                Name = request.Name,
                CreatedId = HttpContext.GetUserId(),
                CreatedOn = DateTime.UtcNow
            };

            var created = await _postService.CreateTagAsync(newTag);
            if (!created)
            {
                return BadRequest(new  { Message = "Unable to create tag" });
            }

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var locationUri = baseUrl + "/" + APIRoute.Tag.Get.Replace("{tagName}", newTag.Name);

            return Created(locationUri, _mapper.Map<TagResponse>(newTag));
        }


        [HttpDelete(APIRoute.Tag.Delete)]
        public async Task<IActionResult> Delete([FromRoute] string tagName)
        {
            var deleted = await _postService.DeleteTagAsync(tagName);

            if (deleted)
                return NoContent();

            return NotFound();
        }

    }
}