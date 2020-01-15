using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BookSite.Models.SiteModels
{
    public class Book
    {
        [Key]
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string GoogleVolumeId { get; set; }
        public int PageCount { get; set; }
        public string Thumbnail { get; set; }
        [NotMapped]
        public string Categories { get; set; }
        [NotMapped]
        public string Authors { get; set; }
    }
}