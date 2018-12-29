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
        public DbSet<Category> Categories { get; set; }
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
            modelBuilder.Entity<Product>()
               .HasOne(c => c.Owner)
               .WithMany(u => u.Products)
               .IsRequired(true);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Product)
                .WithMany();
            modelBuilder.Entity<Message>()
                .HasOne(m => m.FromUser)
                .WithMany();
            modelBuilder.Entity<Message>().HasOne(m => m.ToUser)
                .WithMany();

        }

        public void SeedData()
        {
            if (!Categories.Any())
            {
                var appliances = new Category()
                {
                    Name = "Appliances",
                    SubCategories = new List<Category>()
                    {
                        new Category(){Name = "Microwaves"}
                    }
                };
                Categories.Add(appliances);
                SaveChanges();
            }

            if (!Cities.Any())
            {
                Cities.AddRange(
                    new City { Name = "New York" },
                    new City { Name = "Seattle" },
                    new City { Name = "San Francisco" });
                SaveChanges();
            }

            if (!Users.Any())
            {
                Users.AddRange(
                    new User() { Id = "seller1@seller.com" },
                    new User() { Id = "seller2@seller.com" },
                    new User() { Id = "buyer1@buyer.com" },
                    new User() { Id = "buyer2@buyer2.com" });
                SaveChanges();
            }
        }
    }
}
