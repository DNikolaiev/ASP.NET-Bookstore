using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookStoreWeb.Models;
using BookStoreWeb.Logic;
using System.Web.Services;
using System.Web.Script.Services;
using System.Web.Razor.Generator;

namespace BookStoreWeb.Controllers
{
    public class ProductController : Controller
    {


        public object Upvote(int id)
        {
            CommentRepository cr = new CommentRepository();
            var rates = cr.UpvoteComment(id, User.Identity.Name);
            var data = new {Upvotes = rates.Item1, Downvotes = rates.Item2 };
            return Json(data);

        }
        public object  Downvote(int id)
        {
            CommentRepository cr = new CommentRepository();
            var rates = cr.DownvoteComment(id, User.Identity.Name);
            var data = new { Upvotes = rates.Item1, Downvotes = rates.Item2 };
            return Json(data);

        }
        [HttpPost]
        public ActionResult DeleteComment(int commentId, int bookId)
        {
            CommentRepository cr = new CommentRepository();
            Comment com = cr.GetComment(commentId);
            if (com == null) return Book(bookId);
            cr.DeleteComment(commentId);
            return RedirectToAction("Book", new { id = bookId });
        }
        public ActionResult Book(int id)
        {
            bool isPurchased = false;
            BookRepository br = new BookRepository();
            Book model = br.GetBook(id, User.Identity.Name);
            if (model == null)
                return RedirectToAction("Products", "Home");
            OrderRepository or = new OrderRepository();
            if (or.GetUserOrders(User.Identity.Name).Where(x => x.Order_BookId == id).Any(x => x.IsPaid))
            {
                isPurchased = true;
            }
            ViewBag.isPurchased = isPurchased;
            return View(model: model);
        }
        [HttpPost]
        public ActionResult Book(string commentText, int id)
        {

            string userName = User.Identity.Name;
            if (userName == null) return Book(id);
            CommentRepository cr = new CommentRepository();
            Comment comment = new Comment
            {
                Text = commentText,
                UserName = userName,
                Book_Id = id,
                Date = DateTime.Now,
                IsDownvotedByUser = false,
                IsUpvotedByUser = false
            };
            cr.AddComment(comment);

            return RedirectToAction("Book", new { id = id });
        }
        [Authorize(Roles = "Admin")]
        public ActionResult Update(int id)
        {
            AddBookModel model = new AddBookModel();
            BookRepository br = new BookRepository();
            model.book = br.GetBook(id);
            ViewBag.Title = "Update Book #" + id;
            return View("Add", model);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Update(HttpPostedFileBase file, HttpPostedFileBase image, int id, string title, string genre,
             int rating, string language,
            string description = "", string author_name = "",
            string country = "", string publisher_name = "", float price = 0)
        {
            if (file == null || image == null || author_name == "" || publisher_name == "" || genre == "" || language == "" || title == "")
            {
                return RedirectToAction("Update", new { id = id });
            }
            if (!file.FileName.Contains(".docx") && !file.FileName.Contains(".doc")
               && !file.FileName.Contains(".pdf") && !file.FileName.Contains(".mobi")
               && !file.FileName.Contains(".rtf"))
            {
                ViewBag.Alert = "Wrong file format (.pdf, .doc, .docx, .rtf, .mobi)";
                return RedirectToAction("Update", new {id=id });
            }
            BookRepository br = new BookRepository();
            Book model = new Book()
            {
                Id = id,
                Title = title,
                Rating = rating,
                Genre = genre,
                Price = price,
                Description = description,
                Language = language,

                Author_Name_fk = author_name,
                Publisher_Name_fk = publisher_name
            };
            if (file != null)
            {
                string uniqueName = Guid.NewGuid().ToString() + file.FileName;
                string fileName = HttpContext.Server.MapPath("~/Static/Files/") + uniqueName;
                file.SaveAs(fileName);
                model.fileUrl = uniqueName;
            }
            string converted_path = "";
            string imgName = "";
            if (image != null)
            {
                DrawRepository draw = new DrawRepository();
                string uniqueName = Guid.NewGuid().ToString() + image.FileName;
                imgName = HttpContext.Server.MapPath("~/Static/Images/") + uniqueName;
                image.SaveAs(imgName);

                uniqueName = uniqueName.Replace(".jpg", ".png");
                System.Drawing.Image converted = System.Drawing.Image.FromFile(imgName);
                converted_path = imgName;
                converted_path = converted_path.Replace(".jpg", ".png");
                converted.Save(converted_path, System.Drawing.Imaging.ImageFormat.Png);

                draw.Resize(converted_path, 1400, 2046);
                model.ImgUrl = uniqueName.Replace(".png", "_" + 1400 + "x" + 2046 + ".png");
            }
            br.UpdateBook(model, model.Id);
            return RedirectToAction("Book", new { id = model.Id });
        }
        [Authorize(Roles = "Admin")]
        public string Delete()
        {
            BookRepository bookRepository = new BookRepository();
            Book book = bookRepository.GetBooks(1)[0];
            bool res = bookRepository.DeleteBook(book.Title, book.Author_Name_fk, book.Language);
            return book.Title + " was deleted: " + res.ToString();
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult Add()
        {
            AddBookModel model = new AddBookModel()
            {
                book = new Book()

            };
            ViewBag.Title = "Add new Book to the Store!";
            
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public object Add(HttpPostedFileBase file, HttpPostedFileBase image, string title, string genre,
            string language, int rating,
            string description, string author_name,
            string country, string publisher_name, float price = 0)
        {
            if (!ModelState.IsValid || file == null) 
            { 
                ViewBag.Alert = "Not valid"; 
                return RedirectToAction("Add");
            }
            if (!file.FileName.Contains(".docx") && !file.FileName.Contains(".doc")
                && !file.FileName.Contains(".pdf") && !file.FileName.Contains(".mobi")
                && !file.FileName.Contains(".rtf"))
            {
                ViewBag.Alert = "Wrong file format (.pdf, .doc, .docx, .rtf, .mobi)";
                return RedirectToAction("Add");
            }
            description = description.Trim();
            Author author = new Author
            {
                Author_Name = author_name,
                Country = country
            };
            Publisher publisher = new Publisher
            {
                Publisher_Name = publisher_name
            };
            Book book = new Book
            {
                Title = title,
                Rating = rating,
                Genre = genre,
                Price = price,
                Description = description,
                Language = language,
                Author = author,
                Author_Name_fk = author_name,
                Publisher_Name_fk = publisher_name,
                Publisher = publisher
            };
            if (file != null)
            {
                string uniqueName = Guid.NewGuid().ToString() + file.FileName;
                string fileName = HttpContext.Server.MapPath("~/Static/Files/") + uniqueName;
                file.SaveAs(fileName);
                book.fileUrl = uniqueName;
            }
            string converted_path = "";
            string imgName = "";
            if (image != null)
            {
                DrawRepository draw = new DrawRepository();
                string uniqueName = Guid.NewGuid().ToString() + image.FileName;
                imgName = HttpContext.Server.MapPath("~/Static/Images/") + uniqueName;
                image.SaveAs(imgName);

                uniqueName = uniqueName.Replace(".jpg", ".png");
                System.Drawing.Image converted = System.Drawing.Image.FromFile(imgName);
                converted_path = imgName;
                converted_path = converted_path.Replace(".jpg", ".png");
                converted.Save(converted_path, System.Drawing.Imaging.ImageFormat.Png);

                draw.Resize(converted_path, 1400, 2046);
                book.ImgUrl = uniqueName.Replace(".png", "_" + 1400 + "x" + 2046 + ".png");
            }
            BookRepository bookRepository = new BookRepository();

            try
            {
                string result = bookRepository.AddBook(book);
                if (result == "Added")
                {

                    return RedirectToAction("Products","Home");
                }
                else
                {
                    return "Book exists already";
                }
            }
            catch (Exception)
            {

                return RedirectToAction("Add");
            }

        }
    
}
}