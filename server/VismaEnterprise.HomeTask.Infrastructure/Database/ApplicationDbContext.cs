using Microsoft.EntityFrameworkCore;
using VismaEnterprise.HomeTask.Domain.Aggregates;
using VismaEnterprise.HomeTask.Domain.Entities;
using VismaEnterprise.HomeTask.Domain.ValueObjects;

namespace VismaEnterprise.HomeTask.Infrastructure.Database;

public class HomeTaskDbContext : DbContext
{
    public HomeTaskDbContext(DbContextOptions<HomeTaskDbContext> options) : base(options) { }

    public DbSet<CatalogueEntry> CatalogueEntries { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserAccount> UserAccounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Book>(b =>
        {
            b.OwnsOne(e => e.Author, ba =>
            {
                ba.Property(a => a.Name).HasColumnName(nameof(Author) + nameof(Author.Name));
                ba.Property(a => a.Surname).HasColumnName(nameof(Author) + nameof(Author.Surname));
            });
        });

        modelBuilder.Entity<CatalogueEntry>(ce =>
        {
            ce.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId);
            ce.HasOne(e => e.Book).WithMany().HasForeignKey(e => e.BookId);
        });

        modelBuilder.Entity<User>(u =>
        {
            u.HasOne(e => e.UserAccount).WithOne().HasForeignKey<User>(e => e.UserAccountId);
        });
    }

    // A bit of a hack for migrations
    public HomeTaskDbContext() { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured)
        {
            return;
        }

        var connectionString = "server=localhost;port=3306;database=home_task;user=root;password=29465532";

        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    }
}