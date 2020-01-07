using BookSite.Models;
using BookSite.Models.APIResponseModels;
using BookSite.Models.SiteModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BookSite.Utilities
{
    public class GoogleBookSearchUtilities
    {
        static ApplicationDbContext db = new ApplicationDbContext();
        public static Book ParseSingleSearchResponse(GoogleBookSingleResponse response)
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
        public static Book AddNewSingleBook(GoogleBookSingleResponse response)
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
        public static List<Book> ParseSearchResponse(GoogleBooksSearchResponse response)
        {
            List<Book> output = new List<Book>();
            foreach (Item item in response.items)
            {
                output.Add(ParseSearchItem(item));
            }
            return output;
        }
        public static Book ParseSearchItem(Item item)
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
        public static Book AddNewVolumeBook(Item item)
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
        public static void AddBookAuthorJunctionEntries(Book book, string[] authors)
        {
            foreach (string s in authors)
            {
                Author author = db.Authors.FirstOrDefault(a => a.Name == s);
                db.BookAuthors.Add(new BookAuthors { Id = Guid.NewGuid(), Author = author, Book = book });
            }
            db.SaveChanges();
        }
        public static void AddBookTagJunctionEntries(Book book, string[] categories)
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
        public static string GetCategoryString(string[] categories)
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
        public static string GetAuthorString(string[] authors)
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
    }
}