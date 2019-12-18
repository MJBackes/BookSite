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
    public class MemberController : Controller
    {
        ApplicationDbContext db;
        public MemberController()
        {
            db = new ApplicationDbContext();
        }
        // GET: Member
        public ActionResult Index()
        {
            return View();
        }

        // GET: Member/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Member/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Member/Create
        [HttpPost]
        public ActionResult Create(Member member)
        {
            try
            {
                member.Id = Guid.NewGuid();
                var userId = User.Identity.GetUserId();
                member.ApplicationUserId = userId;
                db.Members.Add(member);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View(member);
            }
        }

        // GET: Member/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Member/Edit/5
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

        // GET: Member/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Member/Delete/5
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
        public ActionResult AddBookToCollection()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddBookToCollection(Book book)
        {
            var userId = User.Identity.GetUserId();
            Member member = db.Members.FirstOrDefault(m => m.ApplicationUserId == userId);
            Collection collection = db.Collections.FirstOrDefault(c => c.MemberId == member.Id);
            if(collection == null)
            {
                db.Collections.Add(new Collection { Id = Guid.NewGuid(), MemberId = member.Id });
                db.SaveChanges();
                collection = db.Collections.FirstOrDefault(c => c.MemberId == member.Id);
            }
            db.CollectionBooks.Add(new CollectionBooks { Id = Guid.NewGuid(), BookId = book.Id, CollectionId = collection.Id });
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult RemoveBookFromCollection()
        {
            return View();
        }
        [HttpPost]
        public ActionResult RemoveBookFromCollection(Book book)
        {
            var userId = User.Identity.GetUserId();
            Member member = db.Members.FirstOrDefault(m => m.ApplicationUserId == userId);
            Collection collection = db.Collections.FirstOrDefault(c => c.MemberId == member.Id);
            Book bookFromDB = db.Books.Find(book.Id);
            db.CollectionBooks.Remove(db.CollectionBooks.FirstOrDefault(c => c.CollectionId == collection.Id && c.BookId == bookFromDB.Id));
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
