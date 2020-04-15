using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookStoreWeb.Logic;
using System.Web.Security;
using BookStoreWeb.Models;
using System.Globalization;
using System.Web.Helpers;
namespace BookStoreWeb.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        [HttpGet]
        public ActionResult Login()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Products", "Home");
            }
            return View();
        }
        [HttpPost]
        public ActionResult Login(string Email, string Password)
        {
            UserRepository ur = new UserRepository();
            bool isValidUser = ur.Login(Email, Password, out string user_name);
            if (isValidUser)
            {
                FormsAuthentication.SetAuthCookie(user_name, true);
                return RedirectToAction("Products", "Home");
            }
            ModelState.AddModelError("", "Wrong User Name or Password");
            return View();
        }
        [HttpGet]
        public ActionResult SignUp()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Products", "Home");
            }
            List<string> countries = new List<string>();
            countries.AddRange(CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                .Select(x => new RegionInfo(x.LCID).EnglishName)
                .Distinct()
                .OrderBy(x => x).ToList());
            SelectList selectList = new SelectList(countries, "Germany");

            ViewBag.Countries = selectList;
            return View();
        }
        [HttpPost] [ValidateAntiForgeryToken]
        public ActionResult SignUp(User model, string SelectedCountry)
        {
            UserRepository ur = new UserRepository();
            model.Country = SelectedCountry;
            AntiForgery.Validate();
            bool isValidated = ur.Register(model, out string status);
            if (isValidated)
                return RedirectToAction("Login");
            else if (status == "Invalid")
                ModelState.AddModelError("", "Not valid");
            else if (status == "Exists")
                ModelState.AddModelError("", "User already exists");
            return View();
        }
        [Authorize]
        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
        [Authorize]
        public new ActionResult Profile()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }

            OrderRepository or = new OrderRepository();
            BookRepository br = new BookRepository();
            UserRepository ur = new UserRepository();
            ProfileModel profile = new ProfileModel()
            {
                Library = br.GetPurchasedBooks(User.Identity.Name),
                Transactions = or.GetUserOrders(User.Identity.Name).Where(x => x.IsPaid).ToList(),
                User = ur.GetUser(User.Identity.Name)
            };
            return View(profile);
        }
        public ActionResult Download(string fileName)
        {
            if (System.IO.File.Exists(Server.MapPath("~/Static/Files/") + fileName))
            {

                // for iphones and ipads, this script can cause problems - especially when trying to view videos, so we will redirect to file if on iphone/ipad
                // if (Request.UserAgent.ToLower().Contains("iphone") || Request.UserAgent.ToLower().Contains("ipad")) { Response.Redirect(filePath + '/' + fileName); }
                Response.Clear();
                Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
                Response.WriteFile(Server.MapPath("~/Static/Files/") + fileName);
                Response.End();
                return RedirectToAction("Profile");
            }
            else
            {
                return View("Error");
            }


        }
        [HttpGet]
        public ActionResult Edit()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }
            OrderRepository or = new OrderRepository();
            BookRepository br = new BookRepository();
            UserRepository ur = new UserRepository();
            ProfileModel profile = new ProfileModel()
            {
                Library = br.GetPurchasedBooks(User.Identity.Name),
                Transactions = or.GetUserOrders(User.Identity.Name).Where(x => x.IsPaid).ToList(),
                User = ur.GetUser(User.Identity.Name)
            };
            return View(profile);
        }
        [HttpPost][Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User user, string country)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }
            OrderRepository or = new OrderRepository();
            BookRepository br = new BookRepository();
            UserRepository ur = new UserRepository();
            ProfileModel profile = new ProfileModel()
            {
                Library = br.GetPurchasedBooks(User.Identity.Name),
                Transactions = or.GetUserOrders(User.Identity.Name).Where(x => x.IsPaid).ToList(),
                User = ur.GetUser(User.Identity.Name)
            };
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Invalid input");
                return View(profile);
            }
            user.Country = country;
            ur.UpdateUser(user);
            return RedirectToAction("Profile");
        }
        [HttpGet][Authorize]
        public ActionResult ChangePassword()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                ModelState.AddModelError("", "Not authenticated");
                return View();
                
            }
            if (model.NewPassword != model.RepeatPassword)
            {
                ModelState.AddModelError("", "Password do not match");
                return View();
            }
            
                ;
            string username = User.Identity.Name;
            UserRepository ur = new UserRepository();
            User user = ur.GetUser(username);
            if (model.OldPassword != user.Password)
            {
                ModelState.AddModelError("", "Old password is not right");
                return View();
            }
            user.Password = model.NewPassword;
            ur.UpdateUser(user);
            return RedirectToAction("Profile");
        }
    }
}