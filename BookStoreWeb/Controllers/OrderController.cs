using BookStoreWeb.Logic;
using BookStoreWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookStoreWeb.Controllers
{
    public class OrderController : Controller
    {
        // GET: Order
        public ActionResult Checkout()
        {
            OrderRepository or = new OrderRepository();
            List <Order> all_orders = or.GetUserOrders(User.Identity.Name);
            List<Order> not_paid_orders = all_orders.Where(x => x.IsPaid == false).ToList();
            return View(not_paid_orders);
        }
        [HttpPost]
        public ActionResult DeleteOrder(int id)
        {
            OrderRepository or = new OrderRepository();
            or.DeleteOrder(id);
            return RedirectToAction("Checkout");
        }
        public ActionResult AddToCart(int book_id)
        {
            OrderRepository or = new OrderRepository();
            if (!or.IsInCart(book_id, User.Identity.Name))
            {
                or.AddOrder(new Order
                {
                    Order_UserName = User.Identity.Name,
                    Order_BookId = book_id,
                    IsPaid = false,
                    Date=DateTime.Now
                });
            }
            return RedirectToAction("Checkout");
        }
        public ActionResult Pay()
        {
            OrderRepository or = new OrderRepository();
            or.Pay(User.Identity.Name);
            return RedirectToAction("Profile", "Account");
        }
    }
}