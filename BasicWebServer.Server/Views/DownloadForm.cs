using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicWebServer.Server.Views
{
    public class DownloadForm
    {
        public static string Html = @"<form action='/content' method='POST'>
   <input type='submit' value='Download Sites Content' /> 
</form>";

    }

}
