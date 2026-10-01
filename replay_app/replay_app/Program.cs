using Microsoft.EntityFrameworkCore;
using replay_app;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AcademyDbContext>(options =>
{
    options.UseSqlServer(@"Server=DOMBROVSKY;Database=Academy_p41;Integrated Security=True;TrustServerCertificate=True;");
});

var app = builder.Build();

app.Use(async (context, next) =>
{
    HttpRequest request = context.Request;
    Console.WriteLine("=====================================");
    Console.WriteLine("NEW REQUEST");
    Console.WriteLine("=====================================");
    Console.WriteLine($"Method: {request.Method}");
    Console.WriteLine($"Path: {request.Path}");
    Console.WriteLine($"Query: {request.QueryString}");
    Console.WriteLine($"IP: {context.Connection.RemoteIpAddress}");
    Console.WriteLine($"Agent: {request.Headers.UserAgent}");
    await next();
});

app.MapGet("/api/students", async (AcademyDbContext db) =>
{
    var students = await db.Students.ToListAsync();
    return Results.Ok(students);
});

app.MapGet("/api/students/{id:int}", async (int id, AcademyDbContext db) =>
{
    var student = await db.Students.FindAsync(id);
    return student is null ? Results.NotFound() : Results.Ok(student);
});

app.MapPost("/api/students", async (Student input, AcademyDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(input.Name) || string.IsNullOrWhiteSpace(input.Email))
    {
        return Results.BadRequest("Name and Email are required.");
    }

    var student = new Student { Name = input.Name, Email = input.Email };
    db.Students.Add(student);
    await db.SaveChangesAsync();
    return Results.Created($"/api/students/{student.Id}", student);
});

app.MapPut("/api/students/{id:int}", async (int id, Student input, AcademyDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(input.Name) || string.IsNullOrWhiteSpace(input.Email))
    {
        return Results.BadRequest("Name and Email are required.");
    }

    var student = await db.Students.FindAsync(id);
    if (student is null)
    {
        return Results.NotFound();
    }

    student.Name = input.Name;
    student.Email = input.Email;
    await db.SaveChangesAsync();
    return Results.Ok(student);
});

app.MapDelete("/api/students/{id:int}", async (int id, AcademyDbContext db) =>
{
    var student = await db.Students.FindAsync(id);
    if (student is null)
    {
        return Results.NotFound();
    }

    db.Students.Remove(student);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();
