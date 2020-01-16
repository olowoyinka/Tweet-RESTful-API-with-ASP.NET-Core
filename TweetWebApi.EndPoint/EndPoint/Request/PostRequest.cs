using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;



namespace TweetWebApi.EndPoint.Request
{
    public class PostRequest
    {
        [Required]
        public string Name { get; set; }

        public IEnumerable<string> Tags { get; set; }
    }
}
