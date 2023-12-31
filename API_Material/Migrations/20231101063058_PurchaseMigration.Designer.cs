﻿// <auto-generated />
using System;
using CommonLibrary.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace API_Material.Migrations
{
    [DbContext(typeof(PurchaseDBContext))]
    [Migration("20231101063058_PurchaseMigration")]
    partial class PurchaseMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CommonLibrary.Models.Purchase", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"));

                    b.Property<DateTime?>("purchaseDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("vendorId")
                        .HasColumnType("int");

                    b.HasKey("id");

                    b.ToTable("Purchases");
                });
#pragma warning restore 612, 618
        }
    }
}
