using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using ksiegarnia.Models.Domain;
using ksiegarnia.Repositories.Abstract;
using ksiegarnia.Repositories.Implementation;
using Microsoft.AspNetCore.Authentication.OAuth;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IUserAuthenticationService, UserAuthenticationService>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IMovieService, MovieService>();

builder.Services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("conn")));

// For Identity  
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(config =>
{
    config.SignIn.RequireConfirmedEmail = true;
})
.AddEntityFrameworkStores<DatabaseContext>()
.AddDefaultTokenProviders();

// Add authentication services
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
    options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
.AddGoogle(options =>
{
    options.ClientId = "";
    options.ClientSecret = "";
    options.CallbackPath = new PathString("/signin-google");
    options.Events = new OAuthEvents
    {
        OnCreatingTicket = context =>
        {
            // You can get user information from the context here
            var accessToken = context.AccessToken;
            var usernameClaim = context.Principal.FindFirst(ClaimTypes.Name);
            var username = usernameClaim?.Value;

            var emailClaim = context.Principal.FindFirst(ClaimTypes.Email);


            System.Diagnostics.Debug.WriteLine(username);
            // Do something with the user info here if needed
            return Task.CompletedTask;
        }
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
