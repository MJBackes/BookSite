using BookSite.APIHandlers;
using BookSite.Models;
using BookSite.Models.APIResponseModels;
using BookSite.Models.MiscModels;
using BookSite.Models.SiteModels;
using BookSite.Models.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        [HttpGet]
        public ActionResult Index(Guid id)
        {
            BookClub club = db.BookClubs.Find(id);
            ClubIndexViewModel viewModel = GetClubIndexViewModel(club);
            return View(viewModel);
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
                return RedirectToAction("Index",club);
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
        public ActionResult Edit(Guid id)
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

                return RedirectToAction("Index", new { id = club.Id});
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
        public ActionResult ChooseNewBook(NewBookViewModel viewModel)
        {
            var userId = User.Identity.GetUserId();
            if (userId == null)
                return RedirectToAction("Login", "Account");
            return RedirectToAction("Create","Discussion",new NewDiscussionInputViewModel { ClubId = viewModel.ClubId, GoogleVolumeId = viewModel.GoogleVolumeId});
        }
        [HttpGet]
        public ActionResult NewBookSearch(Guid clubId)
        {
            NewBookViewModel viewModel = new NewBookViewModel { ClubId = clubId };
            return View(viewModel);
        }
        [HttpPost]
        public ActionResult NewBookSearch(NewBookViewModel viewModel)
        {
            return RedirectToAction("NewBookSearchResults", new NewBookSearchViewModel { ClubId = viewModel.ClubId, InAuthor = viewModel.Search.inauthor, InTitle = viewModel.Search.intitle, ISBN = viewModel.Search.isbn, Other = viewModel.Search.other, Subject = viewModel.Search.subject});
        }
        [HttpGet]
        public ActionResult NewBookSearchResults(NewBookSearchViewModel input)
        {
            GoogleBooksSearchResponse response = GoogleBooksAPIHandler.FullSearch(new Models.MiscModels.Search { inauthor = input.InAuthor, intitle = input.InTitle, isbn = input.ISBN, other = input.Other, subject = input.Subject});
            NewBookViewModel viewModel = new NewBookViewModel { Books = ParseSearchResponse(response), ClubId = input.ClubId};
            return View(viewModel);
        }
        [HttpGet]
        public ActionResult NewBookDetails(string id, Guid clubId)
        {
            NewBookViewModel viewModel = new NewBookViewModel();
            viewModel.GoogleVolumeId = id;
            viewModel.ClubId = clubId;
            viewModel.Book = ParseSingleSearchResponse(GoogleBooksAPIHandler.SingleSearch(viewModel.GoogleVolumeId).Result);
            viewModel.Reviews = db.Reviews.Include("Member").Where(r => r.BookId == viewModel.Book.Id).ToList();
            return View(viewModel);
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
            if (response == null)
                return default;
            List<Book> output = new List<Book>();
            foreach (Item item in response.items)
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
            foreach (string s in authors)
            {
                Author author = db.Authors.FirstOrDefault(a => a.Name == s);
                db.BookAuthors.Add(new BookAuthors { Id = Guid.NewGuid(), Author = author, Book = book });
            }
            db.SaveChanges();
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
            if (categories != null)
                for (int i = 0; i < categories.Length; i++)
                {
                    string categoryName = categories[i];
                    if (db.GenreTags.FirstOrDefault(g => g.Name == categoryName) == null)
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
            if (authors != null)
                for (int i = 0; i < authors.Length; i++)
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
        private ClubIndexViewModel GetClubIndexViewModel(BookClub club)
        {
            ClubIndexViewModel viewModel = new ClubIndexViewModel { Club = club, Discussions = new List<Discussion>(), Members = new List<Member>() };
            AddMembersToClubIndexViewModel(viewModel);
            AddDiscussionsToClubIndexViewModel(viewModel);
            AddBooksToClubIndexViewModel(viewModel);
            return viewModel;
        }

        private void AddMembersToClubIndexViewModel(ClubIndexViewModel viewModel)
        {
            var userId = User.Identity.GetUserId();
            List<ClubMembers> clubMembers = db.ClubMembers.Where(cm => cm.ClubId == viewModel.Club.Id).ToList();
            Member member = db.Members.FirstOrDefault(m => m.ApplicationUserId == userId);
            foreach (ClubMembers cm in clubMembers)
            {
                viewModel.Members.Add(db.Members.Find(cm.MemberId));
                if (cm.MemberId == member.Id && cm.IsManager)
                    viewModel.IsManager = true;
            }
        }

        private void AddDiscussionsToClubIndexViewModel(ClubIndexViewModel viewModel)
        {
            viewModel.Discussions = db.Discussions.Where(d => d.ClubId == viewModel.Club.Id).ToList();
            foreach (Discussion discussion in viewModel.Discussions)
            {
                BookDiscussions bookDiscussions = db.BookDiscussions.Include("Book").FirstOrDefault(bd => bd.DiscussionId == discussion.Id);
                discussion.Book = bookDiscussions.Book;
            }
        }

        private void AddBooksToClubIndexViewModel(ClubIndexViewModel viewModel)
        {
            List<Book> books = new List<Book>();
            foreach (Member m in viewModel.Members)
            {
                Collection collection = db.Collections.FirstOrDefault(c => c.MemberId == m.Id);
                if (collection != null)
                {
                    List<CollectionBooks> collectionBooks = db.CollectionBooks.Include("Book").Where(cb => cb.CollectionId == collection.Id).ToList();
                    foreach (CollectionBooks cb in collectionBooks)
                        books.Add(cb.Book);
                }
            }
            var sortedBooks = books.GroupBy(b => b.GoogleVolumeId, (googleId, Books) => new {
                Count = Books.Count(),
                Key = googleId,
                Value = Books.First()
            }).OrderByDescending(g => g.Count);
            viewModel.Books = new List<Book>();
            for (int i = 0; i < sortedBooks.Count(); i++)
            {
                viewModel.Books.Add(sortedBooks.ElementAt(i).Value);
            }
        }
    }
}
