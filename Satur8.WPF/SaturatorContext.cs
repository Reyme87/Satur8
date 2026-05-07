using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Satur8.WPF.Domain.Models;

namespace Satur8.WPF;

public partial class SaturatorContext : DbContext
{
    public SaturatorContext()
    {
    }

    public SaturatorContext(DbContextOptions<SaturatorContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Favourite> Favourites { get; set; }

    public virtual DbSet<Preset> Presets { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("categories_pkey");

            entity.ToTable("categories");

            entity.HasIndex(e => e.Name, "validate_categories_name").IsUnique();

            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Favourite>(entity =>
        {
            entity.HasKey(e => e.FavouritesId).HasName("favourites_pkey");

            entity.ToTable("favourites");

            entity.Property(e => e.FavouritesId).HasColumnName("favourites_id");
            entity.Property(e => e.PresetId).HasColumnName("preset_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Preset).WithMany(p => p.Favourites)
                .HasForeignKey(d => d.PresetId)
                .HasConstraintName("favourites_preset_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Favourites)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("favourites_user_id_fkey");
        });

        modelBuilder.Entity<Preset>(entity =>
        {
            entity.HasKey(e => e.PresetId).HasName("presets_pkey");

            entity.ToTable("presets");

            entity.Property(e => e.PresetId).HasColumnName("preset_id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.Description)
                .HasMaxLength(250)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.ParametersJson)
                .HasColumnType("json")
                .HasColumnName("parameters_json");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Category).WithMany(p => p.Presets)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("presets_category_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Presets)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("presets_user_id_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("users_pkey");

            entity.ToTable("users");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Login)
                .HasMaxLength(50)
                .HasColumnName("login");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(50)
                .HasColumnName("password_hash");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
