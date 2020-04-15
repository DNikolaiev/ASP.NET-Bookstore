using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BookStoreWeb.Models;
namespace BookStoreWeb.Logic
{
    public class OrderRepository
    {
        public Order GetOrder(int id)
        {
            using (var db = new BookStoreContext())
            {
                BookRepository br = new BookRepository();
                Order order = db.Orders.Find(id);
                if (order == null) return null;
                order.Book = br.GetBook(order.Order_BookId);
                return order;

            }
        }
        public List<Order> GetUserOrders(string username)
        {
            using (var db = new BookStoreContext())
            {
                List<Order> orders = new List<Order>();
                var query =
                    from o in db.Orders
                    where o.Order_UserName == username
                    select new { Order = o };
                foreach(var row in query)
                {
                    orders.Add(GetOrder(row.Order.Order_Id));
                }
                return orders;
            }
        }
        public bool IsInCart(int book_id, string username)
        {
            List<Order> orders = GetUserOrders(username);
            if (orders.Any(x => x.Order_BookId == book_id))
            {
                return true;
            }
            return false;
        }
        public void AddOrder(Order order)
        {
            using (var db = new BookStoreContext())
            {
                try
                {
                    db.Orders.Add(order);
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    return;
                }
            }
        }
        public void DeleteOrder(int order_id)
        {
            using (var db = new BookStoreContext())
            {

                var query =
                    from o in db.Orders
                    where o.Order_Id == order_id
                    select new { Order = o };
                try
                {
                    foreach (var entity in query)
                    {
                        db.Orders.Remove(entity.Order);
                    }

                    db.SaveChanges();
                }
                catch (Exception) { }
                
                
            }
        }
        public float GetOrdersPrice(string username)
        {
            
            using (var db = new BookStoreContext())
            {
                float sum = 0;
                var query =
                    from b in db.Books
                    join o in db.Orders
                    on b.Id equals o.Order_BookId
                    join u in db.Users
                    on o.Order_UserName equals u.UserName
                    where u.UserName == username
                    select new { Price = b.Price };
                foreach(var row in query)
                {
                    sum += row.Price;
                }
                return sum;
                
            }
            
        }
        public bool Pay(string username)
        {
            using (var db = new BookStoreContext())
            {
                // update IsPaid on all user's orders
                BookRepository br = new BookRepository();
                List<Order> orders = GetUserOrders(username).Where(x=>x.IsPaid==false).ToList();
                try
                {
                    foreach (Order o in orders)
                    {
                        Book book = db.Books.Find(o.Order_BookId);
                        if (book != null)
                        {
                            book.Sold++;
                            db.Entry(book).CurrentValues.SetValues(book);
                        }
                        o.IsPaid = true;
                        o.Price = o.Book.Price;
                        o.Date = DateTime.Now;
                        var entity = db.Orders.Find(o.Order_Id);
                        if (entity == null) return false;

                        db.Entry(entity).CurrentValues.SetValues(o);   
                    }
                    db.SaveChanges();
                    return true;
                }
                catch { return false; }
                
                
            }
        }


    }
}