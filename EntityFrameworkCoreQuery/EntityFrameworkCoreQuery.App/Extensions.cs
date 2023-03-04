using AdventureWorks.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCoreQuery.App;

public static class Extensions
{
    public static void NoTracking(this AdventureWorksDbContext context)
    {
        context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }
}
