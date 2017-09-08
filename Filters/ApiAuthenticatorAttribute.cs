using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Filters
{
    public class ApiAuthenticatorAttribute : Attribute, IResourceFilter
    {
        private string _claim;

        public ApiAuthenticatorAttribute(string claim)
        {
            _claim = claim;
        }
        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            Console.WriteLine("Starting...........................");
            var c = context.HttpContext.User.FindFirst(t => t.Type == _claim);
            if (c == null)
                context.Result = new ContentResult()
                {
                    Content = "API KEY is not valid"
                };
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            Console.WriteLine("Ending...........................");
        }
    }
}
