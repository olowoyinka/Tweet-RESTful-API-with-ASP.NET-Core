using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TweetWebApi.Data;
using TweetWebApi.Helpers;
using TweetWebApi.Models;

namespace TweetWebApi.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtSettings _jwtSettings;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly ApplicationDbContext _applicationDbContext;

        public IdentityService(UserManager<IdentityUser> userManager,
                                JwtSettings jwtSettings,
                                TokenValidationParameters tokenValidationParameters,
                                ApplicationDbContext applicationDbContext)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings;
            _tokenValidationParameters = tokenValidationParameters;
            _applicationDbContext = applicationDbContext;
        }


        public async Task<AuthenticationResult> RegisterAsync(string email, string password)
        {
            var existingUser = await _userManager.FindByEmailAsync(email);

            if (existingUser != null)
            {
                return new AuthenticationResult
                {
                    Error = new[] { "User with this Email has Already Exists" }
                };
            }

            var newUser = new IdentityUser
            {
                Email = email,
                UserName = email
            };

            var createdUser = await _userManager.CreateAsync(newUser, password);

            await _userManager.AddClaimAsync(newUser, new Claim("tags.view", "true"));

            if (!createdUser.Succeeded)
            {
                return new AuthenticationResult
                {
                    Error = createdUser.Errors.Select(x => x.Description)
                };
            }

            return await GenerateAuthenticationResult(newUser);

        }


        public async Task<AuthenticationResult> LoginAsync(string email, string password)
        {
            var existUser = await _userManager.FindByEmailAsync(email);

            if (existUser == null)
            {
                return new AuthenticationResult
                {
                    Error = new[] { "User doesn't Exist" }
                };
            }

            var checkPassword = await _userManager.CheckPasswordAsync(existUser, password);

            if (!checkPassword)
            {
                return new AuthenticationResult
                {
                    Error = new[] { "Invalid Password" }
                };
            }

            return await GenerateAuthenticationResult(existUser);
        }


        public async Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken)
        {
            var validatedToken = GetPrincipalFromToken(token);

            if (validatedToken == null)
            {
                return new AuthenticationResult
                {
                    Error = new[] { "Invalid Token" }
                };
            }

            var expiryDateUnix = long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

            var expiryDateTimeUtc = new DateTime(year: 1970, month: 1, day: 1, hour: 0, minute: 0, second: 0, DateTimeKind.Utc)
                                        .AddSeconds(expiryDateUnix);

            if (expiryDateTimeUtc > DateTime.UtcNow)
            {
                return new AuthenticationResult
                {
                    Error = new[] { "This token hasn't expired yet " }
                };
            }

            var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

            var storedRefreshToken = await _applicationDbContext.RefreshTokens.SingleOrDefaultAsync(predicate: x => x.Token == refreshToken);

            if (storedRefreshToken == null)
            {
                return new AuthenticationResult
                {
                    Error = new[] { "This refresh token does not exist" }
                };
            }

            if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
            {
                return new AuthenticationResult
                {
                    Error = new[] { "This refresh token has expired " }
                };
            }

            if (storedRefreshToken.Invalidated)
            {
                return new AuthenticationResult
                {
                    Error = new[] { "This refresh token has been invalidated" }
                };
            }

            if (storedRefreshToken.Used)
            {
                return new AuthenticationResult
                {
                    Error = new[] { "This refresh token has been used" }
                };
            }

            if (storedRefreshToken.JwtId != jti)
            {
                return new AuthenticationResult
                {
                    Error = new[] { "This refresh token does not match this JWT" }
                };
            }

            storedRefreshToken.Used = true;
            _applicationDbContext.RefreshTokens.Update(storedRefreshToken);
            await _applicationDbContext.SaveChangesAsync();

            var user = await _userManager.FindByIdAsync(validatedToken.Claims.Single(x => x.Type == "id").Value);

            return await GenerateAuthenticationResult(user);
        }


        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out var validatedToken);

                if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
                {
                    return null;
                }

                return principal;
            }
            catch
            {
                return null;
            }
        }


        private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtSecurityToken) &&
                    jwtSecurityToken.Header.Alg.Equals(value: SecurityAlgorithms.HmacSha256,
                        StringComparison.InvariantCultureIgnoreCase);
        }


        private async Task<AuthenticationResult> GenerateAuthenticationResult(IdentityUser newUser)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

            var claims = new List<Claim>
            {
                    new Claim(type: JwtRegisteredClaimNames.Sub, value: newUser.Email),
                    new Claim(type: JwtRegisteredClaimNames.Jti, value: Guid.NewGuid().ToString()),
                    new Claim(type: JwtRegisteredClaimNames.Email, value: newUser.Email),
                    new Claim(type: "id", value: newUser.Id)
            };

            var userClaims = await _userManager.GetClaimsAsync(newUser);
            claims.AddRange(userClaims);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(_jwtSettings.TokenLifetime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), algorithm: SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var refreshAddToken = new RefreshToken
            {
                JwtId = token.Id,
                UserId = newUser.Id,
                CreationDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6)
            };

            await _applicationDbContext.RefreshTokens.AddAsync(refreshAddToken);
            await _applicationDbContext.SaveChangesAsync();

            return new AuthenticationResult
            {
                Success = true,
                Token = tokenHandler.WriteToken(token),
                RefreshToken = refreshAddToken.Token
            };
        }
    }
}
