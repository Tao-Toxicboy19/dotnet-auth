using Microsoft.EntityFrameworkCore;

namespace Models;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username) // สร้าง Index สำหรับ Username
            .IsUnique(); // ทำให้ Username เป็น unique
    }
}