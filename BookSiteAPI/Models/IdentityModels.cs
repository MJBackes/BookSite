using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using BookSite.Models.SiteModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace BookSiteAPI.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Member> Members { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BookClub> BookClubs { get; set; }
        public DbSet<Discussion> Discussions { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<ClubMembers> ClubMembers { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Collection> Collections { get; set; }
        public DbSet<CollectionBooks> CollectionBooks { get; set; }
        public DbSet<BookDiscussions> BookDiscussions { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<BookAuthors> BookAuthors { get; set; }
        public DbSet<GenreTag> GenreTags { get; set; }
        public DbSet<BookTags> BookTags { get; set; }
        public DbSet<FriendList> FriendLists { get; set; }
        public DbSet<FriendPair> FriendPairs { get; set; }
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }
        
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}