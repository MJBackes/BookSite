using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BookSite.Models.SiteModels
{
    public class CollectionBooks
    {
        [ForeignKey("Collection")]
        public Guid CollectionId { get; set; }
        public Collection Collection { get; set; }
        [ForeignKey("Book")]
        public Guid BookId { get; set; }
        public Book Book { get; set; }
    }
}