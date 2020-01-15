using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BookSite.Models.SiteModels
{
    public class BookAuthors
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("Book")]
        public Guid BookId { get; set; }
        public Book Book { get; set; }
        [ForeignKey("Author")]
        public Guid AuthorId { get; set; }
        public Author Author { get; set; }
    }
}