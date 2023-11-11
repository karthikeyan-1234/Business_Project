﻿// <auto-generated />
using System;
using CommonLibrary.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace API_Inventory.Migrations
{
    [DbContext(typeof(InventoryDBContext))]
    [Migration("20231105095546_updateInventory")]
    partial class updateInventory
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CommonLibrary.Models.Inventory", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"));

                    b.Property<string>("Notes")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("itemId")
                        .HasColumnType("int");

                    b.Property<DateTime>("lastUpdated")
                        .HasColumnType("datetime2");

                    b.Property<float>("qty")
                        .HasColumnType("real");

                    b.HasKey("id");

                    b.ToTable("Inventories");
                });
#pragma warning restore 612, 618
        }
    }
}
