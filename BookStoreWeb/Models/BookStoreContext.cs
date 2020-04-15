using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BookStoreWeb.Models
{
    public class BookStoreContext:DbContext
    {
        public BookStoreContext()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<BookStoreContext, Migrations.Configuration>());
        }
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<UserRoleMapping> UserInRole { get; set; }
        public DbSet<RoleMaster> Roles { get; set; }
        public DbSet<UserRatedComment> UserRatedComments { get; set; }
       
    }
}