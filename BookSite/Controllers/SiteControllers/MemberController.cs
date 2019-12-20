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
        public ActionResult AddBookToCollection(string id)
        {
            return View(db.Books.FirstOrDefault(b => b.GoogleVolumeId == id));
        }
        [HttpPost]
        public ActionResult AddBookToCollection(Book book)
        {
            var userId = User.Identity.GetUserId();
            if (userId == null)
                return RedirectToAction("Login", "Account");
            Member member = db.Members.FirstOrDefault(m => m.ApplicationUserId == userId);
            Collection collection = db.Collections.FirstOrDefault(c => c.MemberId == member.Id);
            if(collection == null)
            {
                db.Collections.Add(new Collection { Id = Guid.NewGuid(), MemberId = member.Id });
                db.SaveChanges();
                collection = db.Collections.FirstOrDefault(c => c.MemberId == member.Id);
            }
            Book bookFromDb = db.Books.FirstOrDefault(b => b.GoogleVolumeId == book.GoogleVolumeId);
            db.CollectionBooks.Add(new CollectionBooks { Id = Guid.NewGuid(), Book = bookFromDb, Collection = collection });
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult RemoveBookFromCollection(string id)
        {
            Book book = db.Books.FirstOrDefault(b => b.GoogleVolumeId == id);
            return View(book);
        }
        [HttpPost]
        public ActionResult RemoveBookFromCollection(Book book)
        {
            var userId = User.Identity.GetUserId();
            Member member = db.Members.FirstOrDefault(m => m.ApplicationUserId == userId);
            Collection collection = db.Collections.FirstOrDefault(c => c.MemberId == member.Id);
            Book bookFromDB = db.Books.FirstOrDefault(b => b.GoogleVolumeId == book.GoogleVolumeId);
            db.CollectionBooks.Remove(db.CollectionBooks.FirstOrDefault(c => c.CollectionId == collection.Id && c.BookId == bookFromDB.Id));
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult MyShelf()
        {
            var userId = User.Identity.GetUserId();
            if (userId == null)
                return RedirectToAction("Login", "Account");
            return View(GetBookCollection(userId));
        }

        private List<Book> GetBookCollection(string userId)
        {
            Member member = db.Members.FirstOrDefault(m => m.ApplicationUserId == userId);
            Collection collection = db.Collections.FirstOrDefault(c => c.MemberId == member.Id);
            List<CollectionBooks> collectionBooks = db.CollectionBooks.Where(cb => cb.CollectionId == collection.Id).ToList();
            List<Book> books = new List<Book>();
            foreach (CollectionBooks cb in collectionBooks)
                books.Add(db.Books.Find(cb.BookId));
            return books;
        }
    }
}
