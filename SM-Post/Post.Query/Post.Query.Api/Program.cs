using Microsoft.EntityFrameworkCore;
using Post.Query.Infrastructure.DataAccess;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
Action<DbContextOptionsBuilder> configureDbContext = options => 
{
    options.UseLazyLoadingProxies().UseSqlServer(builder.Configuration.GetConnectionString("sqlServer"));
};
builder.Services.AddDbContext<DatabaseContext>(configureDbContext);
builder.Services.AddSingleton<DatabaseContextFactory>(new DatabaseContextFactory(configureDbContext));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();

