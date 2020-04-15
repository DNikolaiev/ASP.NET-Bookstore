using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BookStoreWeb.Models;
namespace BookStoreWeb.Logic
{
    public class UserRepository
    {
        public bool Register(User user, out string status)
        {
            using (var db = new BookStoreContext())
            {
                try
                {
                    if (db.Users.Any(u => u.Email.ToLower() == user.Email.ToLower()
                    || u.UserName.ToLower() == user.UserName.ToLower()))
                    {
                        status = "Exists";
                        return false; // user already exists
                    }
                    db.Users.Add(user);
                    int user_role_id = db.Roles.Where(r => r.Role == "User").Select(x => x.Id).FirstOrDefault();
                    db.UserInRole.Add(new UserRoleMapping { RoleId = user_role_id, UserName = user.UserName });
                    db.SaveChanges();
                    status = "Ok";
                    return true;
                }
                catch (Exception) { status = "Invalid"; return false; }
                  
                
                
                
            }
        }
        public bool Login(string email, string password, out string user_name)
        {
            using (var db= new BookStoreContext() )
            {
               
                if (db.Users.Any(u=>u.Email.ToLower()==email.ToLower()
                && u.Password==password))
                {
                    user_name = db.Users.Where(u => u.Email == email && u.Password == password).ToList().FirstOrDefault().UserName;
                    return true;
                }
                user_name = string.Empty;
                return false;
            }
        }
        public List<Comment> GetComments(string username)
        {
            using (var db = new BookStoreContext())
            {
                var comments_query =
                    from c in db.Comments
                    join u in db.Users
                    on c.UserName equals u.UserName
                    where u.UserName==username
                    select new { Comment = c };
                List<Comment> comments = new List<Comment>();
                foreach (var comment in comments_query)
                {
                    comments.Add(comment.Comment);
                }
                return comments;
            }
        }
        
        public bool UpdateUser(User user)
        {
            if (user == null) return false;
            using (var db = new BookStoreContext())
            {
                User current_user = db.Users.Find(user.UserName);
                
                current_user.Email = user.Email;
                current_user.Age = user.Age;
                current_user.Country = user.Country;
                db.Entry(current_user).CurrentValues.SetValues(current_user);
                db.SaveChanges();
                return true;
            }
        }
        public User GetUser(string username)
        {
            using (var db = new BookStoreContext())
            {
                var entity = db.Users.Find(username);
                if (entity == null) return null ;
                entity.Comments = GetComments(username)??new List<Comment> { };
                return entity;
            }
        }
        public List<Book> GetUserBooks(string username)
        {
            using (var db = new BookStoreContext())
            {
                List<Book> books = new List<Book>();
                var order_query =
                    from o in db.Orders
                    join b in db.Books
                    on o.Order_BookId equals b.Id
                    join u in db.Users
                    on o.Order_UserName equals u.UserName
                    where u.UserName==username
                    select new { Book = b };
                foreach(var row in order_query)
                {
                    books.Add(row.Book);
                }
                return books ?? null;
            }
        }
       
    }
}