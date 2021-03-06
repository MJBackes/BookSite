﻿using BookSite.APIHandlers;
using BookSite.Models;
using BookSite.Models.APIResponseModels;
using BookSite.Models.MiscModels;
using BookSite.Models.SiteModels;
using BookSite.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace BookSite.Controllers.SiteControllers
{
    public class BookController : Controller
    {
        private ApplicationDbContext db;
        public BookController()
        {
            db = new ApplicationDbContext();
        }
        // GET: Book
        public ActionResult Index()
        {
            return View();
        }

        // GET: Book/Details/5
        [HttpGet]
        public ActionResult Details(string id)
        {
            BookDetailsViewModel viewModel = GetBookDetailsViewModel(id);
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult NavBarSearch(NavBarSearchViewModel viewModel)
        {
            return RedirectToAction("NavBarSearchResults",new { input = viewModel.Search});
        }
        [HttpGet]
        public ActionResult NavBarSearchResults(string input)
        {
            GoogleBooksSearchResponse response = GoogleBooksAPIHandler.NavBarSearch(input);
            return View("NavBarSearchResults",Utilities.GoogleBookSearchUtilities.ParseSearchResponse(response));
        }
        [HttpGet]
        public ActionResult SearchResults(Search search)
        {
            GoogleBooksSearchResponse response = GoogleBooksAPIHandler.FullSearch(search);
            return View(Utilities.GoogleBookSearchUtilities.ParseSearchResponse(response));
        }
        [HttpGet]
        public ActionResult AuthorSearch(string InAuthor)
        {
            return RedirectToAction("SearchResults", new Search { inauthor = InAuthor });
        }
        [HttpGet]
        public ActionResult Search()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Search(Search search)
        {
            return RedirectToAction("SearchResults", search);
        }

        //Private Methods
        private BookDetailsViewModel GetBookDetailsViewModel(string id)
        {
            BookDetailsViewModel viewModel = new BookDetailsViewModel();
            viewModel.Book = Utilities.GoogleBookSearchUtilities.ParseSingleSearchResponse(GoogleBooksAPIHandler.SingleSearch(id).Result);
            viewModel.Reviews = db.Reviews.Include("Member").Where(r => r.BookId == viewModel.Book.Id).ToList();
            viewModel.RelatedBooks = GetRelatedBooks(viewModel.Book);
            return viewModel;
        }
        private List<Book> GetRelatedBooks(Book book)
        {
            List<Guid> collectionIds = db.CollectionBooks.Include("Collection")
                                                             .Where(cb => cb.BookId == book.Id)
                                                             .Select(cb => cb.Collection.Id)
                                                             .ToList();
            return db.CollectionBooks.Include("Book")
                                     .Where(cb => collectionIds.Contains(cb.CollectionId))
                                     .Select(cb => cb.Book)
                                     .GroupBy(b => b.GoogleVolumeId, (googleId, Books) => new
                                     {
                                         Key = googleId,
                                         Count = Books.Count(),
                                         Value = Books.FirstOrDefault()
                                     }).OrderByDescending(g => g.Count)
                                       .Select(g => g.Value)
                                       .AsEnumerable()
                                       .SkipWhile(b => b.Id == book.Id)
                                       .Take(5)
                                       .ToList();
        }
    }
}
