using System.ComponentModel.DataAnnotations;


namespace TweetWebApi.EndPoint.Request
{
    public class UserRequest
    {
        public string Email { get; set; }


        public string Password { get; set; }
    }
}