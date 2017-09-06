using FC.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Controllers
{
    [Route("api/attendance")]
    public class AttendanceController : Controller
    {
        private readonly AttendanceContext _context;

        public AttendanceController(AttendanceContext context)
        {
            _context = context;

            if (_context.Attendances.Count() == 0)
            {
                NoContent();
                _context.SaveChanges();
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetbyId(int id)
        {
            var item = _context.Attendances.FirstOrDefault(t => t.id == id);
            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        [HttpPost("checkin/{userid}")]
        public IActionResult CheckIn(int userid)
        {
            try
            {
                var item = _context.Attendances.FirstOrDefault(t => t.userid == userid && t.date == DateTime.UtcNow.Date);
                if (item == null)
                {
                    var attendance = new Attendance();
                    attendance.userid = userid;
                    attendance.date = DateTime.UtcNow.Date;
                    attendance.firstcheckin = DateTime.UtcNow;
                    _context.Attendances.Add(attendance);
                    item = attendance;
                }
                else if (item.secondcheckout!= DateTime.MinValue)
                {
                    return new ObjectResult(new Response<Attendance>("AA300", "Limitst exceeded", null));
                }
                else if (item.secondcheckin != DateTime.MinValue)
                {
                    return new ObjectResult(new Response<Attendance>("AA301", "Please check in", null));
                }
                else if(item.firstcheckout != DateTime.MinValue)
                {
                    item.secondcheckin = DateTime.UtcNow;
                    _context.Attendances.Update(item);
                }
                else
                {
                    return new ObjectResult(new Response<Attendance>("AA302", "Check out first to check in", null));
                }

                _context.SaveChanges();
                return new ObjectResult(new Response<Attendance>("AA100", "Checked out succeffully", item));
            }
            catch (Exception ex)
            {
                return new ObjectResult(new Response<Attendance>("AA400", "Error:" + ex.Message, null));
            }
        }

        [HttpPost("checkout/{userid}")]
        public IActionResult CheckOut(int userid)
        {
            try
            {
                var item = _context.Attendances.FirstOrDefault(t => t.userid == userid && t.date == DateTime.UtcNow.Date);
                if (item == null)
                {
                    return new ObjectResult(new Response<Attendance>("AA300", "No check ins found", null));
                }
                else if (item.secondcheckout != DateTime.MinValue)
                {
                    return new ObjectResult(new Response<Attendance>("AB300", "Limits exceeded", null));
                }
                else if (item.secondcheckin != DateTime.MinValue)
                {
                    item.secondcheckout = DateTime.UtcNow;
                    _context.Attendances.Update(item);
                }
                else if(item.firstcheckout!=DateTime.MinValue)
                {
                    return new ObjectResult(new Response<Attendance>("AB301", "Please check in", null));
                }
                else if(item.firstcheckin!=DateTime.MinValue)
                {
                    item.firstcheckout = DateTime.UtcNow;
                    _context.Attendances.Update(item);
                }
                else
                {
                    return new ObjectResult(new Response<Attendance>("AA300", "No check ins found", null));
                }
                _context.SaveChanges();
                return new ObjectResult(new Response<Attendance>("AA100", "Checked in succeffully", item));
            }
            catch (Exception ex)
            {
                return new ObjectResult(new Response<Attendance>("AA400", "Error:" + ex.Message, null));
            }
        }

        [HttpPost]
        public IActionResult NewRecord([FromBody]Attendance attendance)
        {
            if (attendance == null)
            {
                return BadRequest();
            }
            _context.Attendances.Add(attendance);
            _context.SaveChanges();
            return new ObjectResult(_context.Attendances);
            //return CreatedAtRoute("GetUser", new { id = user.id }, user);
        }

    }
}
