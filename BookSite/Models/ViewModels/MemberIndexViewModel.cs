﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BookSite.Interfaces;
using BookSite.Models.SiteModels;

namespace BookSite.Models.ViewModels
{
    public class MemberIndexViewModel : IViewModel
    {
        public Guid MemberId { get; set; }
        public List<BookClub> Clubs { get; set; }
        public List<Member> Friends { get; set; }
        public List<Book> FriendsBooks { get; set; }
        public List<Book> Recommendations { get; set; }
        public List<Book> AuthorBooks { get; set; }
        public List<Book> SubjectBooks { get; set; }
    }
}