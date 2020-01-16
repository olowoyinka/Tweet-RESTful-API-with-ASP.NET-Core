namespace TweetWebApi.EndPoint.Request
{
    public class RefreshRequest
    {
        public string Token { get; set; }

        public string RefreshToken { get; set; }
    }
}
