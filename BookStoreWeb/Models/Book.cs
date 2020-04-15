using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BookStoreWeb.Models
{
    public class Book
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        [MinLength(3)]
        [DataType(DataType.Text)]
        public string Title { get; set; }
        [ForeignKey("Author")]
        [Display(Name ="Author's name")]
        public string Author_Name_fk { get; set; }
        public Author Author { get; set; }
        [Range(1.0,5.0)]
        public int Rating { get; set; }
        public string Genre { get; set; }
        [DataType(DataType.Currency)]
        public float Price { get; set; } = 1;
        [DataType(DataType.Text)]
        public string Language { get; set; }
        [ForeignKey("Publisher")]
        [Display(Name ="Publisher")]
        public string Publisher_Name_fk { get; set; }
        public Publisher Publisher { get; set; }
        [MaxLength(1000)]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        public List<Comment> Comments { get; set; }
        [Range(0,int.MaxValue)]
        public int Sold { get; set; } = 0;
        [DataType(DataType.ImageUrl)]
        public string ImgUrl { get; set; }
        public string fileUrl { get; set; }

       

    }
}