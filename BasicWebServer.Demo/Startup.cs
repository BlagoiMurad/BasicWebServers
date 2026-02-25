using BasicWebServer.Server;
using BasicWebServer.Server.HTTP;
using BasicWebServer.Server.Responses;
using BasicWebServer.Server.Views;

namespace BasicWebServer.Demo
{
    public class Startup
    {
  
            private static string Filename = "content.txt";
        static async Task Main(string[] args)
        {
            await DownloadSitesAsTextFile(Filename, new string[]
            {
                "https://www.aboutyou.com",
                "https://www.google.com",
                "https://www.github.com"
            });
            var server = new HttpServer(routes =>


            routes.MapGet("/HTML", new HtmlResponse(formView.HTML))
            .MapGet("/redirect", new RedirectResponse("https://www.aboutyou.com"))
            .MapPost("/HTML", new TextResponse("", AddFormDataAction))
            .MapGet("/content", new HtmlResponse(DownloadForm.Html))
            .MapPost("/content", new TextFileResponse(Filename))
            );

            await server.Start();
        }
        private static void AddFormDataAction(Request request, Response response)
        {
            response.Body = "";
            foreach (var (key, value) in request.Form)
            {
                response.Body += $"{key} - {value}";
                response.Body += Environment.NewLine;

            }
        }



        private static async Task<string> DownloadWebSiteContent(string url)
        {
            var client = new HttpClient();

            using (client)
            {
                var response = await client.GetAsync(url);
                var html = await response.Content.ReadAsStringAsync();
                return html;
            }
        }

        private static async Task DownloadSitesAsTextFile(string fileName, string[] urls)
        {
            var downloads = new List<Task<string>>();
            foreach (var url in urls)
            {
                downloads.Add(DownloadWebSiteContent(url));
            }

            var responses = await Task.WhenAll(downloads);

            var responsesString = string.Join($"{Environment.NewLine}{new string('-', 100)}", responses);

            await File.WriteAllTextAsync(fileName, responsesString);
        }
    }


}


