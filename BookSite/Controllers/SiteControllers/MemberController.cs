using BookSite.Models;
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
            if (member != null)
            {
                MemberIndexViewModel viewModel = new MemberIndexViewModel { MemberId = member.Id, Clubs = new List<BookClub>() };
                viewModel.Clubs = GetBookClubs(member);
                viewModel.Friends = GetFriendsList(member);
                return View(viewModel);
            }
            return View();
        }

        // GET: Member/Details/5
        public ActionResult Details(Guid id)
        {
            var userId = User.Identity.GetUserId();
            Member member = db.Members.FirstOrDefault(m => m.ApplicationUserId == userId);
            if (id == member.Id)
                return RedirectToAction("Index");
            MemberDetailsViewModel viewModel = BuildMemberDetailsViewModel(id);
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
            if (collection == null)
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
            List<Member> output = new List<Member>();
            foreach(BookClub club in clubs)
            {
                List<ClubMembers> clubMembers = db.ClubMembers.Where(cm => cm.ClubId == club.Id).ToList();
                foreach (ClubMembers cm in clubMembers)
                    output.Add(db.Members.Find(cm.MemberId));
            }
            return output.Distinct().ToList();
        }

        private List<Member> GetMembersFromBooks(List<Book> books)
        {
            List<Member> output = new List<Member>();
            foreach(Book book in books)
            {
                List<CollectionBooks> collectionBooks = db.CollectionBooks.Include("Collection").Where(cb => cb.BookId == book.Id).ToList();
                foreach(CollectionBooks cb in collectionBooks)
                {
                    output.Add(db.Members.Find(cb.Collection.MemberId));
                }
            }
            return output.Distinct().ToList();
        }

        private List<Member> GetMembersFromAuthors(List<Author> authors)
        {
            List<Book> books = new List<Book>();
            foreach(Author author in authors)
            {
                List<BookAuthors> bookAuthors = db.BookAuthors.Include("Book").Where(ba => ba.AuthorId == author.Id).ToList();
                foreach (BookAuthors ba in bookAuthors)
                    books.Add(ba.Book);
            }
            return GetMembersFromBooks(books.Distinct().ToList());
        }

        private List<Book> GetBookCollection(string userId)
        {
            Member member = db.Members.FirstOrDefault(m => m.ApplicationUserId == userId);
            Collection collection = db.Collections.FirstOrDefault(c => c.MemberId == member.Id);
            List<CollectionBooks> collectionBooks = db.CollectionBooks.Where(cb => cb.CollectionId == collection.Id).ToList();
            List<Book> books = new List<Book>();
            foreach (CollectionBooks cb in collectionBooks)
            {
                Book book = db.Books.Find(cb.BookId);
                books.Add(GetBookWithAuthorAndCategory(book));
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

        private MemberDetailsViewModel BuildMemberDetailsViewModel(Guid id)
        {
            Member member = db.Members.Find(id);
            List<CollectionBooks> collectionBooks = db.CollectionBooks.Include("Collection").Where(cb => cb.Collection.MemberId == member.Id).ToList();
            List<Book> books = new List<Book>();
            foreach (CollectionBooks cb in collectionBooks)
                books.Add(db.Books.Find(cb.BookId));
            List<Review> reviews = db.Reviews.Include("Book").Where(r => r.MemberId == member.Id).ToList();
            MemberDetailsViewModel viewModel = new MemberDetailsViewModel
            {
                Member = member,
                Books = books,
                Reviews = reviews
            };
            return viewModel;
        }

        private List<Member> GetFriendsList(Member member)
        {
            List<Member> friends = new List<Member>();
            FriendList friendList = db.FriendLists.FirstOrDefault(l => l.Id == member.Id);
            if (friendList != null)
            {
                List<FriendPair> pairs = db.FriendPairs.Include("Friend").Where(p => p.ListId == friendList.Id).ToList();
                foreach (FriendPair pair in pairs)
                    friends.Add(pair.Friend);
            }
            return friends;
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
    }
}
