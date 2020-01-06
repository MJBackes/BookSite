using BookSite.Interfaces;
using BookSite.Models.SiteModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookSite.Models.ViewModels
{
    public class RemoveFriendViewModel : IViewModel
    {
        public Member Friend { get; set; }
    }
}