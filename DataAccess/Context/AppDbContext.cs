using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Entities;
using System.Threading.Tasks;

namespace DataAccess.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<ToDo> ToDos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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
