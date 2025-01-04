using Business.Services.Abstract;
using Business.Services.Concrete;
using Business.Utilities.Stripe;
using Core.Entities;
using Data.Contexts;
using Data.Repositories.Abstract;
using Data.Repositories.Concrete;
using Data.UnitOfWork;
using Fruitkh.Utilities.EmailHandler.Abstract;
using Fruitkh.Utilities.EmailHandler.Concrete;
using Fruitkh.Utilities.EmailHandler.Models;
using IdentityProject;
using IdentityProject.Utilities.File;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

#region Builder

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(x => x.UseSqlServer(builder.Configuration.GetConnectionString("Default"), x => x.MigrationsAssembly("Data")));
builder.Services.AddSingleton<IFileService,IdentityProject.Utilities.File.FileService>();

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Lockout.MaxFailedAccessAttempts = 3;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

var emailConfiguration = builder.Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfiguration);
builder.Services.AddSingleton<IEmailService, EmailService>();

builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
#endregion

#region Repositories

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderProductRepository, OrderProductRepository>();
builder.Services.AddScoped<IBasketProductRepository, BasketProductRepository>();
builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.AddScoped<ITeamMemberRepository, TeamMemberRepository>();
builder.Services.AddScoped<INewsRepository, NewsRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

#endregion

#region Services

builder.Services.AddScoped<IHomeService, HomeService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IBasketService, BasketService>();
builder.Services.AddScoped<IShopService, ShopService>();
builder.Services.AddScoped<IAboutService, AboutService>();
builder.Services.AddScoped<ITeamMemberService, TeamMemberService>();
builder.Services.AddScoped<IAccountService, Business.Services.Concrete.AccountService>();
builder.Services.AddScoped<INewsService, NewsService>();
builder.Services.AddScoped<IProductService, Business.Services.Concrete.ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
builder.Services.AddSingleton<IUrlHelperFactory, UrlHelperFactory>();
builder.Services.AddHttpContextAccessor();

#endregion


#region App
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetService<UserManager<User>>();
    var roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();
    await DbInitializer.Seed(userManager, roleManager);
}

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}");

StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe")["SecretKey"];

app.Run();

#endregion