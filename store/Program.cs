using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/store", async (IWebHostEnvironment _environment, [FromForm] IFormFile file, [AsParameters] UploadInfo uploadInfo) =>
{
    Console.WriteLine(JsonSerializer.Serialize(uploadInfo));
    if (file == null || file.Length == 0)
    {
        return Results.BadRequest("No file uploaded.");
    }

    // Create the upload directory if it doesn't exist
    var uploadPath = Path.Combine(_environment.WebRootPath, "uploads", uploadInfo.AppCode, uploadInfo.AuthorCode);
    if (!Directory.Exists(uploadPath))
    {
        Directory.CreateDirectory(uploadPath);
    }

    var filePath = Path.Combine(uploadPath, file.FileName);

    // Save the file to the uploads directory
    using (var stream = new FileStream(filePath, FileMode.Create))
    {
        await file.CopyToAsync(stream);
    }

    return Results.Ok(new { FilePath = filePath });
}).DisableAntiforgery();

app.MapGet("/store", () => {
    return Results.Ok(new { Message = "Hello, World!" });
});

app.MapGet("/", () => {
    return Results.Ok(new { Message = "Hello, !" });
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.Run();

