using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BookSite.Models.SiteModels
{
    public class Discussion
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }

        [ForeignKey("BookClub")]
        public Guid ClubId { get; set; }
        public BookClub BookClub { get; set; }
        public DateTime StartTime { get; set; }
    }
}