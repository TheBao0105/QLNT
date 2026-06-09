using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.EntityFrameworkCore;
using QLNT.Data;
using System.Security.Claims;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("QLNT") ?? throw new InvalidOperationException("Connection string 'QLNT' not found.")));

var facebookAppId = builder.Configuration["Authentication:Facebook:AppId"];
var facebookAppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
var googleClientId = builder.Configuration["Authentication:Google:ClientId"];
var googleClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

var authBuilder = builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
});

authBuilder.AddCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

authBuilder.AddCookie("ExternalCookie");

if (!string.IsNullOrWhiteSpace(facebookAppId) && !string.IsNullOrWhiteSpace(facebookAppSecret))
{
    authBuilder.AddFacebook(options =>
    {
        options.SignInScheme = "ExternalCookie";
        options.AppId = facebookAppId;
        options.AppSecret = facebookAppSecret;
        options.Fields.Add("email");
        options.Fields.Add("name");
        options.Events = new OAuthEvents
        {
            OnCreatingTicket = context =>
            {
                if (context.User.TryGetProperty("email", out var emailProperty) &&
                    !string.IsNullOrEmpty(emailProperty.GetString()))
                {
                    context.Identity?.AddClaim(new Claim(ClaimTypes.Email, emailProperty.GetString()!));
                }

                if (context.User.TryGetProperty("name", out var nameProperty) &&
                    !string.IsNullOrEmpty(nameProperty.GetString()))
                {
                    context.Identity?.AddClaim(new Claim(ClaimTypes.Name, nameProperty.GetString()!));
                }

                return Task.CompletedTask;
            }
        };
    });
}

if (!string.IsNullOrWhiteSpace(googleClientId) && !string.IsNullOrWhiteSpace(googleClientSecret))
{
    authBuilder.AddGoogle(options =>
    {
        options.SignInScheme = "ExternalCookie";
        options.ClientId = googleClientId;
        options.ClientSecret = googleClientSecret;
        options.Scope.Add("profile");
        options.Scope.Add("email");
    });
}

builder.Services.AddAuthorization();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
