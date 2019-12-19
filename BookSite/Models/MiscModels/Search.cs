using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BookSite.Models.MiscModels
{
    public class Search
    {
        [Display(Name = "Title")]
        public string intitle { get; set; }
        [Display(Name = "Author")]
        public string inauthor { get; set; }
        [Display(Name = "Subject")]
        public string subject { get; set; }
        [Display(Name = "ISBN-10")]
        public string isbn { get; set; }
        [Display(Name = "Other")]
        public string other { get; set; }
    }
}