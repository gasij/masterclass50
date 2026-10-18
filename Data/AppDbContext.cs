using Microsoft.EntityFrameworkCore;
using TaskBoard.Models;

namespace TaskBoard.Data;

/// <summary>Мост между C#-объектами и базой SQLite.</summary>
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskItem>(e =>
        {
            e.Property(t => t.Title).HasMaxLength(120).IsRequired();

            // В базе храним слова ("High"), а не числа: так проще читать таблицу глазами.
            e.Property(t => t.Priority).HasConversion<string>();
            e.Property(t => t.State).HasConversion<string>();
        });
    }
}
