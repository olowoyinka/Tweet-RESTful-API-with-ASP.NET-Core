using System.Collections.Generic;

namespace TweetWebApi.EndPoint.Response
{
    public class AuthResponse
    {
        public IEnumerable<string> Error { get; set; }

        public string Token { get; set; }

        public string RefreshToken { get; set; }
    }
}
