using BookSite.Models.SiteModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookSite.Models.ViewModels
{
    public class BookDetailsViewModel
    {
        public Book Book { get; set; }
        public List<Book> RelatedBooks { get; set; }
        public List<Review> Reviews { get; set; }
    }
}