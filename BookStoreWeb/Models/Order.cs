using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BookStoreWeb.Models
{
    public class Order
    {
        [Key][DatabaseGenerated(DatabaseGeneratedOption.Identity)] public int Order_Id { get; set; }
        [ForeignKey("User")] public string Order_UserName { get; set; }
        public User User { get; set; }
        [ForeignKey("Book")] public int Order_BookId { get; set; }
        public Book Book { get; set; }
        public bool IsPaid { get; set; } = false;
        public float Price { get; set; } = 0;
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
    }
}