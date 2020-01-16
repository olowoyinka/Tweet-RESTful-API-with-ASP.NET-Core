using System;
using System.Collections.Generic;



namespace TweetWebApi.EndPoint.Response
{
    public class PostResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string UserId { get; set; }

        public IEnumerable<TagResponse> Tags { get; set; }


    }
}
