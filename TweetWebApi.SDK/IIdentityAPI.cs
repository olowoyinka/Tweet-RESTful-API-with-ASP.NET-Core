using Refit;
using System.Threading.Tasks;
using TweetWebApi.EndPoint.Request;
using TweetWebApi.EndPoint.Response;

namespace TweetWebApi.SDK
{
    public interface IIdentityAPI
    {
        [Post("/v1/register")]
        Task<ApiResponse<AuthResponse>> Register([Body] UserRequest userRequest);

        [Post("/v1/login")]
        Task<ApiResponse<AuthResponse>> Login([Body] UserRequest userRequest);

        [Post("/v1/refreshToken")]
        Task<ApiResponse<AuthResponse>> RefreshToken([Body] RefreshRequest refreshRequest);
    }
}
