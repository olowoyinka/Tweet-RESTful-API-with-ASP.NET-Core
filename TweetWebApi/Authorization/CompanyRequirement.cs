using Microsoft.AspNetCore.Authorization;



namespace TweetWebApi.Authorization
{
    public class CompanyRequirement : IAuthorizationRequirement
    {
        public string DomainName { get; set; }

        public CompanyRequirement(string domainName)
        {
            DomainName = domainName;
        }
    }
}
