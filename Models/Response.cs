using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Models
{
    public class Response<T>
    {
        public string status { get; set; }
        public string message { get; set; }
        public T data { get; set; }
        public string token { get; set; }

        public Response(string status, string message, T data)
        {
            this.data = data;
            this.status = status;
            this.message = message;
        }
        public Response(string status, string message, T data, string token)
        {
            this.data = data;
            this.status = status;
            this.message = message;
            this.token = token;
        }
    }
}
