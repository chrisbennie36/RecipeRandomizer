﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using RecipeRandomizer.Api.Data;

#nullable disable

namespace RecipeRandomizer.Api.WebApplication.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20241209115557_RecipeAndUserRecipePreferences")]
    partial class RecipeAndUserRecipePreferences
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("RecipeRandomizer.Api.Data.Models.RecipePreference", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("RecipeType")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("UpdatedUtc")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("RecipePreferences");
                });

            modelBuilder.Entity("RecipeRandomizer.Api.Data.Models.UserRecipePreference", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("RecipePreferenceId")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("UpdatedUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("RecipePreferenceId");

                    b.ToTable("UserRecipePreferences");
                });

            modelBuilder.Entity("RecipeRandomizer.Api.Data.Models.UserRecipePreference", b =>
                {
                    b.HasOne("RecipeRandomizer.Api.Data.Models.RecipePreference", "RecipePreferece")
                        .WithMany()
                        .HasForeignKey("RecipePreferenceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("RecipePreferece");
                });
#pragma warning restore 612, 618
        }
    }
}
