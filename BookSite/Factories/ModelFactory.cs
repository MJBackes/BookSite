using BookSite.Models.SiteModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookSite.Factories
{
    public class ModelFactory
    {
        public static Collection NewCollection(Member member)
        {
            return new Collection
            {
                Id = Guid.NewGuid(),
                MemberId = member.Id,
                Member = member
            };
        }
        public static CollectionBooks NewCollectionBooks(Collection collection, Book book)
        {
            return new CollectionBooks
            {
                Collection = collection,
                CollectionId = collection.Id,
                BookId = book.Id,
                Book = book,
                Id = Guid.NewGuid()
            };
        }
    }
}