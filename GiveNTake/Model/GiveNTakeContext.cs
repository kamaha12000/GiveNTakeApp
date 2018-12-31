using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GiveNTake.Model
{
    public class GiveNTakeContext : IdentityDbContext<User>
    {
        public GiveNTakeContext(DbContextOptions<GiveNTakeContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Category> Categories { get; set; }

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
            base.OnModelCreating(modelBuilder);
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

            //if (!Products.Any())
            //{
            //    Products.AddRange(
            //        new Product
            //        {
            //            Owner = Users.SingleOrDefault(u => u.Id == "seller1@seller.com"),
            //            Title = "Frigidaire",
            //            Description =@"This classic top freezer refrigerator from Frigidaire is an excellent piece 
            //                            for a starter kitchen. The Store-More™ door shelves featuring gallon storage offer plenty of 
            //                                room for condiments and drinks.",
            //            Category = Categories.SingleOrDefault(c => c.Name == "Appliances"),
            //            City = Cities.SingleOrDefault(c => c.Name == "San Francisco"),
            //            PublishDate = DateTime.Now
            //        },
            //        new Product
            //        {
            //            Owner = Users.SingleOrDefault(u => u.Id == "seller2@seller.com"),
            //            Title = "Dyson V8 Absolute vacuum cleaner",
            //            Description = @"The Dyson V8 Absolute vacuum cleaner has a soft roller cleaner head for 
            //                                hard floors and a motorized cleaner head to remove dirt from carpets. In nickel/iron.",
            //            Category = Categories.SingleOrDefault(c => c.Name == "Appliances"),
            //            City = Cities.SingleOrDefault(c => c.Name == "Seattle"),
            //            PublishDate = DateTime.Now
            //        },
            //        new Product
            //        {
            //            Owner = Users.SingleOrDefault(u => u.Id == "buyer2@buyer2.com"),
            //            Title = "Whirlpool",
            //            Description = @"1.7 cu. ft. Over the Range Microwave in Stainless Steel with Electronic Touch Controls",
            //            Category = Categories.SingleOrDefault(c => c.Name == "Microwaves"),
            //            City = Cities.SingleOrDefault(c => c.Name == "New York"),
            //            PublishDate = DateTime.Now
            //        }
            //        );
            //    SaveChanges();
            //}
        }


        public async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            if(!await roleManager.RoleExistsAsync("Admin"))
            {
                var admin = new IdentityRole("Admin");
                await roleManager.CreateAsync(admin);
            }
        }
    }
}
