using MedicalExpert.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedicalExpert.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Relationship Between Category And Side
            builder.Entity<Category>()
                .HasMany(e => e.Sides)
                .WithMany(e => e.Categories)
                .UsingEntity<CategorySide>(
                    e => e
                    .HasOne(e => e.Side)
                    .WithMany(e => e.CategoriesSides)
                    .HasForeignKey(e => e.SideId),
                    e => e
                    .HasOne(e => e.Category)
                    .WithMany(e => e.CategoriesSides)
                    .HasForeignKey(e => e.CategoryId),
                    e =>
                    {
                        e.HasKey(e => new { e.CategoryId, e.SideId });
                    }
                );

            // Relationship Between Disease And Medicine
            builder.Entity<Disease>()
                .HasMany(e => e.Medicines)
                .WithMany(e => e.Diseases)
                .UsingEntity<DiseaseMedicine>(
                    e => e
                    .HasOne(e => e.Medicine)
                    .WithMany(e => e.DiseasesMedicines)
                    .HasForeignKey(e => e.MedicineId),
                    e => e
                    .HasOne(e => e.Disease)
                    .WithMany(e => e.DiseasesMedicines)
                    .HasForeignKey(e => e.DiseaseId),
                    e =>
                    {
                        e.HasKey(e => new { e.DiseaseId, e.MedicineId });
                    }
                );

            // Relationship Between Medicine And MedicineForm
            builder.Entity<Medicine>()
                .HasMany(e => e.MedicineForms)
                .WithMany(e => e.Medicines)
                .UsingEntity<MedicineAndMedicineForm>(
                    e => e
                    .HasOne(e => e.MedicineForm)
                    .WithMany(e => e.MedicinesAndMedicineForms)
                    .HasForeignKey(e => e.MedicineFormId),
                    e => e
                    .HasOne(e => e.Medicine)
                    .WithMany(e => e.MedicinesAndMedicineForms)
                    .HasForeignKey(e => e.MedicineId),
                    e =>
                    {
                        e.HasKey(e => new { e.MedicineId, e.MedicineFormId });
                    }
                );
        }

        public DbSet<Advice> Advices { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Side> Sides { get; set; }
        public DbSet<CategorySide> CategoriesSides { get; set; }
        public DbSet<Disease> Diseases { get; set; }
        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<DiseaseMedicine> DiseasesMedicines { get; set; }
        public DbSet<MedicineGenre> MedicineGenres { get; set; }
        public DbSet<MedicineForm> MedicineForms { get; set; }
        public DbSet<MedicineAndMedicineForm> MedicinesAndMedicineForms { get; set; }
    }
}
