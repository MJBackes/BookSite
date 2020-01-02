using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using BookSite.Models;
using BookSite.Models.SiteModels;
using Microsoft.AspNet.SignalR;

namespace BookSite
{
    public class ChatHub : Hub
    {
        ApplicationDbContext db;
        public ChatHub()
        {
            db = new ApplicationDbContext();
        }
        public async Task SendAsync(string name, string message, string discussionId,string memberId, string bookId)
        {
            DateTime now = DateTime.Now;
            Comment comment = new Comment
            {
                Id = Guid.NewGuid(),
                Body = message,
                MemberId = Guid.Parse(memberId),
                DiscussionId = Guid.Parse(discussionId),
                BookId = Guid.Parse(bookId),
                TimeOfPost = now
            };
            db.Comments.Add(comment);
            db.SaveChanges();
            await Clients.All.addNewMessageToPage(name, message, now.ToShortDateString(), now.ToShortTimeString());
        }
    }
}