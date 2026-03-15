using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Scalar.AspNetCore;
using System.Text;
using TodoApi.Models;
using TodoApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// MongoDB pour TodoItems
builder.Services.Configure<TodoDatabaseSettings>(
    builder.Configuration.GetSection("BookStoreDatabase"));
builder.Services.AddSingleton<TodoItemsService>();

// MongoDB pour Identity
var mongoConnectionString = builder.Configuration["BookStoreDatabase:ConnectionString"]!;
var mongoDatabaseName = builder.Configuration["BookStoreDatabase:DatabaseName"]!;

// Ajouter tlsInsecure directement dans la connection string
var mongoConnectionStringWithTls = mongoConnectionString.Contains("?")
    ? mongoConnectionString + "&tlsInsecure=true&tls=true"
    : mongoConnectionString + "?tlsInsecure=true&tls=true";

builder.Services.AddIdentity<AppUser, AppRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
})
.AddMongoDbStores<AppUser, AppRole, Guid>(mongoConnectionStringWithTls, mongoDatabaseName)
.AddDefaultTokenProviders();

// JWT
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Seed des rôles et admin par défaut
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

    foreach (var role in new[] { "admin", "user" })
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new AppRole { Name = role });
    }

    var adminEmail = "admin@todoapi.com";
    if (await userManager.FindByEmailAsync(adminEmail) is null)
    {
        var admin = new AppUser { UserName = adminEmail, Email = adminEmail };
        await userManager.CreateAsync(admin, "Admin123!");
        await userManager.AddToRoleAsync(admin, "admin");
    }
}

app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options.Servers = new List<ScalarServer>
    {
        new ScalarServer("https://todoapi-6bin.onrender.com")
    };
});

//app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();