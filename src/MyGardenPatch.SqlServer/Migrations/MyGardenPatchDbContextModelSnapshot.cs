﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyGardenPatch.SqlServer;

#nullable disable

namespace MyGardenPatch.SqlServer.Migrations
{
    [DbContext(typeof(MyGardenPatchDbContext))]
    partial class MyGardenPatchDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("MyGardenPatch.GardenBeds.GardenBed", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("GardenId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("GardenerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ImageDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageUri")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Shape")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("GardenId");

                    b.HasIndex("GardenerId");

                    b.ToTable("GardenBeds");
                });

            modelBuilder.Entity("MyGardenPatch.GardenBeds.Plant", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("GardenBedId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ImageDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageUri")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Shape")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("GardenBedId");

                    b.ToTable("Plant");
                });

            modelBuilder.Entity("MyGardenPatch.Gardeners.Gardener", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("EmailAddress")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<bool>("ReceivesEmail")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("RegisteredAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("EmailAddress")
                        .IsUnique();

                    b.ToTable("Gardeners");
                });

            modelBuilder.Entity("MyGardenPatch.Gardens.Garden", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Center")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("GardenerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ImageDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageUri")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.HasKey("Id");

                    b.HasIndex("GardenerId");

                    b.ToTable("Gardens");
                });

            modelBuilder.Entity("MyGardenPatch.GardenBeds.Plant", b =>
                {
                    b.HasOne("MyGardenPatch.GardenBeds.GardenBed", null)
                        .WithMany("Plants")
                        .HasForeignKey("GardenBedId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MyGardenPatch.GardenBeds.GardenBed", b =>
                {
                    b.Navigation("Plants");
                });
#pragma warning restore 612, 618
        }
    }
}
