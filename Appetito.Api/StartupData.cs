using Microsoft.EntityFrameworkCore;
using Appetito.Domain.Entities;
using Appetito.Infrastructure;

namespace Appetito.Api;

public static class StartupData
{
    public static async Task EnsureSeedAsync(AppetitoDbContext db)
    {
        if (!await db.Units.AnyAsync())
        {
            var g = new Unit { Id = Guid.NewGuid(), Name = "Gram", Abbrev = "g", IsCountable = false, BaseUnitId = null, FactorToBase = 1m };
            var kg = new Unit { Id = Guid.NewGuid(), Name = "Kilogram", Abbrev = "kg", IsCountable = false, BaseUnitId = g.Id, FactorToBase = 1000m };
            var ml = new Unit { Id = Guid.NewGuid(), Name = "Milliliter", Abbrev = "ml", IsCountable = false, FactorToBase = 1m };
            var l  = new Unit { Id = Guid.NewGuid(), Name = "Liter", Abbrev = "L", IsCountable = false, BaseUnitId = ml.Id, FactorToBase = 1000m };
            var pc = new Unit { Id = Guid.NewGuid(), Name = "Piece", Abbrev = "pc", IsCountable = true, FactorToBase = 1m };
            db.Units.AddRange(g, kg, ml, l, pc);
        }

        if (!await db.Categories.AnyAsync())
        {
            db.Categories.AddRange(
                new Category { Id = Guid.NewGuid(), Name = "Cereal" },
                new Category { Id = Guid.NewGuid(), Name = "Dairy" },
                new Category { Id = Guid.NewGuid(), Name = "Vegetables" },
                new Category { Id = Guid.NewGuid(), Name = "Fruits" },
                new Category { Id = Guid.NewGuid(), Name = "Spices" },
                new Category { Id = Guid.NewGuid(), Name = "Snacks" }
            );
        }

        if (!await db.Households.AnyAsync())
        {
            var hh = new Household { Id = Guid.NewGuid(), Name = "My Household" };
            var user = new User
            {
                Id = Guid.NewGuid(),
                HouseholdId = hh.Id,
                Email = "admin@local",
                PasswordHash = "admin", // TODO: replace with hash
                IsActive = true
            };
            db.Households.Add(hh);
            db.Users.Add(user);
        }

        await db.SaveChangesAsync();
    }
}
