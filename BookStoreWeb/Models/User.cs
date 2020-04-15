using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace BookStoreWeb.Models
{
    public class User
    {
        [DataType(DataType.Text)][Required][Display(Name ="Username")]
        [Key][MaxLength(50)]
        public string UserName { get; set; }
        [MaxLength(50)][Required][DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required][MinLength(5)][MaxLength(15)][DataType(DataType.Password)]
        public string Password { get; set; }
      
        [Required][Range(1,110, ErrorMessage ="Incorrect age")]
        public int Age { get; set; }
        [DataType(DataType.Text)]
        public string Country { get; set; }
        public List<Comment> Comments { get; set; }

    }
}