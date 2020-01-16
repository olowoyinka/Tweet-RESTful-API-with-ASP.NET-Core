using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace TweetWebApi.Models
{
    public class Tag
    {
        [Key]
        public string Name { get; set; }

        public string CreatedId { get; set; }

        [ForeignKey(nameof(CreatedId))]
        public IdentityUser CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
