using BookSite.Models.SiteModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookSite.Models.ViewModels
{
    public class NewDiscussionInputViewModel
    {
        public Guid ClubId { get; set; }
        public string GoogleVolumeId { get; set; }
    }
}