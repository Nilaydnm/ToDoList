using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Entities;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace DataAccess.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<ToDo> ToDos { get; set; }
        public DbSet<ToDoGroup> ToDoGroups { get; set; }
        public DbSet<LogEntry> LogEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
        
            modelBuilder.Entity<User>()
            .Property(u => u.Username)
            .IsRequired()
            .HasColumnType("varchar(20)");

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique(); 

            modelBuilder.Entity<User>()
                .Property(u => u.Password)
                .IsRequired()
                .HasColumnType("varchar(100)");

            
            modelBuilder.Entity<ToDo>()
                .Property(t => t.Title)
                .IsRequired()
                .HasColumnType("varchar(200)");

            // Kullanıcı adı zorunlu olsun 
            modelBuilder.Entity<User>()
                .Property(u => u.Username)
                .IsRequired();

            // Görev başlığı zorunlu olsun
            modelBuilder.Entity<ToDo>()
                .Property(t => t.Title)
                .IsRequired();




        }
    }
}
