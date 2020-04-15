using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BookStoreWeb.Models
{
    public enum CommentStatus
    {
        Upvoted, Downvoted,None
    }
    public class UserRatedComment
    {
        [Key][DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("User")]
        public string UserName { get; set; }
        public User User { get; set; }
        [ForeignKey("Comment")]
        public int CommentID { get; set; }
        public Comment Comment { get; set; }
        public string Status { get; set; } = CommentStatus.None.ToString();
    }
}