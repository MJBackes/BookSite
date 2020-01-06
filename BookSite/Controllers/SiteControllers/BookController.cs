using BookSite.APIHandlers;
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

        [HttpGet]
        public ActionResult NavBarSearch()
        {
            return View();
        }
        [HttpPost]
        public ActionResult NavBarSearch(string input)
        {
            GoogleBooksSearchResponse response = GoogleBooksAPIHandler.NavBarSearch(input);
            return View(ParseSearchResponse(response));
        }
        [HttpGet]
        public ActionResult SearchResults(Search search)
        {
            GoogleBooksSearchResponse response = GoogleBooksAPIHandler.FullSearch(search);
            return View(ParseSearchResponse(response));
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
        private Book ParseSingleSearchResponse(GoogleBookSingleResponse response)
        {
            Book book = db.Books.FirstOrDefault(b => b.GoogleVolumeId == response.id);
            if (book != null)
            {
                book.Authors = GetAuthorString(response.volumeInfo.authors);
                book.Categories = GetCategoryString(response.volumeInfo.categories);
                return book;
            }
            else
            {
                return AddNewSingleBook(response);
            }
        }
        private Book AddNewSingleBook(GoogleBookSingleResponse response)
        {
            Book book = new Book()
            {
                Id = Guid.NewGuid(),
                GoogleVolumeId = response.id,
                Description = response.volumeInfo.description,
                Title = response.volumeInfo.title,
                PageCount = response.volumeInfo.pageCount,
                Thumbnail = response.volumeInfo.imageLinks == null ? null : response.volumeInfo.imageLinks.thumbnail
            };
            book.Authors = GetAuthorString(response.volumeInfo.authors);
            AddBookAuthorJunctionEntries(book, response.volumeInfo.authors);
            book.Categories = GetCategoryString(response.volumeInfo.categories);
            AddBookTagJunctionEntries(book, response.volumeInfo.categories);
            db.Books.Add(book);
            db.SaveChanges();
            return book;
        }
        private List<Book> ParseSearchResponse(GoogleBooksSearchResponse response)
        {
            List<Book> output = new List<Book>();
            foreach(Item item in response.items)
            {
                output.Add(ParseSearchItem(item));
            }
            return output;
        }
        private Book ParseSearchItem(Item item)
        {
            Book book = db.Books.FirstOrDefault(b => b.GoogleVolumeId == item.id);
            if (book != null)
            {
                book.Authors = GetAuthorString(item.volumeInfo.authors);
                book.Categories = GetCategoryString(item.volumeInfo.categories);
                return book;
            }
            else
            {
                return AddNewVolumeBook(item);
            }
        }
        private Book AddNewVolumeBook(Item item)
        {
            Book book = new Book()
            {
                Id = Guid.NewGuid(),
                GoogleVolumeId = item.id,
                Description = item.volumeInfo.description,
                Title = item.volumeInfo.title,
                PageCount = item.volumeInfo.pageCount,
                Thumbnail = item.volumeInfo.imageLinks == null ? null : item.volumeInfo.imageLinks.thumbnail
            };
            book.Authors = GetAuthorString(item.volumeInfo.authors);
            AddBookAuthorJunctionEntries(book, item.volumeInfo.authors);
            book.Categories = GetCategoryString(item.volumeInfo.categories);
            AddBookTagJunctionEntries(book, item.volumeInfo.categories);
            return book;
        }
        private void AddBookAuthorJunctionEntries(Book book, string[] authors)
        {
            if (authors != null)
            {
                foreach (string s in authors)
                {
                    Author author = db.Authors.FirstOrDefault(a => a.Name == s);
                    db.BookAuthors.Add(new BookAuthors { Id = Guid.NewGuid(), Author = author, Book = book });
                }
                db.SaveChanges();
            }
        }
        private void AddBookTagJunctionEntries(Book book, string[] categories)
        {
            if (categories != null) 
            { 
                foreach (string s in categories)
                    {
                        GenreTag tag = db.GenreTags.FirstOrDefault(t => t.Name == s);
                        db.BookTags.Add(new BookTags { Id = Guid.NewGuid(), BookId = book.Id, TagId = tag.Id });
                    }
            }
            db.SaveChanges();
        }
        private string GetCategoryString(string[] categories)
        {
            StringBuilder output = new StringBuilder("");
            if(categories != null)
                for(int i = 0; i < categories.Length; i++)
                {
                    string categoryName = categories[i];
                    if(db.GenreTags.FirstOrDefault(g => g.Name == categoryName) == null)
                    {
                        db.GenreTags.Add(new GenreTag { Id = Guid.NewGuid(), Name = categoryName });
                        db.SaveChanges();
                    }
                    output.Append(categories[i]);
                    if (i != categories.Length - 1)
                        output.Append(" , ");
                }
            return output.ToString();
        }
        private string GetAuthorString(string[] authors)
        {
            StringBuilder output = new StringBuilder("");
            if(authors != null)
                for(int i = 0; i < authors.Length; i++)
                {
                    string authorName = authors[i];
                    if (db.Authors.FirstOrDefault(a => a.Name == authorName) == null)
                    {
                        db.Authors.Add(new Author { Name = authorName, Id = Guid.NewGuid() });
                        db.SaveChanges();
                    }
                    output.Append(authors[i]);
                    if (i != authors.Length - 1)
                        output.Append(" , ");
                }
            return output.ToString();
        }
        private BookDetailsViewModel GetBookDetailsViewModel(string id)
        {
            BookDetailsViewModel viewModel = new BookDetailsViewModel();
            viewModel.Book = ParseSingleSearchResponse(GoogleBooksAPIHandler.SingleSearch(id).Result);
            viewModel.Reviews = db.Reviews.Include("Member").Where(r => r.BookId == viewModel.Book.Id).ToList();
            viewModel.RelatedBooks = GetRelatedBooks(viewModel.Book);
            return viewModel;
        }
        private List<Book> GetRelatedBooks(Book book)
        {
            List<Collection> collections = db.CollectionBooks.Include("Collection")
                                                             .Where(cb => cb.BookId == book.Id)
                                                             .Select(cb => cb.Collection)
                                                             .ToList();
            List<Book> books = new List<Book>();
            foreach(Collection c in collections)
            {
                books = books.Concat(db.CollectionBooks.Include("Book")
                                                       .Where(cb => cb.CollectionId == c.Id)
                                                       .Select(cb => cb.Book))
                                                       .ToList();
            }
            books = books.GroupBy(b => b.GoogleVolumeId, (googleId, Books) => new
                {
                    Key = googleId,
                    Count = Books.Count(),
                    Value = Books.First()
                }).OrderByDescending(g => g.Count)
                  .Select(g => g.Value)
                  .Take(6)
                  .ToList();
            books.Remove(book);
            return books;
        }
    }
}
