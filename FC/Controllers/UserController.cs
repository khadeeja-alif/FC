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
                _context.SaveChanges();
            }
        }

        [HttpGet]
        public IEnumerable<User> GetAll()
        {
            return _context.Users.ToList();
        }

        [HttpGet("{id}",Name ="GetUser")]
        public IActionResult GetById(int id)
        {
            var item = _context.Users.FirstOrDefault(t => t.id == id);
            if(item==null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        [HttpPost]
        public IActionResult Create([FromBody]User user)
        {
            if(user==null)
            {
                return BadRequest();
            }
            _context.Users.Add(user);
            _context.SaveChanges();
         
             return CreatedAtRoute("GetUser", new { id = user.id }, user);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id,[FromBody]User user)
        {
            if(user==null||user.id!=id)
            {
                return BadRequest();
            }

            var gotuser = _context.Users.FirstOrDefault(t => t.id == id);
            if(gotuser==null)
            {
                return NotFound();
            }
            gotuser.name = user.name;
            gotuser.id = user.id;
            gotuser.start = user.start;
            gotuser.stop=user.stop;

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
