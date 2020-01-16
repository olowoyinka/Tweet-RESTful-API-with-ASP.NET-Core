using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TweetWebApi.Models
{
    public class AuthenticationResult
    {
        public string Token { get; set; }

        public bool Success { get; set; }

        public string RefreshToken { get; set; }

        public IEnumerable<string> Error { get; set; }
    }
}
