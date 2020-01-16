using System.Collections.Generic;



namespace TweetWebApi.Validator
{
    public class ValidatorResponse
    {
        public List<ValidatorModel> Errors { get; set; } = new List<ValidatorModel>();
    }
}
