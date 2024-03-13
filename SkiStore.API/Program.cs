using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NLog.Web;
using SkiStore.API.Context;
using SkiStore.API.Middleware;
using SkiStore.API.Models.SkiStoreDB;
using SkiStore.API.Services.SkiStoreDB;
using SkiStore.API.Utill;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Auto Mapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);

//Services

builder.Services.AddScoped<BasketService>();
builder.Services.AddScoped<ProductService>();


// NLog Configuration
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.Information);
builder.Logging.AddNLog("NLog.config");


builder.Services.AddDbContext<SkiStoreContext>(options =>
options.UseSqlite("Data source=skistore.db"));

builder.Services.AddIdentityCore<User>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<SkiStoreContext>();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

WebApplication app = builder.Build();

app.UsePathBase(new PathString("/api"));

app.UseMiddleware<ExceptionMiddleware>();


app.UseSwagger();
app.UseSwaggerUI();


app.UseCors(opt =>
{
    opt.AllowAnyHeader().AllowAnyMethod().AllowCredentials().SetIsOriginAllowed((hosts) => true); //.WithOrigins("http://localhost:3000/")
});



app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Seeding
IServiceScope scope = app.Services.CreateScope();
SkiStoreContext context = scope.ServiceProvider.GetRequiredService<SkiStoreContext>();
ILogger<Program> logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
UserManager<User> userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
try
{
    await context.Database.MigrateAsync();
    await DbInitializer.Initialize(context,userManager);
}
catch (Exception ex)
{
    logger.LogError(ex, "A prolem occcured during migration");
}

app.Run();
