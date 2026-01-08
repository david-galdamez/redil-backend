using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace redil_backend.Models;

public partial class RedilDBContext : DbContext
{
    public RedilDBContext()
    {
    }

    public RedilDBContext(DbContextOptions<RedilDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<class_details> class_details { get; set; }

    public virtual DbSet<classes> classes { get; set; }

    public virtual DbSet<groups> groups { get; set; }

    public virtual DbSet<rediles> rediles { get; set; }

    public virtual DbSet<roles> roles { get; set; }

    public virtual DbSet<student_redil> student_redil { get; set; }

    public virtual DbSet<students> students { get; set; }

    public virtual DbSet<users> users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<class_details>(entity =>
        {
            entity.HasKey(e => e.id).HasName("class_details_pkey");

            entity.HasIndex(e => new { e.class_id, e.student_id }, "class_details_class_id_student_id_key").IsUnique();

            entity.Property(e => e.attendance).HasDefaultValue(false);

            entity.HasOne(d => d._class).WithMany(p => p.class_details)
                .HasForeignKey(d => d.class_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("class_details_class_id_fkey");

            entity.HasOne(d => d.student).WithMany(p => p.class_details)
                .HasForeignKey(d => d.student_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("class_details_student_id_fkey");
        });

        modelBuilder.Entity<classes>(entity =>
        {
            entity.HasKey(e => e.id).HasName("classes_pkey");

            entity.HasIndex(e => e.attendance_token, "classes_attendance_token_key").IsUnique();

            entity.Property(e => e.class_date).HasColumnType("timestamp without time zone");
            entity.Property(e => e.expires_at).HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.redil).WithMany(p => p.classes)
                .HasForeignKey(d => d.redil_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("classes_redil_id_fkey");

            entity.HasOne(d => d.teacher).WithMany(p => p.classes)
                .HasForeignKey(d => d.teacher_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("classes_teacher_id_fkey");
        });

        modelBuilder.Entity<groups>(entity =>
        {
            entity.HasKey(e => e.id).HasName("groups_pkey");

            entity.HasIndex(e => e.name, "groups_name_key").IsUnique();

            entity.Property(e => e.name).HasMaxLength(100);
        });

        modelBuilder.Entity<rediles>(entity =>
        {
            entity.HasKey(e => e.id).HasName("rediles_pkey");

            entity.Property(e => e.description).HasMaxLength(255);
            entity.Property(e => e.name).HasMaxLength(100);
        });

        modelBuilder.Entity<roles>(entity =>
        {
            entity.HasKey(e => e.id).HasName("roles_pkey");

            entity.HasIndex(e => e.name, "roles_name_key").IsUnique();

            entity.Property(e => e.name).HasMaxLength(50);
        });

        modelBuilder.Entity<student_redil>(entity =>
        {
            entity.HasKey(e => e.id).HasName("student_redil_pkey");

            entity.HasIndex(e => new { e.student_id, e.redil_id }, "student_redil_student_id_redil_id_key").IsUnique();

            entity.Property(e => e.joined_at).HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.redil).WithMany(p => p.student_redil)
                .HasForeignKey(d => d.redil_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("student_redil_redil_id_fkey");

            entity.HasOne(d => d.student).WithMany(p => p.student_redil)
                .HasForeignKey(d => d.student_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("student_redil_student_id_fkey");
        });

        modelBuilder.Entity<students>(entity =>
        {
            entity.HasKey(e => e.id).HasName("students_pkey");

            entity.HasIndex(e => e.email, "students_email_key").IsUnique();

            entity.Property(e => e.created_at)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.email).HasMaxLength(100);
            entity.Property(e => e.name).HasMaxLength(100);
            entity.Property(e => e.updated_at).HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.group).WithMany(p => p.students)
                .HasForeignKey(d => d.group_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("students_group_id_fkey");
        });

        modelBuilder.Entity<users>(entity =>
        {
            entity.HasKey(e => e.id).HasName("users_pkey");

            entity.HasIndex(e => e.email, "users_email_key").IsUnique();

            entity.Property(e => e.created_at)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.email).HasMaxLength(100);
            entity.Property(e => e.is_active).HasDefaultValue(true);
            entity.Property(e => e.name).HasMaxLength(100);
            entity.Property(e => e.password).HasMaxLength(255);
            entity.Property(e => e.updated_at).HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.redil).WithMany(p => p.users)
                .HasForeignKey(d => d.redil_id)
                .HasConstraintName("users_redil_id_fkey");

            entity.HasOne(d => d.role).WithMany(p => p.users)
                .HasForeignKey(d => d.role_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("users_role_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
