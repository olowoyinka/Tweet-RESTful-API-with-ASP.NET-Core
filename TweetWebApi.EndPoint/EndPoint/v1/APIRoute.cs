namespace TweetWebApi.EndPoint.v1
{
    public static class APIRoute
    {
        public const string version = "v1";

        public static class Post
        {
            public const string GetAll = version + "/posts";

            public const string Create = version + "/posts";

            public const string Get = version + "/posts/{postId}";
        }

        public static class Tag
        {
            public const string GetAll = version + "/tags";

            public const string Get = version + "/tags/{tagName}";

            public const string Create = version + "/tags";

            public const string Delete = version + "/tags/{tagName}";
        }

        public static class Account
        {
            public const string Register = version + "/register";

            public const string Login = version + "/login";

            public const string Refresh = version + "/refresh";
        }
    }
}
