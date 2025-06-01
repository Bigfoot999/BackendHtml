using BackendHtml;
using BackendHtml.Context;
using BackendHtml.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<AIRepository>();
builder.Services.AddScoped<SiteProvider>();
builder.Services.AddMvc();
//Register

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(
    p =>
    {
        p.LoginPath = "/account/login";
        p.AccessDeniedPath = "/account/denied";
        p.Cookie.Name = "MyAppAuth";
        p.ExpireTimeSpan = TimeSpan.FromDays(30);
    }
);
builder.Services.Configure<MailSetting>(builder.Configuration.GetSection("MailSetting"));
builder.Services.AddTransient<IEmailSender, EmailSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "wwwwroot")),
    RequestPath = "" // Ánh xạ đường dẫn gốc (không cần thêm tiền tố)
});

//app.UseStaticFiles();
//app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication(); // Add this line to enable authentication middleware
app.UseAuthorization(); // Add this line to enable authorization middleware




app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");



app.Run();


