using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookSite.Models.ViewModels
{
    public class NewBookSearchViewModel
    {
        public Guid ClubId { get; set; }
        public string InAuthor { get; set; }
        public string InTitle { get; set; }
        public string ISBN { get; set; }
        public string Subject { get; set; }
        public string Other { get; set; }
    }
}