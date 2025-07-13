using Microsoft.EntityFrameworkCore;

namespace DiscountCodeServer.Models
{
    public static class SeedData
    {
        public static async Task Initialize(DatabaseContext context)
        {
            if (!await context.DiscountCodes.AnyAsync())
            {
                await context.DiscountCodes.AddRangeAsync(
                    new DiscountCode { Code = "WELCOME10", IsUsed = false, GeneratedAt = DateTime.UtcNow },
                    new DiscountCode { Code = "SAVE20", IsUsed = false, GeneratedAt = DateTime.UtcNow }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}
