using System.Globalization;
using System.Text;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace Notes.Website.Middleware
{
    public static class IndexPageMiddleware
    {
        public static Func<HttpContext, RequestDelegate, Task> Compile(IFileProvider webRoot)
        {
            var cspBuilder = new StringBuilder();
            cspBuilder.Append("default-src 'none';");
            cspBuilder.Append("img-src 'self';");
            cspBuilder.Append("font-src 'self';");
            cspBuilder.Append("script-src-elem 'self';");
            cspBuilder.Append("style-src-elem 'self';");
            var cspHeader = cspBuilder.ToString();

            var index = webRoot.GetFileInfo("index.html");

            return (context, next) => MW_Delegate(context, next, cspHeader, index);
        }

        private static async Task MW_Delegate(
            HttpContext context, 
            RequestDelegate next, 
            string cspHeader,
            IFileInfo index
        )
        {
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                await next(context);
                return;
            }

            context.Response.OnStarting(AddCspHeaders(context, cspHeader));

            context.Response.ContentType = "text/html";
            context.Response.StatusCode = 200;

            await using var indexStream = index.CreateReadStream();
            await indexStream.CopyToAsync(context.Response.Body);

            await context.Response.StartAsync();
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
