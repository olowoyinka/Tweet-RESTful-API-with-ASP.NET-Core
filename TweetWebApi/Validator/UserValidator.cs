using FluentValidation;
using TweetWebApi.EndPoint.Request;

namespace TweetWebApi.Validator
{
    public class UserValidator : AbstractValidator<UserRequest>
    {
        public UserValidator()
        {
            RuleFor(x => x.Email)
                .EmailAddress()
                .NotEmpty();

            RuleFor(x => x.Password)
                .MinimumLength(8);
    
        }
    }

    public class PostValidator : AbstractValidator<PostRequest>
    {
        public PostValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty();

            RuleFor(x => x.Tags)
                .NotEmpty();

        }
    }

}
