using BookSite.Models;
using BookSite.Models.SiteModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace BookSite.Controllers.SiteControllers
{
    public class ReviewController : Controller
    {
        public ApplicationDbContext db;
        public ReviewController()
        {
            db = new ApplicationDbContext();
        }
        // GET: Review
        public ActionResult Index()
        {
            return View();
        }

        // GET: Review/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Review/Create
        public ActionResult Create(string id)
        {
            var userId = User.Identity.GetUserId();
            if (userId == null)
                return RedirectToAction("Login", "Account", null);
            Member member = db.Members.FirstOrDefault(m => m.ApplicationUserId == userId);
            Book book = GetBookFromGoogleId(id);
            ViewBag.Book = book;
            Review previousReview = db.Reviews.FirstOrDefault(r => r.MemberId == member.Id && r.BookId == book.Id);
            if (previousReview != null)
            {
                string outId = id;
                return RedirectToAction("Edit", new { id = outId });
            }
            return View();
        }

        // POST: Review/Create
        [HttpPost]
        public ActionResult Create(Review review)
        {
            try
            {
                var userId = User.Identity.GetUserId();
                if (userId == null)
                    return RedirectToAction("Login", "Account", null);
                Member member = db.Members.FirstOrDefault(m => m.ApplicationUserId == userId);
                review.MemberId = member.Id;
                review.Id = Guid.NewGuid();
                db.Reviews.Add(review);
                db.SaveChanges();
                return RedirectToAction("Details", "Book", new { id = db.Books.Find(review.BookId).GoogleVolumeId });
            }
            catch
            {
                return View();
            }
        }

        // GET: Review/Edit/5
        public ActionResult Edit(string id)
        {
            var userId = User.Identity.GetUserId();
            if (userId == null)
                return RedirectToAction("Login", "Account", null);
            Member member = db.Members.FirstOrDefault(m => m.ApplicationUserId == userId);
            Book book = GetBookFromGoogleId(id);
            ViewBag.Book = book;
            Review review = db.Reviews.FirstOrDefault(r => r.MemberId == member.Id && r.BookId == book.Id);
            return View(review);
        }

        // POST: Review/Edit/5
        [HttpPost]
        public ActionResult Edit(Review review)
        {
            try
            {
                var userId = User.Identity.GetUserId();
                if (userId == null)
                    return RedirectToAction("Login", "Account");
                Member member = db.Members.FirstOrDefault(m => m.ApplicationUserId == userId);
                if (review.MemberId != member.Id)
                    return RedirectToAction("Details","Book", new { id = db.Books.Find(review.BookId).GoogleVolumeId });
                Review reviewFromDb = db.Reviews.FirstOrDefault(r => r.MemberId == member.Id && r.BookId == review.BookId);
                reviewFromDb.Title = review.Title;
                reviewFromDb.Rating = review.Rating;
                reviewFromDb.Body = review.Body;
                db.SaveChanges();
                return RedirectToAction("Details", "Book", new { id = db.Books.Find(review.BookId).GoogleVolumeId });
            }
            catch
            {
                ViewBag.Book = db.Books.Find(review.BookId);
                return View(review);
            }
        }

        // GET: Review/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Review/Delete/5
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

        private string GetCategoryString(GenreTag[] categories)
        {
            StringBuilder output = new StringBuilder("");
            if (categories != null)
                for (int i = 0; i < categories.Length; i++)
                {
                    output.Append(categories[i].Name);
                    if (i != categories.Length - 1)
                        output.Append(",");
                }
            return output.ToString();
        }
        private string GetAuthorString(Author[] authors)
        {
            StringBuilder output = new StringBuilder("");
            if (authors != null)
                for (int i = 0; i < authors.Length; i++)
                {
                    output.Append(authors[i].Name);
                    if (i != authors.Length - 1)
                        output.Append(",");
                }
            return output.ToString();
        }
        private Author[] GetAuthorArray(Book book)
        {
            List<BookAuthors> bookAuthors = db.BookAuthors.Where(a => a.BookId == book.Id).ToList();
            Author[] authors = new Author[bookAuthors.Count];
            for (int i = 0; i < bookAuthors.Count; i++)
            {
                Author author = db.Authors.Find(bookAuthors[i].AuthorId);
                authors[i] = author;
            }
            return authors;
        }
        private GenreTag[] GetCategoryArray(Book book)
        { 
            List<BookTags> bookTags = db.BookTags.Where(a => a.BookId == book.Id).ToList();
            GenreTag[] tags = new GenreTag[bookTags.Count];
            for(int i = 0; i < bookTags.Count; i++)
            {
                GenreTag tag = db.GenreTags.Find(bookTags[i].TagId);
                tags[i] = tag;
            }
            return tags;
        }
        private Book GetBookFromGoogleId(string id)
        {
            Book book = db.Books.FirstOrDefault(b => b.GoogleVolumeId == id);
            book.Categories = GetCategoryString(GetCategoryArray(book));
            book.Authors = GetAuthorString(GetAuthorArray(book));
            return book;
        }
    }
}
