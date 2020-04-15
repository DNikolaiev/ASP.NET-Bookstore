using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookStoreWeb.Models
{
    public class FilterBooks
    {
        public int Price { get; set; } = 49;
        public IEnumerable<CheckboxItem> Genres { get; set; }
        public IEnumerable<CheckboxItem> Countries { get; set; }
        public IEnumerable<CheckboxItem> Languages { get; set; }
        public FilterBooks(int price, IEnumerable<CheckboxItem> genres,
            IEnumerable<CheckboxItem> countries, IEnumerable<CheckboxItem> languages)
        {
            this.Price = price;
            this.Genres = genres;
            this.Countries = countries;
            this.Languages = languages;
        }
    }
}