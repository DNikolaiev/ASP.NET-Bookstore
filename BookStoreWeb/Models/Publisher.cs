using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace BookStoreWeb.Models
{
    public class Publisher
    {
        [Key][MaxLength(30)][DataType(DataType.Text)][Display(Name ="Publisher name")]
        public string Publisher_Name { get; set; }
        public List<Book> Books { get; set; }
        

    }
}