using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookStoreWeb.Models
{
    public class ProfileModel
    {
        public User User { get; set; }
        public List<Book> Library { get; set; }
        public List<Order> Transactions { get; set; }

    }
}