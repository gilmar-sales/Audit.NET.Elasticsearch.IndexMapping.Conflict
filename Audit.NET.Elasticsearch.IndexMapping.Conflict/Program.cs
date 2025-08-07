using System.Reflection;
using Audit.Core;
using Audit.Elasticsearch.Providers;
using Audit.EntityFramework;
using Audit.NET.Elasticsearch.IndexMapping.Conflict;
using Elastic.Clients.Elasticsearch;
using Microsoft.EntityFrameworkCore;
using Configuration = Audit.Core.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseInMemoryDatabase(Assembly.GetExecutingAssembly().GetName().FullName)
        .AddInterceptors(new AuditSaveChangesInterceptor());
});

var clientSettings = new ElasticsearchClientSettings(new Uri("http://elasticsearch:9200"));

builder.Services.AddSingleton(clientSettings);

builder.Services.AddScoped<ElasticsearchClient>(serviceProvider =>
    new ElasticsearchClient(serviceProvider.GetRequiredService<ElasticsearchClientSettings>()));

builder.Services.AddScoped<ElasticsearchDataProvider>(serviceProvider =>
    new ElasticsearchDataProvider(serviceProvider.GetRequiredService<ElasticsearchClient>()));

Configuration.Setup()
    .IncludeActivityTrace()
    .UseElasticsearch(config =>
    {
        config
            .Client(clientSettings)
            .Index(auditEvent => "audit-index-mapping-conflict")
            .Id(ev => Guid.CreateVersion7());
    });

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
            BirthDay = DateTime.UtcNow.AddYears(-Random.Shared.Next(10, 20))
        };
        
        await dbContext.Persons.AddAsync(person);
        
        await dbContext.SaveChangesAsync();
        
        person.Name = "Mr Elastic 2";
        person.BirthDay = DateTime.UtcNow.AddYears(-Random.Shared.Next(20, 30));

         dbContext.Update(person);
        
        await dbContext.SaveChangesAsync();
    })
    .WithName("SimulateIssue");

app.Run();
