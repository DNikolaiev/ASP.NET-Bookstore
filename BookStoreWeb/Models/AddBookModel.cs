using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookStoreWeb.Models
{
    public class AddBookModel
    {
        public Book book;
        public List<string> languages = new List<string>
        {
            "English", "German", "Russian", "French","Spanish"
        };
        public List<string> genres = new List<string>
        {
            "Fantasy", "Adventure", "Romance", "Sci-Fi", "Dystopian", "Biography","Motivation", "Romance",
            "Horror","Thriller","Historical","Cooking","Art","Professional","Travel","Humor","Children's"
        };

    }
}