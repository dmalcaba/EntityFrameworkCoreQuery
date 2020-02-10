using AdventureWorks.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace EntityFrameworkCoreQuery.App
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Entity Framework Core Query App");

            using var context = new AdventureWorksDbContext();
            context.NoTracking();

            var result = context.ProductSubcategory
                .Include(x => x.ProductCategory)
                .Take(2).ToList();
        }
    }
}
