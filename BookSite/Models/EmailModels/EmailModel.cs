using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookSite.Models.EmailModels
{
    public class EmailModel
    {
        public string FromDisplayName { get; set; }
        public string FromEmailAddress { get; set; }
        public string ToName { get; set; }
        public string ToEmailAddress { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }
}