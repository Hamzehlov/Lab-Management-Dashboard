using System;
using System.Collections.Generic;
using Nabd_AlHayah_Labs.Model;

using Nabd_AlHayah_Labs.Models;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MMedicalLaboratoryAPI.Data;

public partial class MedicalLaboratoryDbContext : IdentityDbContext<ApplicationUser>
{
    public MedicalLaboratoryDbContext()
    {
    }

    public MedicalLaboratoryDbContext(DbContextOptions<MedicalLaboratoryDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<AppointmentPackage> AppointmentPackages { get; set; }

    public virtual DbSet<AppointmentTest> AppointmentTests { get; set; }


    public virtual DbSet<Code> Codes { get; set; }

    public virtual DbSet<HealthMonitoring> HealthMonitorings { get; set; }

    public virtual DbSet<HealthPackage> HealthPackages { get; set; }

    public virtual DbSet<HomeSampling> HomeSamplings { get; set; }

    public virtual DbSet<NewsEvent> NewsEvents { get; set; }

    public virtual DbSet<PackageTest> PackageTests { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    public virtual DbSet<PersonalizedHealth> PersonalizedHealths { get; set; }

    public virtual DbSet<Test> Tests { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        base.OnModelCreating(modelBuilder);



        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.AppointmentId).HasName("PK__Appointm__8ECDFCC2641027B6");

            entity.ToTable("Appointment");

            entity.Property(e => e.AppointmentDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FirstNameAr).HasMaxLength(100);
            entity.Property(e => e.FirstNameEn).HasMaxLength(100);
            entity.Property(e => e.LastNameAr).HasMaxLength(100);
            entity.Property(e => e.LastNameEn).HasMaxLength(100);
            entity.Property(e => e.MiddleNameAr).HasMaxLength(100);
            entity.Property(e => e.MiddleNameEn).HasMaxLength(100);
            entity.Property(e => e.Notes).HasMaxLength(500);

            entity.HasOne(d => d.AppointmentType).WithMany(p => p.AppointmentAppointmentTypes)
                .HasForeignKey(d => d.AppointmentTypeId)
                .HasConstraintName("FK_Appointment_Type");

            entity.HasOne(d => d.Gender).WithMany(p => p.AppointmentGenders)
                .HasForeignKey(d => d.GenderId)
                .HasConstraintName("FK_Appointment_Gender");

            entity.HasOne(d => d.Patient).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.PatientId)
                .HasConstraintName("FK_Appointment_Patient");

            entity.HasOne(d => d.Status).WithMany(p => p.AppointmentStatuses)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK_Appointment_Status");
        });

        modelBuilder.Entity<AppointmentPackage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Appointm__3214EC07DCEB650D");

            entity.ToTable("AppointmentPackage");

            entity.HasOne(d => d.Appointment).WithMany(p => p.AppointmentPackages)
                .HasForeignKey(d => d.AppointmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Appointme__Appoi__2A6B46EF");

            entity.HasOne(d => d.Package).WithMany(p => p.AppointmentPackages)
                .HasForeignKey(d => d.PackageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Appointme__Packa__2B5F6B28");
        });

        modelBuilder.Entity<AppointmentTest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Appointm__3214EC078370E1AB");

            entity.ToTable("AppointmentTest");

            entity.HasOne(d => d.Appointment).WithMany(p => p.AppointmentTests)
                .HasForeignKey(d => d.AppointmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Appointme__Appoi__269AB60B");

            entity.HasOne(d => d.Test).WithMany(p => p.AppointmentTests)
                .HasForeignKey(d => d.TestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Appointme__TestI__278EDA44");
        });



        modelBuilder.Entity<Code>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Code__3214EC07E6BCD832");

            entity.ToTable("Code");

            entity.Property(e => e.Active).HasDefaultValue(true);
            entity.Property(e => e.CodeDescAr).HasMaxLength(200);
            entity.Property(e => e.CodeDescEn).HasMaxLength(200);
        });

        modelBuilder.Entity<HealthMonitoring>(entity =>
        {
            entity.HasKey(e => e.MonitorId).HasName("PK__HealthMo__DF5D95F859912D7F");

            entity.ToTable("HealthMonitoring");

            entity.Property(e => e.BasedOnTests).HasMaxLength(300);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.RecommendationTitleAr).HasMaxLength(200);
            entity.Property(e => e.RecommendationTitleEn).HasMaxLength(200);

            entity.HasOne(d => d.Patient).WithMany(p => p.HealthMonitorings)
                .HasForeignKey(d => d.PatientId)
                .HasConstraintName("FK_HealthMonitoring_Patient");
        });

        modelBuilder.Entity<HealthPackage>(entity =>
        {
            entity.HasKey(e => e.PackageId).HasName("PK__HealthPa__322035CC585BC1F0");

            entity.ToTable("HealthPackage");

            entity.Property(e => e.Duration).HasMaxLength(100);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.PackageNameAr).HasMaxLength(200);
            entity.Property(e => e.PackageNameEn).HasMaxLength(200);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<HomeSampling>(entity =>
        {
            entity.HasKey(e => e.HomeSampleId).HasName("PK__HomeSamp__ED2760A9286349CD");

            entity.ToTable("HomeSampling");

            entity.Property(e => e.AddressAr).HasMaxLength(300);
            entity.Property(e => e.AddressEn).HasMaxLength(300);
            entity.Property(e => e.CityAr).HasMaxLength(100);
            entity.Property(e => e.CityEn).HasMaxLength(100);
            entity.Property(e => e.IsForAnotherPerson).HasDefaultValue(false);
            entity.Property(e => e.TechnicianName).HasMaxLength(150);
            entity.Property(e => e.VisitTime).HasColumnType("datetime");

            entity.HasOne(d => d.Appointment).WithMany(p => p.HomeSamplings)
                .HasForeignKey(d => d.AppointmentId)
                .HasConstraintName("FK_HomeSampling_Appointment");
        });

        modelBuilder.Entity<NewsEvent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__NewsEven__3214EC07425E19AB");

            entity.ToTable("NewsEvent");

            entity.Property(e => e.EventDate).HasColumnType("datetime");
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.TitleAr).HasMaxLength(200);
            entity.Property(e => e.TitleEn).HasMaxLength(200);
        });

        modelBuilder.Entity<PackageTest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PackageT__3214EC07BFF21A95");

            entity.ToTable("PackageTest");

            entity.HasOne(d => d.Package).WithMany(p => p.PackageTests)
                .HasForeignKey(d => d.PackageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PackageTest_Package");

            entity.HasOne(d => d.Test).WithMany(p => p.PackageTests)
                .HasForeignKey(d => d.TestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PackageTest_Test");
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.PatientId).HasName("PK__Patient__970EC3669DCB6103");

            entity.ToTable("Patient");

            entity.Property(e => e.ActivationCode).HasMaxLength(100);
            entity.Property(e => e.AddressAr).HasMaxLength(300);
            entity.Property(e => e.AddressEn).HasMaxLength(300);
            entity.Property(e => e.BloodType).HasMaxLength(10);
            entity.Property(e => e.CityAr).HasMaxLength(100);
            entity.Property(e => e.CityEn).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.EmergencyContact).HasMaxLength(50);
            entity.Property(e => e.FullNameAr).HasMaxLength(200);
            entity.Property(e => e.FullNameEn).HasMaxLength(200);
            entity.Property(e => e.GovernorateAr).HasMaxLength(100);
            entity.Property(e => e.GovernorateEn).HasMaxLength(100);
            entity.Property(e => e.InsuranceNumber).HasMaxLength(50);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastVisitDate).HasColumnType("datetime");
            entity.Property(e => e.MoHhealthNumber).HasMaxLength(50);
            entity.Property(e => e.NationalId).HasMaxLength(20);
            entity.Property(e => e.PasswordHash).HasMaxLength(300);
            entity.Property(e => e.Phone).HasMaxLength(20);

            entity.HasOne(d => d.Gender).WithMany(p => p.Patients)
                .HasForeignKey(d => d.GenderId)
                .HasConstraintName("FK_Patient_Gender");
        });

        modelBuilder.Entity<PersonalizedHealth>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Personal__3214EC074EF472A6");

            entity.ToTable("PersonalizedHealth");

            entity.Property(e => e.CardSnippetAr).HasMaxLength(300);
            entity.Property(e => e.CardSnippetEn).HasMaxLength(300);
            entity.Property(e => e.CardTitleAr).HasMaxLength(200);
            entity.Property(e => e.CardTitleEn).HasMaxLength(200);
            entity.Property(e => e.TestNameAr).HasMaxLength(200);
            entity.Property(e => e.TestNameEn).HasMaxLength(200);

            entity.HasOne(d => d.Test).WithMany(p => p.PersonalizedHealths)
                .HasForeignKey(d => d.TestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PersonalizedHealth_Test");
        });

        modelBuilder.Entity<Test>(entity =>
        {
            entity.HasKey(e => e.TestId).HasName("PK__Test__8CC331602BF08459");

            entity.ToTable("Test");

            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ShortBenefitAr).HasMaxLength(300);
            entity.Property(e => e.ShortBenefitEn).HasMaxLength(300);
            entity.Property(e => e.TestNameAr).HasMaxLength(200);
            entity.Property(e => e.TestNameEn).HasMaxLength(200);

            entity.HasOne(d => d.Category).WithMany(p => p.Tests)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK_Test_Category");
        });



        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
