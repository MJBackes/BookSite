using BookSite.Models.SiteModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookSite.Models.ViewModels
{
    public class MemberDetailsViewModel
    {
        public Member Member { get; set; }
        public List<Review> Reviews { get; set; } 
        public List<Book> Books { get; set; }
        public bool isFriend { get; set; }
    }
}