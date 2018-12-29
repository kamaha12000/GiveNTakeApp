using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GiveNTake.Model
{
    public class GiveNTakeContext : DbContext
    {
        public GiveNTakeContext(DbContextOptions<GiveNTakeContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>()
                .HasOne(sub => sub.ParentCategory)
                .WithMany(c => c.SubCategories)
                .IsRequired(false);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany();
                
        }
    }
}
