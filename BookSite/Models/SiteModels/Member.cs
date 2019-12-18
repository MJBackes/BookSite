using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BookSite.Models.SiteModels
{
    public class Member
    {
        [Key]
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastNme { get; set; }
        public string DisplayName { get; set; }
        public string UserImage { get; set; }
        [ForeignKey("ApplicationUser")]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}