using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BookStoreWeb.Models;
namespace BookStoreWeb.Logic
{
    public class AuthorRepository
    {
        public List<Author> GetAuthors()
        {
            using (var db = new BookStoreContext())
            {
                return db.Authors.Select(x => x).ToList();
            }
        }
        public bool DeleteAuthor(string author_name)
        {
            using (var db = new BookStoreContext())
            {
                try
                {
                    var author_to_delete = db.Authors.Where(a => a.Author_Name == author_name).FirstOrDefault();
                    // delete all associated books
                    var books_to_delete=
                        from b in db.Books
                        join a in db.Authors
                        on b.Author_Name_fk equals a.Author_Name
                        select new { b.Id };
                    foreach(var book in books_to_delete)
                    {
                        var book_in_db = db.Books.Find(book.Id);
                        db.Books.Remove(book_in_db);
                    }
                    //delete author
                    db.Authors.Remove(author_to_delete);
                    db.SaveChanges();
                    return true;
                }
                catch(Exception)
                {
                    return false;
                }
            }
                
            //delete author and all assosiated books
        }
        public bool UpdateAuthor(Author author)
        {
            if (author == null) return false;
            using (var db = new BookStoreContext())
            {
                BookRepository br = new BookRepository();
                var entity = db.Authors.Find(author.Author_Name);
                if (entity == null)
                    return false;
                db.Entry(entity).CurrentValues.SetValues(author);

                db.SaveChanges();
                return true;
            }
        }
        public List<Book> GetAuthorBooks(Author author)
        {
            using (var db = new BookStoreContext())
            {
                var all_author_books =
                from a in db.Authors
                join b in db.Books
                on a.Author_Name equals b.Author_Name_fk
                where a.Author_Name ==author.Author_Name
                select new { Book = b };
                List<Book> books = new List<Book>();
                foreach(var elem in all_author_books)
                {
                    if (elem != null)
                    {
                        books.Add(elem.Book);
                    }
                }
                return books;
            }
        }
        public Author GetAuthor(string author_name)
        {
            using (var db = new BookStoreContext()){
                Author a = db.Authors.Find(author_name);
                if(a!=null)
                    a.Books = GetAuthorBooks(a);
                return a;
            }
        }
        public IEnumerable<string> GetCountries()
        {
            using (var db = new BookStoreContext())
            {
                return db.Authors.Select(x => x.Country).ToList().Distinct() ;
            }
        }

    }
}