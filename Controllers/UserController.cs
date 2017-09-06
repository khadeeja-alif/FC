using FC.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Controllers
{
    [Route("api/user")]
    public class UserController : Controller
    {
        private readonly UserContext _context;

        public UserController(UserContext context)
        {
            _context = context;
          
            if(_context.Users.Count()==0)
            {
                NoContent();
               // var res= NoContent().StatusCode.ToString();
                _context.SaveChanges();
            }
        }


        [HttpPost]
        public IActionResult Create([FromBody]User user)
        {
            if (user == null)
            {
                return new ObjectResult(new Response<User>("UA100", "Insufficient data", null));
            }
            _context.Users.Add(user);
            _context.SaveChanges();
            //return new ObjectResult(_context.Users);
            return new ObjectResult(new Response<User>("UA200", "User created successfully", user));
          //  return CreatedAtRoute("GetUser", new { id = user.id }, user);
        }


        [HttpGet]
        public IActionResult GetAll()
        {
            return new ObjectResult(new Response<List<User>>("UB200", "All users are loaded", _context.Users.ToList()));
            //return _context.Users.ToList();
        }

        [Route("haha")]
        public string Haha()
        {
            return "Hell";
        }

        [HttpPut("authenticate")]
        public IActionResult GetById([FromBody] User user)
        {
            if (user.username == null || user.password == null)
            {
                return new ObjectResult(new Response<User>("UC100", "Insuficient data", null));
            }
            var item = _context.Users.FirstOrDefault(t => t.username == user.username && t.password==user.password);
            if(item==null)
            {
                // return NotFound();
                return new ObjectResult(new Response<User>("UC300", "User not found", null));
            }
            //return new ObjectResult(item);
            return new ObjectResult(new Response<User>("UC200", "User found", item));
        }
        [HttpPut("{name}")]
        public IActionResult Update([FromBody]User user,string name)
        {
            if (user == null)
            {
                //return BadRequest();
                return new ObjectResult(new Response<User>("UD100", "Insufficient data", null));
            }
            else if (user.name != name)
            {
                return new ObjectResult(new Response<User>("UD400", "Invalid data", null));
            }
            else
            {
                var gotuser = _context.Users.FirstOrDefault(t => t.name == name);
                if (gotuser == null)
                {
                    return new ObjectResult(new Response<User>("UD300", "User not found", null));
                }
                gotuser.name = user.name;
                gotuser.username = user.username;
                gotuser.password = user.password;

                _context.Users.Update(gotuser);
                _context.SaveChanges();
                return new ObjectResult(new Response<User>("UD200", "Successfully updated", gotuser));
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var gotuser= _context.Users.First(t => t.id == id);
            if(gotuser==null)
            {
                return new ObjectResult(new Response<User>("UE300", "User not found", null));
            }

            _context.Users.Remove(gotuser);
            _context.SaveChanges();
            return new ObjectResult(new Response<User>("UE200", "Deletion successfull", null));
        }
    }
}
