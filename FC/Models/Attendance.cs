using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Models
{
    public class Attendance
    {
        [Key]
        public int id { get; set; }
        public DateTime date { get; set; }
        public DateTime firstcheckin { get; set; }
        public DateTime firstcheckout { get; set; }
        public DateTime secondcheckin { get; set; }
        public DateTime secondcheckout { get; set; }
        public string totalhours { get; set; }
        public int count { get; set; }

    }
}
