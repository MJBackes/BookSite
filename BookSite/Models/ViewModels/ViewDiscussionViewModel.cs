using BookSite.Interfaces;
using BookSite.Models.SiteModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookSite.Models.ViewModels
{
    public class ViewDiscussionViewModel : IViewModel
    {
        public List<Comment> Comments { get; set; }
        public Discussion Discussion { get; set; }
        public Member Member { get; set; }
        public Book Book { get; set; }
    }
}