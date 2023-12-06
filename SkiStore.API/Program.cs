using Microsoft.EntityFrameworkCore;
using NLog.Web;
using SkiStore.API.Context;
using SkiStore.API.Middleware;
using SkiStore.API.Utill;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Auto Mapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);


// NLog Configuration
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.Information);
builder.Logging.AddNLog("NLog.config");


builder.Services.AddDbContext<SkiStoreContext>(options =>
options.UseSqlite("Data source=skistore.db"));


WebApplication app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(opt =>
{
    opt.AllowAnyHeader().AllowAnyMethod().AllowCredentials().AllowCredentials().SetIsOriginAllowed((hosts) => true); //.WithOrigins("http://localhost:3000/")
});



app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Seeding
IServiceScope scope = app.Services.CreateScope();
SkiStoreContext context =scope.ServiceProvider.GetRequiredService<SkiStoreContext>();
ILogger<Program> logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
try
{
    context.Database.Migrate();
    DbInitializer.Initialize(context);  
}
catch (Exception ex) 
{
    logger.LogError(ex, "A prolem occcured during migration");
}

app.Run();
