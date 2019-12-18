using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BookSite.Models.SiteModels
{
    public class Collection
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("Member")]
        public Guid MemberId { get; set; }
        public Member Member { get; set; }
    }
}