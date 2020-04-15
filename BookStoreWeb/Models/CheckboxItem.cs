using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookStoreWeb.Models
{
    public class CheckboxItem
    {
        public string Name { set; get; }
        public int Id { set; get; }
        public bool IsSelected { set; get; }
    }
}