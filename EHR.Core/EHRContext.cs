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
                eb.HasKey(k => new { k.PatientId, k.UserId });
                eb.HasOne<Patient>(nav=> nav.Patient);
                eb.HasOne<User>(nav => nav.User);
            });
            modelBuilder.Entity<Patient>(eb =>
            {
                eb.HasKey("MRN");
                eb.HasMany<Covering>(nav => nav.CareTeam);
                eb.HasMany<Test>();
                eb.HasMany<Medication>();
                eb.HasMany<Note>();
            });
            modelBuilder.Entity<Note>(eb =>
            {
                eb.HasOne<Patient>(nav => nav.Patient);
                eb.HasOne<User>(nav => nav.UserOrdered);
            });
            modelBuilder.Entity<Medication>(eb =>
            {
                eb.HasOne<Patient>(nav => nav.Patient);
                eb.HasOne<User>(nav => nav.UserOrdered);
            });
            modelBuilder.Entity<Test>(eb =>
            {
                eb.HasOne<Patient>(nav => nav.Patient);
                eb.HasOne<User>(nav => nav.UserOrdered);
            });
            base.OnModelCreating(modelBuilder);
        }
    }
}
