using Microsoft.EntityFrameworkCore;

namespace redil_backend.Models
{
    public class RedilDbContext : DbContext
    {
        public RedilDbContext(DbContextOptions<RedilDbContext> options) : base(options)
        {
        }

        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Redile> Rediles => Set<Redile>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Group> Groups => Set<Group>();
        public DbSet<Student> Students => Set<Student>();
        public DbSet<StudentRedil> StudentRediles => Set<StudentRedil>();
        public DbSet<Class> Classes => Set<Class>();
        public DbSet<ClassDetail> ClassDetails => Set<ClassDetail>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin" },
                new Role { Id = 2, Name = "Maestro" });

            modelBuilder.Entity<Redile>()
                .HasIndex(r => r.Code)
                .IsUnique();

            modelBuilder.Entity<Redile>()
                .HasIndex(r => r.Name);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Group>()
                .HasIndex(g => g.Name)
                .IsUnique();

            modelBuilder.Entity<Student>()
                .HasIndex(s => s.Phone)
                .IsUnique();

            modelBuilder.Entity<StudentRedil>()
                .HasIndex(sr => new { sr.StudentId, sr.RedilId })
                .IsUnique();

            modelBuilder.Entity<StudentRedil>()
                .HasIndex(sr => new { sr.RedilId, sr.JoinedAt });

            modelBuilder.Entity<ClassDetail>()
                .HasIndex(cd => new { cd.ClassId, cd.StudentId })
                .IsUnique();

            modelBuilder.Entity<Class>()
                .HasIndex(c => c.AttendanceToken)
                .IsUnique();

            modelBuilder.Entity<Class>()
                .HasIndex(c => new { c.RedilId, c.ClassDate })
                .IsDescending(false, true);

            modelBuilder.Entity<Class>()
                .Property(c => c.ClassDate)
                .HasColumnType("timestamp with time zone");

            modelBuilder.Entity<User>()
                .Property(u => u.CreatedAt)
                .HasDefaultValueSql("NOW()");

            modelBuilder.Entity<Student>()
                .Property(s => s.CreatedAt)
                .HasDefaultValueSql("NOW()");

            modelBuilder.Entity<StudentRedil>()
                .Property(s => s.JoinedAt)
                .HasDefaultValueSql("NOW()");

            modelBuilder.Entity<StudentRedil>()
                .Property(u => u.Active)
                .HasDefaultValue(true);

            modelBuilder.Entity<User>()
                .Property(u => u.IsActive)
                .HasDefaultValue(true);

            modelBuilder.Entity<ClassDetail>()
                .Property(u => u.Attendance)
                .HasDefaultValue(false);
        }
    }
}
