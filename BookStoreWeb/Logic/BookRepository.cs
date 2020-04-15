using System;
using System.Collections.Generic;
using System.Linq;
using BookStoreWeb.Models;
using System.Data.Entity;
namespace BookStoreWeb.Logic
{
    public class BookRepository
    {
        public List<Book> GetPurchasedBooks(string username)
        {
            OrderRepository or = new OrderRepository();
            List<Order> complete_orders = or.GetUserOrders(username).Where(x => x.IsPaid).ToList();
            List<Book> library = new List<Book>();
            foreach (Order order in complete_orders)
            {
                if(order.Book!=null)
                    library.Add(order.Book);
            }
            return library;
        }
        public List<Book> Search(string search_string)
        {
            if (search_string == string.Empty || search_string == "" || search_string == null) return null;
            List<Book> books = new List<Book>();
            string[] words = search_string.Split(' ');
            var dbString =
                   "Select * from Books where Title LIKE '%"+words[0]+"%'";
            foreach (string s in words)
            {
                dbString += "OR Title LIKE '%" + s + "%'";
            }

            using (var db = new BookStoreContext())
            {
                books = db.Books.SqlQuery(dbString).ToList<Book>();
            }
            return books;

        }
       
       
        public List<Book> FilterQuery(int price, List<string> Genres, 
            List<string> Countries, List<string> Languages)
        {
            List<Book> books = new List<Book>();
            string query = "SELECT * from Books JOIN Authors ON Books.Author_Name_fk = Authors.Author_Name WHERE Books.Price <" + price;
            
            foreach(string genre in Genres)
            {
                if (genre == Genres.First())
                    query += " AND Books.Genre = " + "'"+genre+"'";
                else
                    query += " OR Books.Genre = " + "'" + genre + "'";
            }
            foreach (string country in Countries)
            {
                if (country == Countries.First())
                    query += " AND Authors.Country = " + "'"+country+"'";
                else
                    query += " OR Authors.Country = " + "'"+country+"'";
            }
            foreach (string language in Languages)
            {
                if (language == Languages.First())
                    query += " AND Books.Language = " + "'"+language + "'";
                else
                    query += " OR Books.Language = " + "'"+language+ "'";
            }
            using (var db = new BookStoreContext())
            {
                books = db.Books.SqlQuery(query).ToList<Book>();
            }
                //execute query
                //add results to books
                return books;
        }
        public Book GetBook(int id, string username=null)
        {
            CommentRepository cr = new CommentRepository();
            using (var db = new BookStoreContext())
            {
                var book = db.Books.Find(id);
             
                var info_query =
                    from b in db.Books
                    join a in db.Authors
                    on b.Author_Name_fk equals a.Author_Name
                    join p in db.Publishers
                    on b.Publisher_Name_fk equals p.Publisher_Name

                    join c in db.Comments
                    on b.Id equals c.Book_Id
                    where b.Id == id
                    select new { Book = b, Publisher = p, Author = a, Comment = c ?? null };

                foreach (var elem in info_query)
                {
                   
                    book.Author = new Author()
                    {
                        Author_Name = elem.Author.Author_Name,
                        Biography = elem.Author.Biography,
                        Country = elem.Author.Country
                    };
                    book.Publisher = new Publisher()
                    {
                        Publisher_Name = elem.Publisher.Publisher_Name,
                    };

                    if (elem.Comment != null)
                    {
                        foreach(var c in book.Comments)
                        {
                            c.IsDownvotedByUser = cr.IsCommentDownvoted(c.Id, username);
                            c.IsUpvotedByUser = cr.IsCommentUpvoted(c.Id, username);
                        }
                        
                    }
                   
                }
                return book;
            } 
            } //TODO: 
      
        public bool UpdateBook(Book book, int id)
        {
            if (book == null) return false;
            using (var db = new BookStoreContext()) 
            {
                
                var entity = db.Books.Find(id);
                if (entity == null)
                {
                    return false;
                }
                db.Entry(entity).CurrentValues.SetValues(book);
                db.SaveChanges();
                return true;
                

            }
        }
        
        public bool DeleteBook(string title, string author_name, string language)
        {
            using (var db = new BookStoreContext())
            {
                try
                {
                    var results = db.Books.Where(b => b.Title == title
                    && b.Author_Name_fk == author_name
                    && b.Language == language).ToList();
                    
                    db.Books.RemoveRange(results);
                    db.SaveChanges();
                    return true;
                }
                catch(Exception)
                {
                    return false;
                }
               
               
            }
        }
        public bool DeleteBook(int id)
        {
            using (var db = new BookStoreContext())
            {
                try
                {
                    var result = db.Books.Where(b => b.Id == id).FirstOrDefault();
                    if (result != null)
                    {
                        db.Books.Remove(result);
                        db.SaveChanges();
                        return true;
                    }
                    else throw new Exception { };
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
       
        public string AddBook(Book book)
        {
            using (var db = new BookStoreContext())
            {
                // check if a book already exists in DB
                if(db.Books.Any(
                b=>b.Author.Author_Name==book.Author.Author_Name
                && b.Language==book.Language 
                && b.Title==book.Title))
                {
                    return "Exists";
                }
                if (db.Authors.Count(a => a.Author_Name == book.Author.Author_Name) == 0)
                {
                    db.Authors.Add(book.Author);
                }
                if (!db.Publishers.Any(p => p.Publisher_Name == book.Publisher.Publisher_Name))
                {
                    db.Publishers.Add(book.Publisher);
                }
                book.Publisher = null; // in order to avoid insert duplicate error on publisher_name primary key
                book.Author = null; //and author_name primary key
                db.Books.Add(book);
                db.SaveChanges();
                return "Added";
                
               
            }
         
        }
        public List<Book> GetBooks(int range=1)
        {
            List<Book> bookList = new List<Book>();
            using (var db = new BookStoreContext())
            {
                var query = db.Books
                    .Join(db.Authors,
                    book => book.Author_Name_fk, author => author.Author_Name,
                    (book, author) => new { Book = book, Author = author })
                    .Join(db.Publishers,
                    b => b.Book.Publisher_Name_fk, p => p.Publisher_Name,
                    (b, p) => new
                    {
                        Book=b.Book,
                        Publisher = p,
                        Author=b.Author
                    }).OrderByDescending(b=>b.Book.Price).Take(range);
                   
                    
                foreach ( var elem in query)
                {
                    Book b = new Book
                    {
                        Id = elem.Book.Id,
                        Title = elem.Book.Title,
                        Author_Name_fk = elem.Author.Author_Name,
                        Rating = elem.Book.Rating,
                        Price = elem.Book.Price,
                        Publisher_Name_fk = elem.Publisher.Publisher_Name,
                        Description = elem.Book.Description,
                        Sold = elem.Book.Sold,
                        Language = elem.Book.Language,
                        ImgUrl = elem.Book.ImgUrl

                    };
                    Author a = new Author 
                    { 
                        Author_Name = elem.Author.Author_Name,
                        Country=elem.Author.Country
                    };
                    Publisher p = new Publisher { Publisher_Name = elem.Publisher.Publisher_Name };

                    b.Author = a;
                    b.Publisher = p;

                    bookList.Add(b);
                    
                }
                
                
            }
            return bookList;

        }
        public IEnumerable<string> GetGenres()
        {
            using (var db = new BookStoreContext())
            {
                return db.Books.Select(x => x.Genre).ToList().Distinct();
            }
        }
        public IEnumerable<string> GetLanguages()
        {
            using (var db = new BookStoreContext())
            {
                return db.Books.Select(x => x.Language).ToList().Distinct() ;
            }
        }
      
    }
   
}