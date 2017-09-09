using FC.Filters;
using FC.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Controllers
{
    [Route("api/attendance")]
   // [ApiAuthenticator("API_KEY")]
    public class AttendanceController : Controller
    {
        private AttendanceContext _context;
        private UserContext _contextUser;

        public AttendanceController(AttendanceContext context, UserContext contextuser)
        {
            _context = context;
            _contextUser = contextuser;

            if (_context.Attendances.Count() == 0)
            {
                NoContent();
                _context.SaveChanges();
            }
        }

        [HttpGet("today/{userid}")]
        public IActionResult GetToday(int userid)
        {
            //var _token = HttpContext.User.FindFirst(t => t.Type == "id");
            //var userid = Convert.ToInt32(_token);
            try
            {
                var item = _context.Attendances.ToList().FirstOrDefault(t => t.userid == userid && t.date == DateTime.UtcNow.Date);
                if (item == null)
                {
                    return new ObjectResult(new Response<Attendance>("AC300", "No attendance found", null));
                }
                return new ObjectResult(new Response<Attendance>("AC200", "Attendance for the user found", item));
            }
            catch (Exception ex)
            {
                return new ObjectResult(new Response<Attendance>("AC400", "Error:" + ex.Message, null));
            }

        }       


        [HttpPost("checkin")]
        [UserAuthenticator("id")]
        public IActionResult CheckIn()
        {
            var _token = HttpContext.User.FindFirst(t => t.Type == "id");
            var userid = Convert.ToInt32(_token.Value);
            try
            {
                var check = _contextUser.Users.FirstOrDefault(t => t.id == userid);
                if (check == null)
                {
                    return new ObjectResult(new Response<Attendance>("AA100", "No user found", null));
                }
                var item = _context.Attendances.FirstOrDefault(t => t.userid == userid && t.date == DateTime.UtcNow.Date);
                if (item == null)
                {
                    var attendance = new Attendance
                    {
                        userid = userid,
                        date = DateTime.UtcNow.Date,
                        firstcheckin = DateTime.UtcNow
                    };
                    _context.Attendances.Add(attendance);
                    item = attendance;
                }
                else if (item.secondcheckout!= DateTime.MinValue)
                {
                    return new ObjectResult(new Response<Attendance>("AA300", "Limits exceeded", null));
                }
                else if (item.secondcheckin != DateTime.MinValue)
                {
                    return new ObjectResult(new Response<Attendance>("AA301", "Please check out", null));
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
                return new ObjectResult(new Response<Attendance>("AA200", "Checked in succeffully", item));
            }
            catch (Exception ex)
            {
                return new ObjectResult(new Response<Attendance>("AA400", "Error:" + ex.Message, null));
            }
        }

        [HttpPost("checkout")]
        [UserAuthenticator("id")]
        public IActionResult CheckOut()
        {
            var _token = HttpContext.User.FindFirst(t => t.Type == "id");
            var userid = Convert.ToInt32(_token.Value);
            try
            {
                var check = _contextUser.Users.FirstOrDefault(t => t.id == userid);
                if (check == null)
                {
                    return new ObjectResult(new Response<Attendance>("AB100", "No user found", null));
                }
                var item = _context.Attendances.FirstOrDefault(t => t.userid == userid && t.date == DateTime.UtcNow.Date);
                if (item == null)
                {
                    return new ObjectResult(new Response<Attendance>("AB302", "No check ins yet", null));
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
                    return new ObjectResult(new Response<Attendance>("AB303", "No check ins found", null));
                }
                _context.SaveChanges();
                return new ObjectResult(new Response<Attendance>("AB200", "Checked out succeffully", item));
            }
            catch (Exception ex)
            {
                return new ObjectResult(new Response<Attendance>("AB400", "Error:" + ex.Message, null));
            }
        }

        [HttpPost]
        [UserAuthenticator("id")]
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
