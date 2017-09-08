using FC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using FC.Helpers;
using System.Threading.Tasks;
using FC.Filters;

namespace FC.Controllers
{

    [Route("api/user")]
   // [ApiAuthenticator("API_KEY")]
    public class UserController : Controller
    {
        private readonly UserContext _context;
        private IConfigurationRoot _config;

        public UserController(UserContext context, IConfigurationRoot config)
        {
            _context = context;
            _config = config;
          
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
            try
            {
                //Adding new user to database
                _context.Users.Add(user);
                _context.SaveChanges();
                return new ObjectResult(new Response<User>("UA200", "User created successfully", user));
            }
            catch(Exception ex)
            {
                return new ObjectResult(new Response<Attendance>("UA400", "Error:" + ex.Message, null));
            }
            //  return CreatedAtRoute("GetUser", new { id = user.id }, user);
        }

        [HttpGet]
        //[ApiAuthenticator("API_KEY")]
        public IActionResult GetAll()
        {
            return new ObjectResult(new Response<List<User>>("UB200", "All users are loaded", _context.Users.ToList()));
            //return _context.Users.ToList();
        }

        [Route("GetApiToken")]
        public string Haha()
        {
            return "ApiKey "+CheckClaim.GetApiToken(_config);
        }

        [HttpPut("authenticate")]
        public IActionResult Login([FromBody] User user)
        {
            if (user.username == null || user.password == null)
            {
                return new ObjectResult(new Response<User>("UC100", "Insuficient data", null,null));
            }
            try
            {
                //validating credentials
                var item = _context.Users.FirstOrDefault(t => t.username == user.username && t.password == user.password);
                if (item == null)
                {
                    return new ObjectResult(new Response<User>("UC300", "User not found", null));
                }
                //creating token for the user
                var token = CheckClaim.GetToken(item, _config);
                return new ObjectResult(new Response<User>("UC200", "User found", item, token));
            }
           catch(Exception ex)
            {
                return new ObjectResult(new Response<Attendance>("UC400", "Error:" + ex.Message, null));
            }
        }

        [HttpPut("update")]
        [UserAuthenticator("id")]
        public IActionResult Update([FromBody]User user)
        {
            if (user == null)
            {
                return new ObjectResult(new Response<User>("UD100", "Insufficient data", null));
            }
            //getting claim from token
            var _token = HttpContext.User.FindFirst(t => t.Type == "id");
            var tokenid = Convert.ToInt32(_token.Value);
            try
            {
                //checking validity of claim from database
                var gotuser = _context.Users.FirstOrDefault(t => t.id == tokenid);
                if (gotuser == null)
                {
                    return new ObjectResult(new Response<User>("UD400", "Invalid user", null));
                }
                else
                {
                    if(user.name!=null)
                    gotuser.name = user.name;
                    if(user.username!=null)
                    gotuser.username = user.username;
                    if(user.password!=null)
                    gotuser.password = user.password;

                    //update into database
                    _context.Users.Update(gotuser);
                    _context.SaveChanges();
                    return new ObjectResult(new Response<User>("UD200", "Successfully updated", gotuser));
                }
            }catch(Exception ex)
            {
                return new ObjectResult(new Response<Attendance>("UD400", "Error:" + ex.Message, null));
            }
        }

        [HttpDelete]
        [UserAuthenticator("id")]
        public IActionResult Delete()
        {
            var _token = HttpContext.User.FindFirst(t => t.Type == "id");
            var tokenid = Convert.ToInt32(_token.Value);
            try
            {
                //getting claim from token
                var gotuser = _context.Users.FirstOrDefault(t => t.id == tokenid);
                if (gotuser == null)
                {
                    return new ObjectResult(new Response<User>("UE300", "User not found", null));
                }

                _context.Users.Remove(gotuser);
                _context.SaveChanges();
                return new ObjectResult(new Response<User>("UE200", "Deletion successfull", null));
            }
            catch(Exception ex)
            {
                return new ObjectResult(new Response<Attendance>("UE400", "Error:" + ex.Message, null));
            }
        }
    }
}
