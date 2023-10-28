using System.Text;
using Microsoft.Extensions.FileProviders;
using Notes.Business.Configurations;

namespace Notes.Website.Middleware
{
    public static class StaticPageMiddleware
    {
        public static Delegate Compile(string filePath, NotesConfig config)
        {
            var pagePath = Path.Combine("/pages/", filePath);

            Dictionary<string, string[]> cspDict = new()
            {
                { "default-src", new []{"'none'"} },
                { "img-src", new []{ "'self'" } },
                { "font-src", new []{ "'self'" } },
                { "style-src-elem", new []{ "'self'", "'unsafe-inline'" } },
                { "script-src-elem", new []{ "'self' ", "https://cdn.ckeditor.com" } }
            };


            var cspBuilder = new StringBuilder();
            foreach (var (directive, policies) in cspDict)
            {
                cspBuilder.Append($"{directive} {string.Join(' ', policies)};");
            }

            var cspHeader = cspBuilder.ToString();


            return (HttpContext context, IWebHostEnvironment env) =>
                MW_Delegate(context, env, pagePath, cspHeader);
        }

        private static async Task MW_Delegate(
            HttpContext context, 
            IWebHostEnvironment env, 
            string pagePath, 
            string cspHeader
        )
        {
            context.Response.OnStarting(AddCspHeaders(context, cspHeader));

            context.Response.ContentType = "text/html";
            context.Response.StatusCode = 200;

            await context.Response.SendFileAsync(env.ContentRootFileProvider.GetFileInfo(pagePath));
        }

        private static Func<Task> AddCspHeaders(HttpContext context, string cspHeader)
        {
            return () =>
            {
                context.Response.Headers.Add("Content-Security-Policy", cspHeader);

                return Task.CompletedTask;
            };
        }
    }
}
