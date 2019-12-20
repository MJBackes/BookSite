using BookSite.Models;
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
        public ActionResult Details(int id)
        {
            return View();
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
        //[HttpGet]
        //public ActionResult Join(BookClub club)
        //{

        //}

        // GET: Club/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Club/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

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
    }
}
