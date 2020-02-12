using AdventureWorks.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace EntityFrameworkCoreQuery.Queries
{
    public class AggregateOperators
    {
        public void CountSample()
        {
            using (var context = new AdventureWorksDbContext())
            {
                var result = context.ProductSubcategory
                    .Include(x => x.Product)
                    .Select(x => new { x.Name, x.Product.Count })
                    .OrderBy(x => x.Name)
                    .ToList();

                ObjectDumper.Write(result);
            };

            /*
            Generates:

            info: Microsoft.EntityFrameworkCore.Database.Command[20101]
                  Executed DbCommand (32ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
                  SELECT [p0].[Name], (
                      SELECT COUNT(*)
                      FROM [Production].[Product] AS [p]
                      WHERE [p0].[ProductSubcategoryID] = [p].[ProductSubcategoryID]) AS [Count]
                  FROM [Production].[ProductSubcategory] AS [p0]
                  ORDER BY [p0].[Name]             
             */
        }

        public void CountSampleV2()
        {
            using (var context = new AdventureWorksDbContext())
            {
                var result = context.Product
                    .GroupBy(o => o.ProductSubcategory)
                    .Select(g => new { g.Key.ProductCategory.Name, Count = g.Count() })
                    .ToList();

                ObjectDumper.Write(result);
            };

            /*
            Generates:

            info: Microsoft.EntityFrameworkCore.Database.Command[20101]
                  Executed DbCommand (32ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
                  SELECT [p0].[Name], (
                      SELECT COUNT(*)
                      FROM [Production].[Product] AS [p]
                      WHERE [p0].[ProductSubcategoryID] = [p].[ProductSubcategoryID]) AS [Count]
                  FROM [Production].[ProductSubcategory] AS [p0]
                  ORDER BY [p0].[Name]             
             */
        }
    }
}
