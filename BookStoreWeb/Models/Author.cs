using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Text;
namespace BookStoreWeb.Models
{
    public class Author
    {
        [Key][MaxLength(50)][MinLength(3)][Display(Name ="Author")]
        public string Author_Name { get; set; }
        
        public string Country { get; set; }
        [DataType(DataType.MultilineText)]
        public string Biography { get; set; }
        public List<Book> Books { get; set; }
        public string ImgUrl { get; set; }
       
        public override string ToString()
        {
           StringBuilder sb = new StringBuilder("Author: ");
            return sb.Append(Author_Name).Append("\n").Append("Country: ")
                 .Append(Country).Append("\n").ToString();
            
        }
    }
}