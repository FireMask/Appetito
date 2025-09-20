using Appetito.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Appetito.Infrastructure;

public sealed class AppetitoDbContext(DbContextOptions<AppetitoDbContext> options)
    : DbContext(options)
{
    public DbSet<Household> Households => Set<Household>();
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Unit> Units => Set<Unit>();
    public DbSet<Item> Items => Set<Item>();
    public DbSet<StockLot> StockLots => Set<StockLot>();
    public DbSet<StockTransaction> StockTransactions => Set<StockTransaction>();
    public DbSet<ShoppingList> ShoppingLists => Set<ShoppingList>();
    public DbSet<ShoppingListItem> ShoppingListItems => Set<ShoppingListItem>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.HasPostgresExtension("uuid-ossp"); // optional

        // Household
        b.Entity<Household>().Property(x => x.Name).HasMaxLength(120).IsRequired();

        // User
        b.Entity<User>()
         .HasIndex(x => x.Email).IsUnique();
        b.Entity<User>()
         .HasOne(x => x.Household).WithMany(h => h.Users).HasForeignKey(x => x.HouseholdId);

        // RefreshToken
        b.Entity<RefreshToken>()
         .HasOne(rt => rt.User).WithMany(u => u.RefreshTokens).HasForeignKey(rt => rt.UserId);

        // Category
        b.Entity<Category>().HasIndex(x => x.Name).IsUnique();
        b.Entity<Category>().Property(x => x.Name).HasMaxLength(80).IsRequired();

        // Unit
        b.Entity<Unit>().HasIndex(x => x.Abbrev).IsUnique();
        b.Entity<Unit>().Property(x => x.Name).HasMaxLength(50).IsRequired();
        b.Entity<Unit>().Property(x => x.Abbrev).HasMaxLength(10).IsRequired();

        // Item
        b.Entity<Item>()
         .HasIndex(i => i.HouseholdId) // case-insensitive-ish
         .IsUnique();
        b.Entity<Item>().Property(x => x.Name).HasMaxLength(140).IsRequired();
        b.Entity<Item>().Property(x => x.ReorderPoint).HasColumnType("numeric(14,3)");
        b.Entity<Item>().Property(x => x.TargetQty).HasColumnType("numeric(14,3)");

        // StockLot
        b.Entity<StockLot>()
         .HasIndex(l => new { l.ItemId, l.ExpiresAt, l.PurchasedAt });
        b.Entity<StockLot>().Property(x => x.Quantity).HasColumnType("numeric(14,3)");
        b.Entity<StockLot>().Property(x => x.UnitPrice).HasColumnType("numeric(14,4)");

        // StockTransaction
        b.Entity<StockTransaction>()
         .HasIndex(t => new { t.ItemId, t.OccurredAt });
        b.Entity<StockTransaction>().Property(x => x.QuantityDelta).HasColumnType("numeric(14,3)");

        // Shopping
        b.Entity<ShoppingList>().Property(x => x.Status).HasDefaultValue(ShoppingListStatus.Suggested);
        b.Entity<ShoppingListItem>().Property(x => x.ReasonFlags).HasDefaultValue(SuggestionFlags.None);
    }
}
