using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BookSite.Models.SiteModels
{
    public class BookClub
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string PrivacyLevel { get; set; }
        public string Description { get; set; }
        [ForeignKey("NextBook")]
        public Guid? NextBookId { get; set; }
        [Display(Name = "Current Book")]
        public Book NextBook { get; set; }
        [NotMapped]
        public bool NameIsTaken { get; set; }
    }
}