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
        public string ISBN { get; set; }
        public int PageCount { get; set; }
        [ForeignKey("Author")]
        public Guid AuthorId { get; set; }
        public Author Author { get; set; }
    }
}