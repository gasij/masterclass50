using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using TaskBoard.Data;
using TaskBoard.Endpoints;
using TaskBoard.Services;

var builder = WebApplication.CreateBuilder(args);

// База данных: один файл taskboard.db рядом с проектом.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=taskboard.db"));

builder.Services.AddScoped<ITaskService, TaskService>();

// Enum'ы в JSON передаём словами ("InProgress"), а не числами.
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

var app = builder.Build();

// При старте создаём базу, если её ещё нет, и кладём в неё примеры.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    DbSeeder.Seed(db);
}

// Отдаём страницу из папки wwwroot (index.html открывается по адресу "/").
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapTaskEndpoints();

app.Run();
