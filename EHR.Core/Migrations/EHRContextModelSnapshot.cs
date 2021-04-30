﻿// <auto-generated />
using System;
using EHR.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace EHR.Data.Migrations
{
    [DbContext(typeof(EHRContext))]
    partial class EHRContextModelSnapshot : ModelSnapshot
    {
        //Entity Framework model building for database migrations
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.4")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("EHR.Data.Models.Covering", b =>
                {
                    b.Property<Guid>("PatientId")
                        .HasColumnType("uuid");

                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<bool>("Primary")
                        .HasColumnType("boolean");

                    b.HasKey("PatientId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("Coverings");
                });

            modelBuilder.Entity("EHR.Data.Models.Medication", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Dosage")
                        .HasColumnType("text");

                    b.Property<DateTime?>("Expires")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Frequency")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<Guid>("PatientId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("PatientMRN")
                        .HasColumnType("uuid");

                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("PatientId");

                    b.HasIndex("PatientMRN");

                    b.HasIndex("UserId");

                    b.ToTable("Medications");
                });

            modelBuilder.Entity("EHR.Data.Models.Note", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<Guid>("PatientId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("PatientMRN")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("Recorded")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Text")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("PatientId");

                    b.HasIndex("PatientMRN");

                    b.HasIndex("UserId");

                    b.ToTable("Notes");
                });

            modelBuilder.Entity("EHR.Data.Models.Patient", b =>
                {
                    b.Property<Guid>("MRN")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("DOB")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Gender")
                        .HasColumnType("text");

                    b.Property<decimal>("Height")
                        .HasColumnType("numeric");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<decimal>("Weight")
                        .HasColumnType("numeric");

                    b.HasKey("MRN");

                    b.ToTable("Patients");
                });

            modelBuilder.Entity("EHR.Data.Models.Role", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("ConcurrencyStamp")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("NormalizedName")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("EHR.Data.Models.Test", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<Guid>("PatientId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("PatientMRN")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("Performed")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Results")
                        .HasColumnType("text");

                    b.Property<int>("TestTypeId")
                        .HasColumnType("integer");

                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("PatientId");

                    b.HasIndex("PatientMRN");

                    b.HasIndex("TestTypeId");

                    b.HasIndex("UserId");

                    b.ToTable("Tests");
                });

            modelBuilder.Entity("EHR.Data.Models.TestType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("TypesOfTests");
                });

            modelBuilder.Entity("EHR.Data.Models.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("integer");

                    b.Property<string>("ConcurrencyStamp")
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("boolean");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnType("text");

                    b.Property<string>("NormalizedUserName")
                        .HasColumnType("text");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("RoleId")
                        .HasColumnType("text");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("text");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("boolean");

                    b.Property<string>("UserName")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("EHR.Data.Models.Covering", b =>
                {
                    b.HasOne("EHR.Data.Models.Patient", "Patient")
                        .WithMany("CareTeam")
                        .HasForeignKey("PatientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EHR.Data.Models.User", "User")
                        .WithMany("PatientsCovering")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    //b.Navigation("Patient");

                    //b.Navigation("User");
                });

            modelBuilder.Entity("EHR.Data.Models.Medication", b =>
                {
                    b.HasOne("EHR.Data.Models.Patient", "Patient")
                        .WithMany("Medications")
                        .HasForeignKey("PatientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EHR.Data.Models.Patient", null)
                        .WithMany()
                        .HasForeignKey("PatientMRN");

                    b.HasOne("EHR.Data.Models.User", "UserOrdered")
                        .WithMany("MedicationsOrdered")
                        .HasForeignKey("UserId");

                    //b.Navigation("Patient");

                    //b.Navigation("UserOrdered");
                });

            modelBuilder.Entity("EHR.Data.Models.Note", b =>
                {
                    b.HasOne("EHR.Data.Models.Patient", "Patient")
                        .WithMany("Notes")
                        .HasForeignKey("PatientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EHR.Data.Models.Patient", null)
                        .WithMany()
                        .HasForeignKey("PatientMRN");

                    b.HasOne("EHR.Data.Models.User", "UserOrdered")
                        .WithMany("NotesWritten")
                        .HasForeignKey("UserId");

                    //b.Navigation("Patient");

                    //b.Navigation("UserOrdered");
                });

            modelBuilder.Entity("EHR.Data.Models.Test", b =>
                {
                    b.HasOne("EHR.Data.Models.Patient", "Patient")
                        .WithMany("Tests")
                        .HasForeignKey("PatientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EHR.Data.Models.Patient", null)
                        .WithMany()
                        .HasForeignKey("PatientMRN");

                    b.HasOne("EHR.Data.Models.TestType", "TestType")
                        .WithMany()
                        .HasForeignKey("TestTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EHR.Data.Models.User", "UserOrdered")
                        .WithMany("TestsOrdered")
                        .HasForeignKey("UserId");

                    //b.Navigation("Patient");

                    //b.Navigation("TestType");

                    //b.Navigation("UserOrdered");
                });

            modelBuilder.Entity("EHR.Data.Models.User", b =>
                {
                    b.HasOne("EHR.Data.Models.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId");

                    //b.Navigation("Role");
                });

            modelBuilder.Entity("EHR.Data.Models.Patient", b =>
                {
                    //b.Navigation("CareTeam");

                    //b.Navigation("Medications");

                    //b.Navigation("Notes");

                    //b.Navigation("Tests");
                });

            modelBuilder.Entity("EHR.Data.Models.User", b =>
                {
                    //b.Navigation("MedicationsOrdered");

                    //b.Navigation("NotesWritten");

                    //b.Navigation("PatientsCovering");

                    //b.Navigation("TestsOrdered");
                });
#pragma warning restore 612, 618
        }
    }
}
