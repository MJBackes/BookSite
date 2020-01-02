using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BookSite.Models.SiteModels;

namespace BookSite.Models.ViewModels
{
    public class MemberIndexViewModel
    {
        public Guid MemberId { get; set; }
        public List<BookClub> Clubs { get; set; }
        public List<Member> Friends { get; set; }
        public List<Book> FriendsBooks { get; set; }
    }
}