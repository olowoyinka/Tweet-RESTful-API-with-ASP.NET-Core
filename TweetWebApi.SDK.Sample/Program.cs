using Refit;
using System;
using System.Threading.Tasks;
using TweetWebApi.EndPoint.Request;

namespace TweetWebApi.SDK.Sample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var cachedToken = string.Empty;

            var identityApi = RestService.For<IIdentityAPI>("https://localhost:5001");


            var registerResponse = await identityApi.Register(new UserRequest
            {
                Email = "sdkaccount@gmail.com",
                Password = "Test1234!"
            });

            var loginResponse = await identityApi.Login(new UserRequest
            {
                Email = "sdkaccount@gmail.com",
                Password = "Test1234!"
            });


            cachedToken = loginResponse.Content.Token;

            var tweetbookApi = RestService.For<IPostAPI>("https://localhost:5001", new RefitSettings
            {
                AuthorizationHeaderValueGetter = () => Task.FromResult(cachedToken)
            });

            var allPosts = await tweetbookApi.GetAll();

            var createdPost = await tweetbookApi.Create(new PostRequest
            {
                Name = "This is created by the SDK",
                Tags = new[] { "sdk" }
            });

            var retrievedPost = await tweetbookApi.Get(createdPost.Content.Id);

            var updatedPost = await tweetbookApi.Update(createdPost.Content.Id, new PostRequest
            {
                Name = "This is updated by the SDK"
            });

            var deletePost = await tweetbookApi.Delete(createdPost.Content.Id);
        }
    }
}
