using BasicWebServer.Server;
using BasicWebServer.Server.HTTP;
using BasicWebServer.Server.Responses;
using BasicWebServer.Server.Views;
using System.Net.Http;

namespace BasicWebServer.Demo
{
    public class Startup
    {
        private static string Filename = "content.txt";

        private const string LoginForm = @"<form action='/Login' method='POST'>
   Username: <input type='text' name='Username'/>
   Password: <input type='text' name='Password'/>
   <input type='submit' value ='Log In' /> 
</form>";

        private const string ValidUsername = "user";
        private const string ValidPassword = "user123";

        static async Task Main(string[] args)
        {
            await DownloadSitesAsTextFile(Filename, new string[]
            {
                "https://www.aboutyou.com",
                "https://www.google.com",
                "https://www.github.com"
            });

            var server = new HttpServer(routes =>
                routes

                // Forms
                .MapGet("/HTML", new HtmlResponse(formView.HTML))
                .MapPost("/HTML", new TextResponse("", AddFormDataAction))

                // Redirect
                .MapGet("/redirect", new RedirectResponse("https://www.aboutyou.com"))

                // File download
                .MapGet("/content", new HtmlResponse(DownloadForm.Html))
                .MapPost("/content", new TextFileResponse(Filename))

                // Cookies (HTML string, но action през TextResponse)
                .MapGet("/Cookies", new TextResponse("", AddCookiesAction))

                // Session
                .MapGet("/Session", new TextResponse("", DisplaySessionInfoAction))

                // Login
                .MapGet("/Login", new HtmlResponse(LoginForm))
                .MapPost("/Login", new TextResponse("", LoginAction))

                // Logout
                .MapGet("/Logout", new TextResponse("", LogoutAction))

                // Profile
                .MapGet("/UserProfile", new TextResponse("", GetUserDataAction))
            );

            await server.Start();
        }

        // ----------------- FORM -----------------
        private static void AddFormDataAction(Request request, Response response)
        {
            response.Body = "";

            foreach (var (key, value) in request.Form)
            {
                response.Body += $"{key} - {value}{Environment.NewLine}";
            }
        }

        // ----------------- COOKIES -----------------
        private static void AddCookiesAction(Request request, Response response)
        {
            var bodyText = string.Empty;

            bool onlySessionCookie =
                request.Cookies.Count == 1 &&
                request.Cookies.Contains(Session.SessionCookieName);

            if (request.Cookies.Count == 0 || onlySessionCookie)
            {
                bodyText = "<h1>No cookies yet!</h1>";
            }
            else
            {
                bodyText = "<h1>Cookies</h1><ul>";

                foreach (var cookie in request.Cookies)
                {
                    if (cookie.Name == Session.SessionCookieName)
                        continue;

                    bodyText += $"<li>{cookie.Name} = {cookie.Value}</li>";
                }

                bodyText += "</ul>";
            }

            if (!request.Cookies.Contains("My-Cookie"))
            {
                response.Cookies.Add(new BasicWebServer.Server.HTTP.Cookie("My-Cookie", "Hello"));
                response.Cookies.Add(new BasicWebServer.Server.HTTP.Cookie("Another-Cookie", "World"));
            }

            response.Body = bodyText;
        }

        // ----------------- SESSION -----------------
        private static void DisplaySessionInfoAction(Request request, Response response)
        {
            if (!request.Session.ContainsKey(Session.CurrentDateKey))
            {
                response.Body = "Current date stored!";
                return;
            }

            response.Body = $"Stored date: {request.Session[Session.CurrentDateKey]}";
        }

        // ----------------- LOGIN -----------------
        private static void LoginAction(Request request, Response response)
        {
            request.Session.Clear();

            bool usernameOk =
                request.Form.ContainsKey("Username") &&
                request.Form["Username"] == ValidUsername;

            bool passwordOk =
                request.Form.ContainsKey("Password") &&
                request.Form["Password"] == ValidPassword;

            if (usernameOk && passwordOk)
            {
                request.Session[Session.UserKey] = request.Form["Username"];
                response.Body = "<h1>Successfully logged in!</h1>";
            }
            else
            {
                response.Body = "<h1>Invalid username or password.</h1>" + LoginForm;
            }
        }

        // ----------------- LOGOUT -----------------
        private static void LogoutAction(Request request, Response response)
        {
            request.Session.Clear();
            response.Body = "<h1>Logged out.</h1>";
        }

        // ----------------- PROFILE -----------------
        private static void GetUserDataAction(Request request, Response response)
        {
            if (request.Session.ContainsKey(Session.UserKey))
            {
                response.Body = $"<h1>User Profile</h1><p>Username: {request.Session[Session.UserKey]}</p>";
                return;
            }

            response.Body = "<h1>You are not logged in.</h1><a href='/Login'>Go to Login</a>";
        }

        // ----------------- FILE DOWNLOAD -----------------
        private static async Task<string> DownloadWebSiteContent(string url)
        {
            using var client = new HttpClient();

            var response = await client.GetAsync(url);
            var html = await response.Content.ReadAsStringAsync();

            return html.Substring(0, Math.Min(2000, html.Length));
        }

        private static async Task DownloadSitesAsTextFile(string fileName, string[] urls)
        {
            var downloads = urls.Select(DownloadWebSiteContent).ToArray();
            var responses = await Task.WhenAll(downloads);

            var content = string.Join($"{Environment.NewLine}{new string('-', 100)}{Environment.NewLine}", responses);

            await File.WriteAllTextAsync(fileName, content);
        }
    }
}