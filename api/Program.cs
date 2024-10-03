using System.Text;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
        .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
        .AddTransforms(builderContext =>
        {
            builderContext.AddRequestTransform(transformContext =>
            {
                // Include the Authorization header in the request
                var userToken = "Bearer token";
                transformContext.ProxyRequest.Headers.Add("Authorization", userToken);

                // Get the original request URL
                var request = transformContext.HttpContext.Request;
                var originalPath = request.Path;

                // Add the query string to the URL
                var queryToAdd = $"AppCode=123&AuthorCode=456";
                var newQueryString = new StringBuilder();
                newQueryString.Append(queryToAdd);

                var newUrl = $"{originalPath}?{newQueryString}";

                // Update the request URL
                transformContext.ProxyRequest.RequestUri = new Uri(transformContext.DestinationPrefix + newUrl, UriKind.Absolute);
        

                return ValueTask.CompletedTask;
            });
        })        ;
        
var app = builder.Build();

app.MapReverseProxy();
app.MapGet("/", () =>
{
    var html = "<form enctype=\"multipart/form-data\" method=\"post\" action=\"/store\"><input type=\"file\" id=\"myFile\" name=\"file\"><input type=\"submit\"></form>";
    return Results.Content(html, "text/html");
});

app.Run();