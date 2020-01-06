using BookSite.Interfaces;
using BookSite.Models.SiteModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BookSite.Models.ViewModels
{
    public class NewDiscussionViewModel : IViewModel
    {
        public string Name { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM-dd-yyyy}")]
        public DateTime Date { get; set; }
        [Display(Name = "Start Time")]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:H:mm tt}")]
        public DateTime StartTime { get; set; }
        public Book Book { get; set; }
        public Guid ClubId { get; set; }
        
    }
}