using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BookSite.Models.SiteModels
{
    public class ClubMembers
    {
        [ForeignKey("Club")]
        public Guid ClubId { get; set; }
        public BookClub Club { get; set; }
        [ForeignKey("Member")]
        public Guid MemberId { get; set; }
        public Member Member { get; set; }
        public bool IsManager { get; set; }
    }
}