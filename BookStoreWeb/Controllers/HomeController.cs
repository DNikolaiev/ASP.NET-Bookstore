using BookStoreWeb.Logic;
using BookStoreWeb.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;


namespace BookStoreWeb.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Products()
        {
            BookRepository bookRep = new BookRepository();
            AuthorRepository authorRep = new AuthorRepository();

            List<CheckboxItem> checkBoxesGenre = new List<CheckboxItem>();
            List<CheckboxItem> checkBoxesCountry = new List<CheckboxItem>();
            List<CheckboxItem> checkBoxesLanguage = new List<CheckboxItem>();
            List<string> genres = bookRep.GetGenres().ToList();
            List<string> countries = authorRep.GetCountries().ToList();
            List<string> languages = bookRep.GetLanguages().ToList();
            for (int i = 0; i < genres.Count; i++)
            {
                checkBoxesGenre.Add(new CheckboxItem
                {
                    Id = i,
                    Name = genres[i],
                    IsSelected = false
                });
            }
            for (int i = 0; i < countries.Count; i++)
            {
                checkBoxesCountry.Add(new CheckboxItem
                {
                    Id = i,
                    Name = countries[i],
                    IsSelected = false
                });
            }
            for (int i = 0; i < languages.Count; i++)
            {
                checkBoxesLanguage.Add(new CheckboxItem
                {
                    Id = i,
                    Name = languages[i],
                    IsSelected = false
                });
            }

            FilterBooks fb = new FilterBooks(99, checkBoxesGenre.AsEnumerable<CheckboxItem>(), checkBoxesCountry.AsEnumerable(), checkBoxesLanguage.AsEnumerable());
            var books = bookRep.GetBooks(20);
            //FilterBooks fb = new FilterBooks(99, bookRep.GetGenres(), authorRep.GetCountries(), bookRep.GetLanguages());
            ViewBag.Books = books;
            return View("Index", fb);
        }
        [HttpGet]
        public ActionResult Filter(FilterBooks filter)
        {
            BookRepository br = new BookRepository();
            AuthorRepository ar = new AuthorRepository();
            List<string> checkedGenres = new List<string>();
            List<string> checkedCountries = new List<string>();
            List<string> checkedLanguages = new List<string>();
            List<string> allGenres = br.GetGenres().ToList();
            List<string> allCountries = ar.GetCountries().ToList();
            List<string> allLanguages = br.GetLanguages().ToList();
            for (int i = 0; i < filter.Genres.Count(); i++)
            {
                filter.Genres.ToList()[i].Name = allGenres[i];
                if (filter.Genres.ToList()[i].IsSelected)
                {
                    checkedGenres.Add(allGenres[i]);
                }
            }
            for (int i = 0; i < filter.Countries.Count(); i++)
            {
                filter.Countries.ToList()[i].Name = allCountries[i];
                if (filter.Countries.ToList()[i].IsSelected)
                {
                    checkedCountries.Add(allCountries[i]);
                }
            }
            for (int i = 0; i < filter.Languages.Count(); i++)
            {
                filter.Languages.ToList()[i].Name = allLanguages[i];
                if (filter.Languages.ToList()[i].IsSelected)
                {
                    checkedLanguages.Add(allLanguages[i]);
                }
            }
            var books = br.FilterQuery(filter.Price, checkedGenres, checkedCountries, checkedLanguages);
            ViewBag.Books = books;
            
            return View("Index", filter);
        }
        [HttpPost]
        public ActionResult Products(int price, List<CheckboxItem> Genres, List<CheckboxItem> Countries, List<CheckboxItem> Languages)
        {
            return Filter(new FilterBooks(price, Genres, Countries, Languages));
        }
        public ActionResult Search(string text)
        {
            if(text==null || text == string.Empty)
            {
                return RedirectToAction("Products");
            }
            BookRepository br = new BookRepository();
            List<Book> books = br.Search(text);

            AuthorRepository authorRep = new AuthorRepository();

            List<CheckboxItem> checkBoxesGenre = new List<CheckboxItem>();
            List<CheckboxItem> checkBoxesCountry = new List<CheckboxItem>();
            List<CheckboxItem> checkBoxesLanguage = new List<CheckboxItem>();
            List<string> genres = br.GetGenres().ToList();
            List<string> countries = authorRep.GetCountries().ToList();
            List<string> languages = br.GetLanguages().ToList();
            for (int i = 0; i < genres.Count; i++)
            {
                checkBoxesGenre.Add(new CheckboxItem
                {
                    Id = i,
                    Name = genres[i],
                    IsSelected = false
                });
            }
            for (int i = 0; i < countries.Count; i++)
            {
                checkBoxesCountry.Add(new CheckboxItem
                {
                    Id = i,
                    Name = countries[i],
                    IsSelected = false
                });
            }
            for (int i = 0; i < languages.Count; i++)
            {
                checkBoxesLanguage.Add(new CheckboxItem
                {
                    Id = i,
                    Name = languages[i],
                    IsSelected = false
                });
            }

            FilterBooks fb = new FilterBooks(99, checkBoxesGenre.AsEnumerable<CheckboxItem>(), checkBoxesCountry.AsEnumerable(), checkBoxesLanguage.AsEnumerable());

            //FilterBooks fb = new FilterBooks(99, bookRep.GetGenres(), authorRep.GetCountries(), bookRep.GetLanguages());
            ViewBag.Books = books;
            ViewBag.Title = "Search Results: '" + text + "'";
            return View("Index", fb);
        }


    }
}