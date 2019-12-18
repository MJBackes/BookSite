using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookSite.Models.MiscModels
{
    public class Search
    {
        public string intitle { get; set; }
        public string inauthor { get; set; }
        public string subject { get; set; }
        public string isbn { get; set; }
        public string other { get; set; }
    }
}