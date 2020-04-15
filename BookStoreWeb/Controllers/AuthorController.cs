using BookStoreWeb.Logic;
using BookStoreWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookStoreWeb.Controllers
{
    public class AuthorController : Controller
    {
       public ActionResult Info(string author)
        {
            if (author == null)
            {
                return RedirectToAction("Products","Home");
            }
            AuthorRepository ar = new AuthorRepository();
            Author model = ar.GetAuthor(author);
            return View(model);
        }
        public ActionResult List()
        {
            return View();
        }
        [Authorize(Roles ="Admin")]
        [HttpGet]
        public ActionResult Edit(string author)
        {
            AuthorRepository ar = new AuthorRepository();
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Info", new { author = author });
            }
            return View(ar.GetAuthor(author));
        }
        [HttpPost][Authorize(Roles ="Admin")]
        public ActionResult Edit(HttpPostedFileBase image, Author author, string country)
        {
            author.Country = country;
            AuthorRepository ar = new AuthorRepository();
            if (!User.Identity.IsAuthenticated || author==null || country==null)
            {
                return RedirectToAction("Info", new { author = author });
            }
            string converted_path = "";
            string imgName = "";
            if (image != null)
            {
                DrawRepository draw = new DrawRepository();
                string uniqueName = Guid.NewGuid().ToString() + image.FileName;
                imgName = HttpContext.Server.MapPath("~/Static/Images/Authors/") + uniqueName;
                image.SaveAs(imgName);

                uniqueName = uniqueName.Replace(".jpg", ".png");
                System.Drawing.Image converted = System.Drawing.Image.FromFile(imgName);
                converted_path = imgName;
                converted_path = converted_path.Replace(".jpg", ".png");
                converted.Save(converted_path, System.Drawing.Imaging.ImageFormat.Png);

                draw.Resize(converted_path, 1400, 2046);
                author.ImgUrl = uniqueName.Replace(".png", "_" + 1400 + "x" + 2046 + ".png");
            }
            ar.UpdateAuthor(author);
            
           
            return RedirectToAction("Info", new { author = author.Author_Name });
        }
    }
}