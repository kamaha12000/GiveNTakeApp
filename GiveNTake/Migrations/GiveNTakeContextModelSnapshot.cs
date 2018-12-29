﻿// <auto-generated />
using System;
using GiveNTake.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GiveNTake.Migrations
{
    [DbContext(typeof(GiveNTakeContext))]
    partial class GiveNTakeContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.0-rtm-35687")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("GiveNTake.Model.Category", b =>
                {
                    b.Property<int>("CategoryId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.Property<int?>("ParentCategoryCategoryId");

                    b.HasKey("CategoryId");

                    b.HasIndex("ParentCategoryCategoryId");

                    b.ToTable("Category");
                });

            modelBuilder.Entity("GiveNTake.Model.City", b =>
                {
                    b.Property<int>("CityId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.HasKey("CityId");

                    b.ToTable("Cities");
                });

            modelBuilder.Entity("GiveNTake.Model.Message", b =>
                {
                    b.Property<int>("MessageId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("FromUserUserId");

                    b.Property<int?>("ProductId");

                    b.Property<string>("Title");

                    b.Property<int?>("ToUserUserId");

                    b.Property<string>("body");

                    b.HasKey("MessageId");

                    b.HasIndex("FromUserUserId");

                    b.HasIndex("ProductId");

                    b.HasIndex("ToUserUserId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("GiveNTake.Model.Product", b =>
                {
                    b.Property<int>("ProductId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("CategoryId");

                    b.Property<int?>("CityId");

                    b.Property<string>("Description");

                    b.Property<int?>("OwnerUserId");

                    b.Property<DateTime>("PublishDate");

                    b.Property<string>("Title");

                    b.HasKey("ProductId");

                    b.HasIndex("CategoryId");

                    b.HasIndex("CityId");

                    b.HasIndex("OwnerUserId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("GiveNTake.Model.ProductMedia", b =>
                {
                    b.Property<int>("ProductMediaId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("ProductId");

                    b.Property<string>("Url");

                    b.HasKey("ProductMediaId");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductMedia");
                });

            modelBuilder.Entity("GiveNTake.Model.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("GiveNTake.Model.Category", b =>
                {
                    b.HasOne("GiveNTake.Model.Category", "ParentCategory")
                        .WithMany("SubCategories")
                        .HasForeignKey("ParentCategoryCategoryId");
                });

            modelBuilder.Entity("GiveNTake.Model.Message", b =>
                {
                    b.HasOne("GiveNTake.Model.User", "FromUser")
                        .WithMany()
                        .HasForeignKey("FromUserUserId");

                    b.HasOne("GiveNTake.Model.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId");

                    b.HasOne("GiveNTake.Model.User", "ToUser")
                        .WithMany()
                        .HasForeignKey("ToUserUserId");
                });

            modelBuilder.Entity("GiveNTake.Model.Product", b =>
                {
                    b.HasOne("GiveNTake.Model.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId");

                    b.HasOne("GiveNTake.Model.City", "City")
                        .WithMany()
                        .HasForeignKey("CityId");

                    b.HasOne("GiveNTake.Model.User", "Owner")
                        .WithMany("Products")
                        .HasForeignKey("OwnerUserId");
                });

            modelBuilder.Entity("GiveNTake.Model.ProductMedia", b =>
                {
                    b.HasOne("GiveNTake.Model.Product")
                        .WithMany("Media")
                        .HasForeignKey("ProductId");
                });
#pragma warning restore 612, 618
        }
    }
}
