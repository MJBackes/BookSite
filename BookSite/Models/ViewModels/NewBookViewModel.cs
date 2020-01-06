using BookSite.Models.SiteModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BookSite.Models.MiscModels;
using BookSite.Interfaces;

namespace BookSite.Models.ViewModels
{
    public class NewBookViewModel : IViewModel
    {
        public Guid ClubId { get; set; }
        public string GoogleVolumeId { get; set; }
        public Search Search { get; set; }
        public List<Book> Books { get; set; }
        public Book Book { get; set; }
        public List<Review> Reviews { get; set; }
    }
}