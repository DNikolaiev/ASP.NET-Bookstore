using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BookStoreWeb.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        [Required][MaxLength(200)][DataType(DataType.MultilineText)]
        public string Text { get; set; }
        [Required][DataType(DataType.Date)]
        public DateTime Date { get; set; }
        [Range(0,int.MaxValue)]
        public int UpVotes { get; set; } = 0;
        [Range(0, int.MaxValue)]
        public int DownVotes { get; set; } = 0;
        [ForeignKey("User")][Display(Name ="Username")]
        public string UserName { get; set; }
        public User User { get; set; }
        [ForeignKey("Book")]
        public int Book_Id { get; set; }
        public Book Book { get; set; }
        public bool IsUpvotedByUser { get; set; } = false;
        public bool IsDownvotedByUser { get; set; } = false;
    }
}