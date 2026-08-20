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
            });
        }
    }
}
