using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TweetWebApi.EndPoint.Request;
using TweetWebApi.EndPoint.Response;
using TweetWebApi.EndPoint.v1;
using TweetWebApi.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TweetWebApi.Controllers
{
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IIdentityService _identityService;


        public AccountController(IIdentityService identityService)
        {
            _identityService = identityService;
        }


        [HttpPost(APIRoute.Account.Register)]
        public async Task<IActionResult> Register([FromBody] UserRequest userRequest)
        {
            var authResponse = await _identityService.RegisterAsync(userRequest.Email, userRequest.Password);

            if (!authResponse.Success)
            {
                return BadRequest(new AuthResponse
                {
                    Error = authResponse.Error
                });

            }

            return Ok(new AuthResponse
            {
                Token = authResponse.Token
            });
        }


        [HttpPost(APIRoute.Account.Login)]
        public async Task<IActionResult> Login([FromBody] UserRequest userRequest)
        {
            var authUserResponse = await _identityService.LoginAsync(userRequest.Email, userRequest.Password);

            if (!authUserResponse.Success)
            {
                return BadRequest(new AuthResponse
                {
                    Error = authUserResponse.Error
                });

            }

            return Ok(new AuthResponse
            {
                Token = authUserResponse.Token,
                RefreshToken = authUserResponse.RefreshToken
            });
        }


        [HttpPost(APIRoute.Account.Refresh)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshRequest refreshRequest)
        {
            var authUserResponse = await _identityService.RefreshTokenAsync(refreshRequest.Token, refreshRequest.RefreshToken);

            if (!authUserResponse.Success)
            {
                return BadRequest(new AuthResponse
                {
                    Error = authUserResponse.Error
                });

            }

            return Ok(new AuthResponse
            {
                Token = authUserResponse.Token,
                RefreshToken = authUserResponse.RefreshToken
            });
        }
    }
}
