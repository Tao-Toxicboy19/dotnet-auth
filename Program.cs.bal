using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Models;

var builder = WebApplication.CreateBuilder(args);

// Configure JWT authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme; // เปลี่ยน DefaultAuthenticateScheme เป็น CookieAuthenticationDefaults.AuthenticationScheme
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme; // เปลี่ยน DefaultChallengeScheme เป็น CookieAuthenticationDefaults.AuthenticationScheme
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        // ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
}).AddCookie(options =>
{
    options.Cookie.HttpOnly = true; // ทำให้ Cookie เป็น HttpOnly เพื่อป้องกันการแอบอ่านโดย JavaScript
});

// cookie authentication
builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme).AddIdentityCookies();

// configure authorization
builder.Services.AddAuthorizationBuilder();

// builder.Services.AddAuthorization();

// ตั้งค่า logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// อ่านค่าจาก appsettings.json
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// add identity and opt-in to endpoints
// builder.Services.AddIdentityCore<IdentityUser>()
//     .AddEntityFrameworkStores<AppDbContext>()
//     .AddApiEndpoints();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapRazorPages();
app.MapDefaultControllerRoute();
app.UseAuthentication(); // Ensure JWT authentication is used
app.UseAuthorization();
app.UseRouting(); // เพิ่ม UseRouting สำหรับใช้ Cookie
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
