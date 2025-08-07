using System.Reflection;
using Audit.NET.Elasticsearch.IndexMapping.Conflict;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase(Assembly.GetExecutingAssembly().GetName().FullName));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(static options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", Assembly.GetExecutingAssembly().GetName().Name);
    });
}

app.UseHttpsRedirection();

app.MapPost("/simulate-issue", async (AppDbContext dbContext) =>
    {
        var person = new Person
        {
            Name = "Mr Elastic",
            BirthDate = DateTime.UtcNow.AddYears(-Random.Shared.Next(10, 30))
        };
        
        await dbContext.Persons.AddAsync(person);
        
        await dbContext.SaveChangesAsync();
        
        person.Name = "Mr Elastic 2";
        
        await dbContext.SaveChangesAsync();
    })
    .WithName("SimulateIssue");

app.Run();
