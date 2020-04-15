using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace BookStoreWeb.Models
{
    public class ChangePasswordModel
    {
        [Required]
        [MinLength(5)]
        [MaxLength(15)]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }
        [Required]
        [MinLength(5)]
        [MaxLength(15)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [Required]
        [MinLength(5)]
        [MaxLength(15)]
        [DataType(DataType.Password)]
        public string RepeatPassword { get; set; }

    }
}