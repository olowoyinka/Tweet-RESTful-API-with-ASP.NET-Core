using AutoMapper;
using System.Linq;
using TweetWebApi.EndPoint.Response;
using TweetWebApi.Models;
using TweetWebApi.Pagination;
using TweetWebApi.Pagination.Filter;

namespace TweetWebApi.Mapping
{
    public class ModelResponseMapping : Profile
    {
        public ModelResponseMapping()
        {
            CreateMap<Post, PostResponse>()
                .ForMember(dest => dest.Tags, opt =>
                    opt.MapFrom(src => src.Tags.Select(x => new TagResponse { Name = x.TagName })));

            CreateMap<Tag, TagResponse>();

            CreateMap<PaginationQuery, PaginationFilter>();

            CreateMap<FilterPostQuery, GetFilterPost>();
        
        }
    }
}