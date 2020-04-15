using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BookStoreWeb.Models
{
    [Table("Roles")]
    public class RoleMaster
    {
        [Key]
        public int Id { get; set; }
        public string Role { get; set; }

    }
}