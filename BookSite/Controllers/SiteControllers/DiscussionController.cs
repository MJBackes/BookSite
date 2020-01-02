using BookSite.Models;
using BookSite.Models.EmailModels;
using BookSite.Models.SiteModels;
using BookSite.Models.ViewModels;
using BookSite.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace BookSite.Controllers.SiteControllers
{
    public class DiscussionController : Controller
    {
        public ApplicationDbContext db;
        public DiscussionController()
        {
            db = new ApplicationDbContext();
        }
        // GET: Discussion
        public ActionResult Index()
        {
            return View();
        }

        // GET: Discussion/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Discussion/Create
        public ActionResult Create(NewDiscussionInputViewModel viewModel)
        {
            Book book = db.Books.FirstOrDefault(b => b.GoogleVolumeId == viewModel.GoogleVolumeId);
            return View(new NewDiscussionViewModel { Book = book, ClubId = viewModel.ClubId});
        }

        // POST: Discussion/Create
        [HttpPost]
        public ActionResult Create(NewDiscussionViewModel viewModel)
        {
            try
            {
                DateTime Start = new DateTime
                    (
                    viewModel.Date.Year,
                    viewModel.Date.Month,
                    viewModel.Date.Day,
                    viewModel.StartTime.Hour,
                    viewModel.StartTime.Minute,
                    0
                    );
                if ((Start - DateTime.Now).Ticks < 0)
                    return View(viewModel);
                Discussion discussion = new Discussion
                {
                    Id = Guid.NewGuid(),
                    ClubId = viewModel.ClubId,
                    Name = viewModel.Name,
                    StartTime = Start,
                    Date = Start.Date
                };
                BookDiscussions bookDiscussions = new BookDiscussions
                {
                    Id = Guid.NewGuid(),
                    Book = db.Books.Find(viewModel.Book.Id),
                    Discussion = discussion
                };
                db.Discussions.Add(discussion);
                db.BookDiscussions.Add(bookDiscussions);
                db.SaveChanges();
                return RedirectToAction("Index", "Club", new { id = viewModel.ClubId });
            }
            catch
            {
                return View(viewModel);
            }
        }

        // GET: Discussion/Edit/5
        public ActionResult Edit(Guid id)
        {
            return View(db.Discussions.Find(id));
        }

        // POST: Discussion/Edit/5
        [HttpPost]
        public ActionResult Edit(Discussion discussion)
        {
            try
            {
                Discussion discussionFromDb = db.Discussions.Find(discussion.Id);
                discussionFromDb.Name = discussion.Name;
                DateTime Start = new DateTime
                    (
                    discussion.Date.Year,
                    discussion.Date.Month,
                    discussion.Date.Day,
                    discussion.StartTime.Hour,
                    discussion.StartTime.Minute,
                    0
                    );
                discussionFromDb.StartTime = Start;
                discussionFromDb.Date = Start.Date;
                db.SaveChanges();
                return RedirectToAction("Index", "Club", discussion.ClubId);
            }
            catch
            {
                return View();
            }
        }

        // GET: Discussion/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Discussion/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        [HttpGet]
        public ActionResult GoLive(Guid id)
        {
            Discussion discussion = db.Discussions.Find(id);
            //NotifyMembers(discussion);
            discussion.HasStarted = true;
            db.SaveChanges();
            return RedirectToAction("View", new { id = id });
        }
        [HttpGet]
        public ActionResult Close(Guid id)
        {
            Discussion discussion = db.Discussions.Find(id);
            discussion.HasEnded = true;
            db.SaveChanges();
            return RedirectToAction("Index", "Club", new { id = discussion.ClubId });
        }
        [HttpGet]
        public ActionResult View(Guid id)
        {
            var userId = User.Identity.GetUserId();
            ViewDiscussionViewModel viewModel = GetViewDiscussionViewModel(userId, id);
            return View(viewModel);
        }

        //private methods
        private async Task NotifyMembers(Discussion discussion)
        {
            BookClub club = db.BookClubs.Find(discussion.ClubId);
            List<ClubMembers> clubMembers = db.ClubMembers.Include("Member").Where(cm => cm.ClubId == club.Id).ToList();
            discussion.Book = db.BookDiscussions.Include("Book").FirstOrDefault(bd => bd.DiscussionId == discussion.Id).Book;
            foreach (ClubMembers cm in clubMembers)
                await BookSiteEmailService.SendEmail(BuildEmailModel(cm.Member, discussion));
        }
        private EmailModel BuildEmailModel(Member member, Discussion discussion)
        {
            string BaseURL = "";
            string MemberEmail = db.Users.Find(member.ApplicationUserId).Email;
            string URL = $"{BaseURL}/Discussion/View/{discussion.Id}";
            return new EmailModel
            {
                FromDisplayName = "BookSite",
                ToName = member.FirstName,
                ToEmailAddress = MemberEmail,
                Subject = $"The Book Discussion {discussion.Name} has just started!",
                Message = $"{member.FirstName}, \n\n A book club you are a memeber of just began their discussion of {discussion.Book.Title}. Go to \n {URL} \n to participate."
            };
        }

        private ViewDiscussionViewModel GetViewDiscussionViewModel(string userId, Guid id)
        {
            Member member = db.Members.FirstOrDefault(m => m.ApplicationUserId == userId);
            Discussion discussion = db.Discussions.FirstOrDefault(d => d.Id == id);
            Book book = db.BookDiscussions.Include("Book").FirstOrDefault(bd => bd.DiscussionId == discussion.Id).Book;
            List<Comment> comments = db.Comments.Include("Member").Where(c => c.DiscussionId == discussion.Id).OrderBy(c => c.TimeOfPost).ToList();
            ViewDiscussionViewModel viewModel = new ViewDiscussionViewModel
            {
                Member = member,
                Book = book,
                Discussion = discussion,
                Comments = comments
            };
            return viewModel;
        }
    }
}
