using Business.Interfaces;
using Business.Managers;
using Business.ValidationRules;
using DataAccess;
using DataAccess.Context;
using DataAccess.Interfaces;
using DataAccess.Repositories;
using Entities;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
//builder.Services.AddScoped<IValidator<User>, UserValidator>();

builder.Services.AddScoped<IToDoGroupService, ToDoGroupManager>();
builder.Services.AddScoped<IToDoGroupRepository, ToDoGroupRepository>();
builder.Services.AddScoped<IUserService, UserManager>();
builder.Services.AddScoped<IUserRepository, EfUserRepository>();
builder.Services.AddScoped<IValidator<User>, UserValidator>();
builder.Services.AddScoped<IToDoService, ToDoManager>();
builder.Services.AddValidatorsFromAssemblyContaining<ToDoGroupValidator>();
builder.Services.AddScoped<IValidator<ToDo>, ToDoValidator>();
builder.Services.AddScoped<IToDoRepository, EfToDoRepository>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(EfGenericRepository<>));




builder.Services.AddControllersWithViews();
    //.AddFluentValidation(fv =>
    //{
    //    fv.RegisterValidatorsFromAssemblyContaining<UserValidator>();
    //    fv.DisableDataAnnotationsValidation = true;
    //    fv.ImplicitlyValidateChildProperties = true;
    //    fv.ImplicitlyValidateRootCollectionElements = true;
    //    fv.AutomaticValidationEnabled = true; 
    //});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});


builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/Login";
    });

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}");

app.Run();
