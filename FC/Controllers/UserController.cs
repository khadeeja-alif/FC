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
                return BadRequest();
            }
            _context.Users.Add(user);
            _context.SaveChanges();
            //return new ObjectResult(_context.Users);
            return CreatedAtRoute("GetUser", new { id = user.id }, user);
        }


        [HttpGet]
        public IEnumerable<User> GetAll()
        {
            return _context.Users.ToList();
        }

        [Route("haha")]
        public string Haha()
        {
            return "Hell";
        }

        [HttpGet("{username}/{password}",Name ="GetUser")]
        public IActionResult GetById(string username,string password)
        {
            var item = _context.Users.FirstOrDefault(t => t.username == username && t.password==password);
            if(item==null)
            {
                // return NotFound();
                return new ObjectResult(item);
            }
            return new ObjectResult(item);
        }
        [HttpPut("{name}")]
        public IActionResult Update([FromBody]User user,string name)
        {
            if(user==null||user.name!=name)
            {
                return BadRequest();
            }

            var gotuser = _context.Users.FirstOrDefault(t => t.name == name);
            if(gotuser==null)
            {
                return NotFound();
            }
            gotuser.name = user.name;
            gotuser.username = user.username;
            gotuser.password = user.password;

            _context.Users.Update(gotuser);
            _context.SaveChanges();
            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var gotuser= _context.Users.First(t => t.id == id);
            if(gotuser==null)
            {
                return NotFound();
            }

            _context.Users.Remove(gotuser);
            _context.SaveChanges();
            return new NoContentResult();
        }
    }
}
