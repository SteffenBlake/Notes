using System.Text;
using Microsoft.Extensions.FileProviders;
namespace Notes.Website.Middleware
{
    public static class StaticPageMiddleware
    {
        public static RequestDelegate Compile(IFileProvider root, string filePath)
        {
            var pagePath = Path.Combine("/pages/", filePath);
            var page = root.GetFileInfo(pagePath);

            var cspBuilder = new StringBuilder();
            cspBuilder.Append("default-src 'none';");
            cspBuilder.Append("img-src 'self';");
            cspBuilder.Append("font-src 'self';");
            cspBuilder.Append("script-src-elem 'self';");
            cspBuilder.Append("style-src-elem 'self';");
            var cspHeader = cspBuilder.ToString();

            return (context) => MW_Delegate(context, cspHeader, page);
        }

        private static async Task MW_Delegate(
            HttpContext context, 
            string cspHeader,
            IFileInfo page
        )
        {
            context.Response.OnStarting(AddCspHeaders(context, cspHeader));

            context.Response.ContentType = "text/html";
            context.Response.StatusCode = 200;

            await context.Response.SendFileAsync(page);
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
