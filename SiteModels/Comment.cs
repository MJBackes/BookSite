using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BookSite.Models.SiteModels
{
    public class Comment
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("Discussion")]
        public Guid DiscussionId { get; set; }
        public Discussion Discussion { get; set; }
        [ForeignKey("Book")]
        public Guid BookId { get; set; }
        public Book Book { get; set; }
        [ForeignKey("Member")]
        public Guid MemberId { get; set; }
        public Member Member { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime TimeOfPost { get; set; }
    }
}