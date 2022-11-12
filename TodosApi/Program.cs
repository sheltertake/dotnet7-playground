using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.InferSecuritySchemes();
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference{Type= ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[]{}
        }
    });
});
builder.Services.Configure<SwaggerGeneratorOptions>(opts =>
{
    opts.InferSecuritySchemes = true;
});


builder.Services.AddDbContext<TodoDb>(
    opt => opt.UseInMemoryDatabase("Todos"));

builder.Services.AddAuthentication().AddJwtBearer();
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapGet("/", () => "Hello World!");


var todos = app.MapGroup("/todos").RequireAuthorization();

todos.MapGet("/", (TodoDb db) =>
    db.Todos.ToListAsync());

todos.MapPost("/", async (Todo todo, TodoDb db) =>
{
    db.Todos.Add(todo);
    await db.SaveChangesAsync();
    return TypedResults.Created($"/todos/{todo.Id}", todo);
});


app.Run();


class Todo
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool Completed { get; set; }
}
class TodoDb : DbContext
{
    public virtual DbSet<Todo> Todos { get; set; }
    public TodoDb() { }
    public TodoDb(DbContextOptions options) : base(options) { }

}