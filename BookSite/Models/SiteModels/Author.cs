using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BookSite.Models.SiteModels
{
    public class Author
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}