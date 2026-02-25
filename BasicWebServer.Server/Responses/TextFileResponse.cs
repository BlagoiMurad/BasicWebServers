using BasicWebServer.Server.HTTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicWebServer.Server.Responses
{
    public class TextFileResponse : Response
    {
        public string Filename { get; init; }

        public TextFileResponse(string filename)
            : base(StatusCode.OK) 
        {
            Filename = filename;

            Header.Add(Header.ContentType, ContentType.PlainText);
        }


        public override string ToString()
        {
            if (File.Exists(Filename))
            {
                Body = File.ReadAllText(Filename);
            }

            var fileBytesCount = Encoding.UTF8.GetByteCount(Body);

            Header.Add("Content-Length", fileBytesCount.ToString());
            Header.Add("Content-Disposition", $"attachment; filename=\"{Filename}\"");

            return base.ToString();
        }

    }
}
