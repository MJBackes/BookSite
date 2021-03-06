﻿using BookSite.Interfaces;
using BookSite.Models.SiteModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookSite.Models.ViewModels
{
    public class ClubIndexViewModel :IViewModel
    {
        public BookClub Club { get; set; }
        public List<Discussion> Discussions { get; set; }
        public List<Member> Members { get; set; }
        public bool IsManager { get; set; }
        public List<Book> Books { get; set; }
    }
}