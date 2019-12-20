using BookSite.Models;
using BookSite.Models.MiscModels;
using BookSite.Models.SiteModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookSite.Controllers.SiteControllers
{
    public class ClubController : Controller
    {
        public ApplicationDbContext db;
        private string[] COMMON_WORD_ARRAY = new string[] { "the", "a"};
        public ClubController()
        {
            db = new ApplicationDbContext();
        }
        // GET: Club
        public ActionResult Index()
        {
            return View();
        }

        // GET: Club/Details/5
        public ActionResult Details(Guid? id)
        {
            return View(db.BookClubs.Find(id));
        }

        // GET: Club/Create
        public ActionResult Create()
        {
            var userId = User.Identity.GetUserId();
            if (userId == null)
                return RedirectToAction("Login", "Account");
            return View();
        }

        // POST: Club/Create
        [HttpPost]
        public ActionResult Create(BookClub club)
        {
            try
            {
                var userId = User.Identity.GetUserId();
                if (userId == null)
                    return RedirectToAction("Login", "Account");
                Member member = db.Members.FirstOrDefault(m => m.ApplicationUserId == userId);
                club.NameIsTaken = false;
                if(club.Id == default)
                    club.Id = Guid.NewGuid();
                if(db.BookClubs.FirstOrDefault(c => c.Name == club.Name) != null)
                {
                    club.NameIsTaken = true;
                    return View(club);
                } 
                db.BookClubs.Add(club);
                db.ClubMembers.Add(new ClubMembers { Id = Guid.NewGuid(), Club = club, Member = member, IsManager = true });
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        [HttpGet]
        public ActionResult Join(Guid? id)
        {
            return View(db.BookClubs.Find(id));
        }
        [HttpPost]
        public ActionResult Join(BookClub club)
        {
            var userId = User.Identity.GetUserId();
            if (userId == null)
                return RedirectToAction("Login", "Account");
            Member member = db.Members.FirstOrDefault(m => m.ApplicationUserId == userId);
            club = db.BookClubs.Find(club.Id);
            if(db.ClubMembers.FirstOrDefault(cm => cm.MemberId == member.Id && cm.ClubId == club.Id) == null)
            {
                db.ClubMembers.Add(new ClubMembers { Id = Guid.NewGuid(), Club = club, Member = member });
                db.SaveChanges();
            }
            return RedirectToAction("Details", club);
        }

        // GET: Club/Edit/5
        public ActionResult Edit(Guid? id)
        {
            return View(db.BookClubs.Find(id));
        }

        // POST: Club/Edit/5
        [HttpPost]
        public ActionResult Edit(BookClub club)
        {
            try
            {
                club.NameIsTaken = false;
                var userId = User.Identity.GetUserId();
                if (userId == null)
                    return RedirectToAction("Login", "Account");
                BookClub bookClubWithSameName = db.BookClubs.FirstOrDefault(bc => bc.Name == club.Name);
                if (bookClubWithSameName != null && bookClubWithSameName.Id != club.Id)
                {
                    club.NameIsTaken = true;
                    return View(club);
                }
                Member member = db.Members.FirstOrDefault(m => m.ApplicationUserId == userId);
                ClubMembers clubMembers = db.ClubMembers.FirstOrDefault(cb => cb.MemberId == member.Id && cb.ClubId == club.Id);
                if (clubMembers.IsManager)
                {
                    BookClub clubFromDb = db.BookClubs.Find(club.Id);  
                    clubFromDb.Name = club.Name;
                    clubFromDb.PrivacyLevel = club.PrivacyLevel;
                    clubFromDb.Description = club.Description;
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Club/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Club/Delete/5
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
        public ActionResult Search()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Search(SimpleTextSearch search)
        {
            if (search.Input != null)
            {
                string[] inputArray = search.Input.ToLower().Split(' ');
                List<BookClub> clubs = new List<BookClub>();
                foreach (string s in inputArray)
                {
                    List<BookClub> matchingClubs = db.BookClubs.Include("NextBook").Where(c => (c.Name.Contains(s.Trim()) || c.Description.Contains(s.Trim())) && c.PrivacyLevel == "Public").ToList();
                    clubs = clubs.Concat(matchingClubs).ToList();
                }
                return View("SearchResults", clubs);
            }
            return View(search);
        }
        [HttpGet]
        public ActionResult ChooseNewBook()
        {
            var userId = User.Identity.GetUserId();
            if (userId == null)
                return RedirectToAction("Login", "Account");
            return View();
        }
        [HttpPost]
        public ActionResult ChooseNewBook(Guid? clubId,Book book)
        {
            var userId = User.Identity.GetUserId();
            if (userId == null)
                return RedirectToAction("Login", "Account");
            Member member = db.Members.FirstOrDefault(m => m.ApplicationUserId == userId);
            BookClub club = db.BookClubs.Find(clubId);
            if(db.ClubMembers.FirstOrDefault(cm => cm.MemberId == member.Id && cm.ClubId == club.Id).IsManager)
            {
                club.NextBookId = book.Id;
                db.SaveChanges();
            }
            return View("Index");
        }
    }
}
