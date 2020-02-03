using BookSite.APIHandlers;
using BookSite.Factories;
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
using System.Threading.Tasks;
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
            var userId = User.Identity.GetUserId();
            if (userId == null)
                return RedirectToAction("Login", "Account");
            Member member = db.Members.FirstOrDefault(m => m.ApplicationUserId == userId);
            if (member.Id == default)
                return RedirectToAction("Create");
            if (member != null)
            {
                MemberIndexViewModel viewModel = BuildMemberIndexViewModel(member);
                return View(viewModel);
            }
            return View();
        }

        // GET: Member/Details/5
        public ActionResult Details(Guid id)
        {
            var userId = User.Identity.GetUserId();
            Member member = db.Members.FirstOrDefault(m => m.ApplicationUserId == userId);
            if (member != null && id == member.Id)
                return RedirectToAction("Index");
            MemberDetailsViewModel viewModel = BuildMemberDetailsViewModel(id, member);
            return View(viewModel);
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
                if (db.Members.FirstOrDefault(m => m.DisplayName == member.DisplayName) != null)
                {
                    member.NameIsTaken = true;
                    return View(member);
                }
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
        [HttpGet]
        public ActionResult AddBookToCollection(string id)
        {
            return View(Utilities.GoogleBookSearchUtilities.ParseSingleSearchResponse(GoogleBooksAPIHandler.SingleSearch(id).Result));
        }
        [HttpPost]
        public ActionResult AddBookToCollection(Book book)
        {
            var userId = User.Identity.GetUserId();
            if (userId == null)
                return RedirectToAction("Login", "Account");
            Member member = db.Members.FirstOrDefault(m => m.ApplicationUserId == userId);
            Collection collection = db.Collections.FirstOrDefault(c => c.MemberId == member.Id);
            if (collection == null)
            {
                db.Collections.Add(ModelFactory.NewCollection(member));
                db.SaveChanges();
                collection = db.Collections.FirstOrDefault(c => c.MemberId == member.Id);
            }
            Book bookFromDb = db.Books.FirstOrDefault(b => b.GoogleVolumeId == book.GoogleVolumeId);
            db.CollectionBooks.Add(ModelFactory.NewCollectionBooks(collection,bookFromDb));
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
        [HttpGet]
        public ActionResult AddFriend(Guid id)
        {
            var userId = User.Identity.GetUserId();
            Member member = db.Members.FirstOrDefault(m => m.ApplicationUserId == userId);
            FriendList friendList = db.FriendLists.FirstOrDefault(l => l.Id == member.Id);
            if (friendList == null)
            {
                friendList = new FriendList { Member = member };
                db.FriendLists.Add(friendList);
                db.SaveChanges();
            }
            if (db.FriendPairs.FirstOrDefault(p => p.ListId == friendList.Id && p.FriendId == id) != null)
                return RedirectToAction("Details", new { id = id });
            FriendPair pair = new FriendPair
            {
                Friend = db.Members.Find(id),
                List = friendList
            };
            db.FriendPairs.Add(pair);
            db.SaveChanges();
            return RedirectToAction("Details", new { id = id });
        }
        [HttpGet]
        public ActionResult RemoveFriend(Guid id)
        {
            RemoveFriendViewModel viewModel = new RemoveFriendViewModel
            {
                Friend = db.Members.Find(id)
            };
            return View(viewModel);
        }
        [HttpPost]
        public ActionResult RemoveFriend(RemoveFriendViewModel viewModel)
        {
            var userId = User.Identity.GetUserId();
            Member member = db.Members.FirstOrDefault(m => m.ApplicationUserId == userId);
            db.FriendPairs.Remove(db.FriendPairs.FirstOrDefault(p => p.FriendId == viewModel.Friend.Id && p.ListId == member.Id));
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Search()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Search(MemberSearch search)
        {
            return RedirectToAction("SearchResults", search);
        }
        [HttpGet]
        public ActionResult SearchResults(MemberSearch search)
        {
            return View(GetMembersFromSearchInput(search));
        }




        //private methods

        private List<Member> GetMembersFromSearchInput(MemberSearch search)
        {
            List<Member> members = new List<Member>();
            if (search.MemberString != null && search.MemberString.Length > 0)
            {
                members = db.Members.Where(m => m.DisplayName.Contains(search.MemberString) || search.MemberString.Contains(m.DisplayName)).ToList();
            }
            if (search.ClubString != null && search.ClubString.Length > 0)
            {
                List<BookClub> clubs = db.BookClubs.Where(c => c.Name.Contains(search.ClubString) || search.ClubString.Contains(c.Name)).ToList();
                members = members.Concat(GetMembersFromClubs(clubs)).ToList();
            }
            if (search.BookString != null && search.BookString.Length > 0)
            {
                List<Book> books = db.Books.Where(b => b.Title.Contains(search.BookString) || search.BookString.Contains(b.Title)).ToList();
                members = members.Concat(GetMembersFromBooks(books)).ToList();
            }
            if (search.AuthorString != null && search.AuthorString.Length > 0)
            {
                List<Author> authors = db.Authors.Where(a => a.Name.Contains(search.AuthorString) || search.AuthorString.Contains(a.Name)).ToList();
                members = members.Concat(GetMembersFromAuthors(authors)).ToList();
            }
            return members;
        }

        private List<Member> GetMembersFromClubs(List<BookClub> clubs)
        {
            List<Guid> clubIds = clubs.Select(c => c.Id).ToList();
            return db.ClubMembers.Include("Member")
                                 .Where(cm => clubIds.Contains(cm.ClubId))
                                 .Select(cm => cm.Member)
                                 .Distinct()
                                 .ToList();
        }

        private List<Member> GetMembersFromBooks(List<Book> books)
        {
            List<Guid> bookIds = books.Select(b => b.Id).ToList();
            List<Guid> memberIds = db.CollectionBooks.Include("Collection")
                                                     .Where(cb => bookIds.Contains(cb.BookId))
                                                     .Select(cb => cb.Collection.MemberId)
                                                     .ToList();
            return db.Members.Where(m => memberIds.Contains(m.Id))
                             .Distinct()
                             .ToList();
        }

        private List<Member> GetMembersFromAuthors(List<Author> authors)
        {
            List<Guid> authorIds = authors.Select(a => a.Id).ToList();
            List<Book> books = db.BookAuthors.Include("Book")
                                             .Where(ba => authorIds.Contains(ba.AuthorId))
                                             .Select(ba => ba.Book)
                                             .Distinct()
                                             .ToList();
            return GetMembersFromBooks(books);
        }

        private List<Book> GetBookCollection(string userId)
        {
            Member member = db.Members.FirstOrDefault(m => m.ApplicationUserId == userId);
            Collection collection = db.Collections.FirstOrDefault(c => c.MemberId == member.Id);
            List<Book> books = new List<Book>();
            if (collection != null)
            {
                books = db.CollectionBooks.Include("Book")
                                          .Where(cb => cb.CollectionId == collection.Id)
                                          .Select(cb => cb.Book)
                                          .ToList();
                for(int i = 0; i < books.Count; i++)
                    books[i] = GetBookWithAuthorAndCategory(books[i]);
            }
            return books;
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
            for (int i = 0; i < bookTags.Count; i++)
            {
                GenreTag tag = db.GenreTags.Find(bookTags[i].TagId);
                tags[i] = tag;
            }
            return tags;
        }
        private Book GetBookWithAuthorAndCategory(Book book)
        {
            book.Categories = GetCategoryString(GetCategoryArray(book));
            book.Authors = GetAuthorString(GetAuthorArray(book));
            return book;
        }

        private MemberDetailsViewModel BuildMemberDetailsViewModel(Guid id, Member user)
        {
            Member member = db.Members.Find(id);
            List<Book> books = db.CollectionBooks.Include("Collection").Where(cb => cb.Collection.MemberId == member.Id).Select(cb => cb.Book).ToList();
            List<Review> reviews = db.Reviews.Include("Book").Where(r => r.MemberId == member.Id).ToList();
            MemberDetailsViewModel viewModel = new MemberDetailsViewModel
            {
                Member = member,
                Books = books,
                Reviews = reviews
            };
            if (user != null && db.FriendPairs.FirstOrDefault(p => p.ListId == user.Id && p.FriendId == id) != null)
                viewModel.isFriend = true;
            return viewModel;
        }

        private List<Member> GetFriendsList(Member member)
        {
            List<Member> friends = new List<Member>();
            FriendList friendList = db.FriendLists.FirstOrDefault(l => l.Id == member.Id);
            if (friendList != null)
                friends = db.FriendPairs.Include("Friend").Where(p => p.ListId == friendList.Id).Select(p => p.Friend).ToList();
            return friends;
        }
        private List<Book> GetFriendsBooks(List<Member> friends, List<Book> myBooks)
        {
            List<Guid> friendIds = friends.Select(m => m.Id).ToList();
            List<Guid> myBookIds = myBooks.Select(b => b.Id).ToList();
            return db.CollectionBooks.Include("Collection")
                                                  .Include("Book")
                                                  .Where(cb => friendIds.Contains(cb.Collection.MemberId))
                                                  .Select(cb => cb.Book)
                                                  .GroupBy(b => b.GoogleVolumeId, (googleId, Books) => new {
                                                      Key = googleId,
                                                      Count = Books.Count(),
                                                      Value = Books.FirstOrDefault()
                                                  })
                                                  .OrderByDescending(g => g.Count)
                                                  .Select(g => g.Value)
                                                  .AsEnumerable()
                                                  .SkipWhile(b => myBookIds.Contains(b.Id))
                                                  .Take(5)
                                                  .ToList();
        }
        private string GetAuthorStringForBook(Book book)
        {
            List<Author> authors = db.BookAuthors.Include("Author").Where(ba => ba.BookId == book.Id).Select(ba => ba.Author).ToList();
            return GetAuthorString(authors.ToArray());
        }
        private List<Book> GetRecommendations(Member member)
        {
            List<Book> recommendations = GetRelatedRecommendations(GetShelfBooks(member)).Take(5).ToList();
            foreach (Book book in recommendations)
                book.Authors = GetAuthorStringForBook(book);
            return recommendations;
        }
        private List<Book> GetAuthorBooks(Member member)
        {
            List<Book> books = GetShelfBooks(member);
            List<List<Book>> booksByAuthor = GetBooksByAuthor(GetMostCommonAuthors(books));
            List<Book> output = new List<Book>();
            int index = 0;
            while(index < 5 && index < booksByAuthor.Count)
            {
                int innerIndex = 0;
                while (books.FirstOrDefault(b => b.Id == booksByAuthor[index][innerIndex].Id) != null || booksByAuthor[index][innerIndex].Thumbnail == null)
                {
                    innerIndex++;
                    if (innerIndex > booksByAuthor[index].Count)
                        break;
                }
                if(innerIndex < booksByAuthor[index].Count)
                    output.Add(booksByAuthor[index][innerIndex]);
                index++;
            }
            return output;
        }
        private List<Book> GetRelatedRecommendations(List<Book> shelfBooks)
        {
            List<Book> recomendations = new List<Book>();
            foreach(Book book in shelfBooks)
            {
                recomendations = recomendations.Concat(GetRelatedBooks(book)).ToList();
            }
            recomendations = recomendations.GroupBy(b => b.GoogleVolumeId, (googleId, books) => new
            {
                Key = googleId,
                Count = books.Count(),
                Value = books.FirstOrDefault()
            }).OrderByDescending(g => g.Count)
              .Select(g => g.Value)
              .ToList();
            foreach(Book book in shelfBooks)
            {
                recomendations.Remove(book);
            }
            return recomendations;
        }
        private List<List<Book>> GetBooksByAuthor(List<Author> authors)
        {
            List<List<Book>> AuthorBooks = new List<List<Book>>();
            List<GoogleBooksSearchResponse> responses = new List<GoogleBooksSearchResponse>();
            Parallel.ForEach(authors, (author) =>
            {
                GoogleBooksSearchResponse response = GoogleBooksAPIHandler.FullSearch(new Search { inauthor = author.Name });
                responses.Add(response);
            });
            foreach(GoogleBooksSearchResponse response in responses)
            {
                AuthorBooks.Add(Utilities.GoogleBookSearchUtilities.ParseSearchResponse(response).ToList());
            }
            AuthorBooks = AuthorBooks.OrderByDescending(l => l.Count).ToList();
            return AuthorBooks;
        }
        private List<Book> GetShelfBooks(Member member)
        {
            return db.CollectionBooks.Include("Collection")
                                     .Where(cb => cb.Collection.MemberId == member.Id)
                                     .Select(cb => cb.Book)
                                     .ToList();
        }
        private List<Author> GetMostCommonAuthors(List<Book> books)
        {
            List<Author> authors = new List<Author>();
            foreach(Book book in books)
            {
                authors = authors.Concat(db.BookAuthors.Include("Author")
                                                       .Where(ba => ba.BookId == book.Id)
                                                       .Select(ba => ba.Author))
                                                       .ToList();
            }
            return authors.GroupBy(a => a.Id, (id, Authors) => new
            {
                Key = id,
                Count = Authors.Count(),
                Value = Authors.FirstOrDefault()
            }).Select(g => g.Value).ToList();
        }
        private List<Book> GetRelatedBooks(Book book)
        {
            List<Collection> collections = db.CollectionBooks.Include("Collection")
                                                             .Where(cb => cb.BookId == book.Id)
                                                             .Select(cb => cb.Collection)
                                                             .ToList();
            List<Book> books = new List<Book>();
            foreach (Collection c in collections)
            {
                books = books.Concat(db.CollectionBooks.Include("Book")
                                                       .Where(cb => cb.CollectionId == c.Id && cb.Book.Thumbnail != null)
                                                       .Select(cb => cb.Book))
                                                       .ToList();
            }
            books = books.GroupBy(b => b.GoogleVolumeId, (googleId, Books) => new
            {
                Key = googleId,
                Count = Books.Count(),
                Value = Books.FirstOrDefault()
            }).OrderByDescending(g => g.Count)
                  .Select(g => g.Value)
                  .Take(6)
                  .ToList();
            books.Remove(book);
            return books;
        }
        private List<BookClub> GetBookClubs(Member member)
        {
            List<BookClub> clubs = new List<BookClub>();
            List<ClubMembers> clubMembers = db.ClubMembers.Where(cm => cm.MemberId == member.Id).ToList();
            foreach (ClubMembers cm in clubMembers)
            {
                BookClub club = db.BookClubs.Include("NextBook").FirstOrDefault(c => c.Id == cm.ClubId);
                clubs.Add(club);
            }
            return clubs;
        }
        private MemberIndexViewModel BuildMemberIndexViewModel(Member member)
        {
            MemberIndexViewModel viewModel = new MemberIndexViewModel { MemberId = member.Id };
            viewModel.Clubs = GetBookClubs(member);
            viewModel.Friends = GetFriendsList(member);
            List<Book> books = db.CollectionBooks.Include("Book")
                                                 .Include("Collection")
                                                 .Where(cb => cb.Collection.Member.Id == member.Id)
                                                 .Select(cb => cb.Book)
                                                 .ToList();
            viewModel.FriendsBooks = GetFriendsBooks(viewModel.Friends, books);
            viewModel.Recommendations = GetRecommendations(member);
            viewModel.AuthorBooks = GetAuthorBooks(member);
            return viewModel;
        }
    }
}
