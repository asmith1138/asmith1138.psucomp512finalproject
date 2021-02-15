using EHR.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace EHR.Data
{
    public partial class EHRContext : DbContext
    {
        public EHRContext()
        {

        }

        public EHRContext(DbContextOptions<EHRContext> options)
            : base(options)
        {
        }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Covering> Coverings { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<TestType> TypesOfTests { get; set; }
        public DbSet<Medication> Medications { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Host=localhost;Database=EHR;Username=EHRApp;Password=EHRApp");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Covering>(eb =>
            {
                eb.HasNoKey();
                eb.HasMany<Patient>();
                eb.HasMany<User>();
            });
            base.OnModelCreating(modelBuilder);
        }
    }
}
