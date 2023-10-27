using BlazorApp1.Areas.Identity;
using BlazorApp1.Data;
using BlazorApp1.Domain;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString), ServiceLifetime.Transient);

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddScoped(typeof(UserManager<>));
builder.Services.AddSingleton<EncryptionHelper>();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<ApplicationUser>>();
builder.Services.AddSingleton<WeatherForecastService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

using(var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[] { "User", "Admin" };

    foreach (var role in roles)
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
}

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    var users = new List<(ApplicationUser, string)>
    {
        ( new ApplicationUser { Id = Guid.NewGuid().ToString(), UserName = "User-1", Email = "user1@tesss.com", EmailConfirmed = true }, "User" ),
        ( new ApplicationUser { Id = Guid.NewGuid().ToString(), UserName = "User-2", Email = "user2@email.com", EmailConfirmed = true }, "User" ),
        ( new ApplicationUser { Id = Guid.NewGuid().ToString(), UserName = "User-3", Email = "user3@email.com", EmailConfirmed = true }, "User" ),
        ( new ApplicationUser { Id = Guid.NewGuid().ToString(), UserName = "Admin", Email = "admin@email.com", EmailConfirmed = true }, "Admin" ),
    };

    foreach(var (user, role) in users)
    {
        if (await userManager.FindByEmailAsync(user.Email) == null)
        {
            await userManager.CreateAsync(user, @"Hejsa123###");
            await userManager.AddToRoleAsync(user, role);
        }
    }

    string email = "test@test.com";
    string password = "Test123#";

    if(await userManager.FindByEmailAsync(email) == null)
    {
        var user = new ApplicationUser();
        user.Email = email;
        user.UserName = email;
        await userManager.CreateAsync(user, password);

    }   

   
}

app.Run();
