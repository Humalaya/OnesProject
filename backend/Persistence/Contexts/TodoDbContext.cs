using Microsoft.EntityFrameworkCore;
using backend.Domain.Entities;

namespace backend.Persistence.Contexts
{
    public class TodoDbContext : DbContext
    {
        public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options)
        {
        }

        public DbSet<ToDo> ToDos => Set<ToDo>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Schedule> Schedules => Set<Schedule>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ToDo>(entity =>
            {
                entity.ToTable("ToDo");
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID)
                      .HasColumnType("UNIQUEIDENTIFIER")
                      .HasDefaultValueSql("NEWID()");

                entity.Property(e => e.UserID)
                      .IsRequired();

                entity.Property(e => e.Title)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(e => e.Description)
                      .HasMaxLength(500);

                entity.Property(e => e.IsCompleted)
                      .IsRequired();

                entity.Property(e => e.Priority)
                      .IsRequired()
                      .HasDefaultValue(1);

                // No HasMaxLength: comma-separated free-text tags, unbounded like ProfilePictureUrl above.
                entity.Property(e => e.Tags);

                entity.Property(e => e.CreatedAt)
                      .IsRequired()
                      .HasDefaultValueSql("GETDATE()");

                entity.HasOne<User>()
                      .WithMany()
                      .HasForeignKey(e => e.UserID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Schedule>(entity =>
            {
                entity.ToTable("Schedule");
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID)
                      .HasColumnType("UNIQUEIDENTIFIER")
                      .HasDefaultValueSql("NEWID()");

                entity.Property(e => e.ToDoID)
                      .IsRequired();
                entity.HasIndex(e => e.ToDoID);

                entity.Property(e => e.ScheduledAt)
                      .IsRequired();

                entity.Property(e => e.Order)
                      .IsRequired();

                entity.HasOne<ToDo>()
                      .WithMany()
                      .HasForeignKey(e => e.ToDoID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID)
                      .HasColumnType("UNIQUEIDENTIFIER")
                      .HasDefaultValueSql("NEWID()");

                entity.Property(e => e.Username)
                      .IsRequired()
                      .HasMaxLength(50);
                entity.HasIndex(e => e.Username).IsUnique();

                entity.Property(e => e.Email)
                      .IsRequired()
                      .HasMaxLength(256);
                entity.HasIndex(e => e.Email).IsUnique();

                entity.Property(e => e.PasswordHash)
                      .IsRequired();

                entity.Property(e => e.FullName)
                      .HasMaxLength(100);

                // No HasMaxLength/HasColumnType: needs to hold a full base64 image data URI, and
                // an explicit length/type here wouldn't be portable across SQLite (dev) and MSSQL (prod)
                // anyway - each provider's default unbounded string mapping (TEXT / nvarchar(max)) is used.
                entity.Property(e => e.ProfilePictureUrl);

                entity.Property(e => e.EmailVerified)
                      .IsRequired()
                      .HasDefaultValue(false);

                entity.Property(e => e.EmailVerificationToken)
                      .HasMaxLength(64);

                entity.Property(e => e.CreatedAt)
                      .IsRequired()
                      .HasDefaultValueSql("GETDATE()");
            });
        }
    }
}
