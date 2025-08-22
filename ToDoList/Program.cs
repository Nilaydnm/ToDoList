using AutoMapper;
using Business.Interfaces;
using Business.Managers;
using Business.Mapping;
using Business.ValidationRules;
using DataAccess;
using DataAccess.Context;
using DataAccess.Interfaces;
using DataAccess.Repositories;
using Entities;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using ToDoList.Middleware;


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
builder.Services.AddScoped<IValidator<ToDoGroup>, ToDoGroupValidator>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(EfGenericRepository<>));
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MappingProfile>();
});
builder.Services.AddScoped<ILogRepository, EfLogRepository>();
builder.Services.AddScoped<ILogService, LogManager>();



builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});



builder.Services.AddControllersWithViews();
    

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

app.UseMiddleware<RequestLoggingMiddleware>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}");


app.Run();
