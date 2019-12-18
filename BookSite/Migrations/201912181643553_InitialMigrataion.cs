namespace BookSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialMigrataion : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Authors",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BookClubs",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        PrivacyLevel = c.String(),
                        Description = c.String(),
                        NextBookId = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Books", t => t.NextBookId)
                .Index(t => t.NextBookId);
            
            CreateTable(
                "dbo.Books",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Title = c.String(),
                        ISBN = c.String(),
                        PageCount = c.Int(nullable: false),
                        AuthorId = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Authors", t => t.AuthorId)
                .Index(t => t.AuthorId);
            
            CreateTable(
                "dbo.BookDiscussions",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        BookId = c.Guid(),
                        DiscussionId = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Books", t => t.BookId)
                .ForeignKey("dbo.Discussions", t => t.DiscussionId)
                .Index(t => t.BookId)
                .Index(t => t.DiscussionId);
            
            CreateTable(
                "dbo.Discussions",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        ClubId = c.Guid(nullable: false),
                        StartTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BookClubs", t => t.ClubId, cascadeDelete: true)
                .Index(t => t.ClubId);
            
            CreateTable(
                "dbo.ClubMembers",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ClubId = c.Guid(nullable: false),
                        MemberId = c.Guid(nullable: false),
                        IsManager = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BookClubs", t => t.ClubId, cascadeDelete: true)
                .ForeignKey("dbo.Members", t => t.MemberId, cascadeDelete: true)
                .Index(t => t.ClubId)
                .Index(t => t.MemberId);
            
            CreateTable(
                "dbo.Members",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        FirstName = c.String(),
                        LastNme = c.String(),
                        DisplayName = c.String(),
                        UserImage = c.String(),
                        ApplicationUserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId)
                .Index(t => t.ApplicationUserId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.CollectionBooks",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        CollectionId = c.Guid(nullable: false),
                        BookId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Books", t => t.BookId, cascadeDelete: true)
                .ForeignKey("dbo.Collections", t => t.CollectionId, cascadeDelete: true)
                .Index(t => t.CollectionId)
                .Index(t => t.BookId);
            
            CreateTable(
                "dbo.Collections",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        MemberId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Members", t => t.MemberId, cascadeDelete: true)
                .Index(t => t.MemberId);
            
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        DiscussionId = c.Guid(nullable: false),
                        BookId = c.Guid(nullable: false),
                        MemberId = c.Guid(nullable: false),
                        Title = c.String(),
                        Body = c.String(),
                        TimeOfPost = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Books", t => t.BookId, cascadeDelete: true)
                .ForeignKey("dbo.Discussions", t => t.DiscussionId, cascadeDelete: true)
                .ForeignKey("dbo.Members", t => t.MemberId, cascadeDelete: true)
                .Index(t => t.DiscussionId)
                .Index(t => t.BookId)
                .Index(t => t.MemberId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Comments", "MemberId", "dbo.Members");
            DropForeignKey("dbo.Comments", "DiscussionId", "dbo.Discussions");
            DropForeignKey("dbo.Comments", "BookId", "dbo.Books");
            DropForeignKey("dbo.CollectionBooks", "CollectionId", "dbo.Collections");
            DropForeignKey("dbo.Collections", "MemberId", "dbo.Members");
            DropForeignKey("dbo.CollectionBooks", "BookId", "dbo.Books");
            DropForeignKey("dbo.ClubMembers", "MemberId", "dbo.Members");
            DropForeignKey("dbo.Members", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ClubMembers", "ClubId", "dbo.BookClubs");
            DropForeignKey("dbo.BookDiscussions", "DiscussionId", "dbo.Discussions");
            DropForeignKey("dbo.Discussions", "ClubId", "dbo.BookClubs");
            DropForeignKey("dbo.BookDiscussions", "BookId", "dbo.Books");
            DropForeignKey("dbo.BookClubs", "NextBookId", "dbo.Books");
            DropForeignKey("dbo.Books", "AuthorId", "dbo.Authors");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Comments", new[] { "MemberId" });
            DropIndex("dbo.Comments", new[] { "BookId" });
            DropIndex("dbo.Comments", new[] { "DiscussionId" });
            DropIndex("dbo.Collections", new[] { "MemberId" });
            DropIndex("dbo.CollectionBooks", new[] { "BookId" });
            DropIndex("dbo.CollectionBooks", new[] { "CollectionId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Members", new[] { "ApplicationUserId" });
            DropIndex("dbo.ClubMembers", new[] { "MemberId" });
            DropIndex("dbo.ClubMembers", new[] { "ClubId" });
            DropIndex("dbo.Discussions", new[] { "ClubId" });
            DropIndex("dbo.BookDiscussions", new[] { "DiscussionId" });
            DropIndex("dbo.BookDiscussions", new[] { "BookId" });
            DropIndex("dbo.Books", new[] { "AuthorId" });
            DropIndex("dbo.BookClubs", new[] { "NextBookId" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Comments");
            DropTable("dbo.Collections");
            DropTable("dbo.CollectionBooks");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Members");
            DropTable("dbo.ClubMembers");
            DropTable("dbo.Discussions");
            DropTable("dbo.BookDiscussions");
            DropTable("dbo.Books");
            DropTable("dbo.BookClubs");
            DropTable("dbo.Authors");
        }
    }
}
