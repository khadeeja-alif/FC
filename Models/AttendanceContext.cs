using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Models
{
    public class AttendanceContext: DbContext
    {
        public AttendanceContext(DbContextOptions<AttendanceContext> options) : base(options)
        {

        }
        public DbSet<Attendance> Attendances { get; set; }
    }
}
