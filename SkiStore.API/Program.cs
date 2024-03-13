using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog.Web;
using SkiStore.API;
using SkiStore.API.Context;
using SkiStore.API.Middleware;
using SkiStore.API.Models.SkiStoreDB;
using SkiStore.API.Services.SkiStoreDB;
using SkiStore.API.Utill;
using System.Text;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    OpenApiSecurityScheme jwtSecurityScheme = new()
    {
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Description = "Put Bearer + your token in the box bellow",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement { { jwtSecurityScheme, Array.Empty<string>() } });
});


builder.Services.AddAutoMapper(typeof(Program).Assembly);


builder.Services.AddScoped<BasketService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<AccountService>();


builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.Information);
builder.Logging.AddNLog("NLog.config");


builder.Services.AddDbContext<SkiStoreContext>(options =>
options.UseSqlite("Data source=skistore.db"));

builder.Services.AddIdentityCore<User>(opt =>
{
    opt.User.RequireUniqueEmail = true;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<SkiStoreContext>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Settings.TokenKey))
        };
    });
builder.Services.AddAuthorization();

WebApplication app = builder.Build();

app.UsePathBase(new PathString("/api"));

app.UseMiddleware<ExceptionMiddleware>();


app.UseSwagger();
app.UseSwaggerUI(c => { c.ConfigObject.AdditionalItems.Add("persistAuthorization", "true"); });


app.UseCors(opt =>
{
    opt.AllowAnyHeader().AllowAnyMethod().AllowCredentials().SetIsOriginAllowed((hosts) => true); //.WithOrigins("http://localhost:3000/")
});



app.UseHttpsRedirection();

app.UseAuthentication();
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
    await DbInitializer.Initialize(context, userManager);
}
catch (Exception ex)
{
    logger.LogError(ex, "A prolem occcured during migration");
}

app.Run();
