using BasicWebServer.Server.HTTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicWebServer.Server.Contracts
{
    public interface IRoutingTable
    {
        public IRoutingTable Map(string url, Method method, Response response);


        public IRoutingTable MapGet(string url, Response response);

        public IRoutingTable MapPost(string url, Response response);
    }


}
