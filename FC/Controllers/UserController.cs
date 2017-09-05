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
                return new ObjectResult(new Response<User>("MB400","Insufficient data",null));
            }
            _context.Users.Add(user);
            _context.SaveChanges();
            //return new ObjectResult(_context.Users);
            return new ObjectResult(new Response<User>("MB100", "Succesfully created", user));
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
                return new ObjectResult(new Response<User>("MA200","User not found",null));
            }
            return new ObjectResult(new Response<User>("MA100","User exists",item));
        }

        [HttpPut("{name}")]
        public IActionResult Update([FromBody]User user,string name)
        {
            if(user==null||user.name!=name)
            {
                return new ObjectResult(new Response<User>("MC200","Invalid Data",null));
            }

            var gotuser = _context.Users.FirstOrDefault(t => t.name == name);
            if(gotuser==null)
            {
                return new ObjectResult(new Response<User>("MC300", "User not found", null));
            }
            gotuser.name = user.name;
            gotuser.username = user.username;
            gotuser.password = user.password;

            _context.Users.Update(gotuser);
            _context.SaveChanges();
            return new ObjectResult(new Response<User>("MC100", "Successfully updated", gotuser));
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var gotuser= _context.Users.First(t => t.id == id);
            if(gotuser==null)
            {
                return new ObjectResult(new Response<User>("MD300", "No user found", null));
            }

            _context.Users.Remove(gotuser);
            _context.SaveChanges();
            return new ObjectResult(new Response<User>("MD100", "Successfully deleted", null));
        }
    }
}
