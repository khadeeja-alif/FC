using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Models
{
    public class User
    {
        [Key]
        public int id { get; set; }
        [Required]
        [MaxLength(20)]
        public string name { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        
    }
}
