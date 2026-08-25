using Microsoft.EntityFrameworkCore;
using TodoListApi.Domain.Entities;

namespace TodoListApi.Persistence.Contexts
{
    public class TodoDbContext : DbContext
    {
        public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options)
        {
        }

        public DbSet<ToDo> ToDos => Set<ToDo>();
        public DbSet<User> Users => Set<User>();

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

                entity.Property(e => e.CreatedAt)
                      .IsRequired()
                      .HasDefaultValueSql("GETDATE()");

                entity.HasOne<User>()
                      .WithMany()
                      .HasForeignKey(e => e.UserID)
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

                entity.Property(e => e.ProfilePictureUrl)
                      .HasMaxLength(500);

                entity.Property(e => e.CreatedAt)
                      .IsRequired()
                      .HasDefaultValueSql("GETDATE()");
            });
        }
    }
}
