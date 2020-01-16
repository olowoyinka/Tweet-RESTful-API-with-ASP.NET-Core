using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;



namespace TweetWebApi.Authorization
{
    public class CompanyHandler : AuthorizationHandler<CompanyRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CompanyRequirement requirement)
        {
            var userEmailAddress = context.User?.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
            
            if(userEmailAddress.EndsWith(requirement.DomainName))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            context.Fail();
            return Task.CompletedTask;
        }
    }
}
