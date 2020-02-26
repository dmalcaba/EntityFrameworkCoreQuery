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
                    .AsNoTracking()
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
                var result = context.ProductSubcategory
                    .Select(b =>
                        new 
                        { 
                            ProductCategory = b.ProductCategory,
                            ProductCount = b.Product.Count()
                        });

                ObjectDumper.Write(result);
            };

            /*
            Generates:

            info: Microsoft.EntityFrameworkCore.Database.Command[20101]
                  Executed DbCommand (48ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
                  SELECT [p1].[ProductCategoryID], [p1].[ModifiedDate], [p1].[Name], [p1].[rowguid], (
                      SELECT COUNT(*)
                      FROM [Production].[Product] AS [p]
                      WHERE [p0].[ProductSubcategoryID] = [p].[ProductSubcategoryID]) AS [ProductCount]
                  FROM [Production].[ProductSubcategory] AS [p0]
                  INNER JOIN [Production].[ProductCategory] AS [p1] ON [p0].[ProductCategoryID] = [p1].[ProductCategoryID]          
             */
        }

        public void GroupBy()
        {
            using (var context = new AdventureWorksDbContext())
            {
                var query = from p in context.ProductVendor
                            group p by p.BusinessEntityId into g
                            where g.Count() > 3
                            orderby g.Key
                            select new
                            {
                                g.Key,
                                Count = g.Count()
                            };

                var result = query.AsNoTracking().OrderBy(x => x.Count).ToList();

                ObjectDumper.Write(result);
            }

            /*
            info: Microsoft.EntityFrameworkCore.Database.Command[20101]
                  Executed DbCommand (27ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
                  SELECT [p].[BusinessEntityID] AS [Key], COUNT(*) AS [Count]
                  FROM [Purchasing].[ProductVendor] AS [p]
                  GROUP BY [p].[BusinessEntityID]
                  HAVING COUNT(*) > 3
                  ORDER BY COUNT(*)
            */
        }

        public void GroupByMultipleColumns()
        {
            using (var context = new AdventureWorksDbContext())
            {
                var query = from p in context.ProductVendor
                            join v in context.Vendor
                                on p.BusinessEntityId equals v.BusinessEntityId
                            group p by new { p.BusinessEntityId, v.AccountNumber, v.Name } into g
                            where g.Count() > 3
                            orderby g.Key.Name
                            select new
                            {
                                g.Key.BusinessEntityId,
                                g.Key.AccountNumber,
                                g.Key.Name,
                                Count = g.Count()
                            };

                var result = query.AsNoTracking().OrderBy(x => x.Count).ToList();

                ObjectDumper.Write(result);
            }

            /*
            info: Microsoft.EntityFrameworkCore.Database.Command[20101]
                  Executed DbCommand (29ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
                  SELECT [p].[BusinessEntityID] AS [BusinessEntityId], [v].[AccountNumber], [v].[Name], COUNT(*) AS [Count]
                  FROM [Purchasing].[ProductVendor] AS [p]
                  INNER JOIN [Purchasing].[Vendor] AS [v] ON [p].[BusinessEntityID] = [v].[BusinessEntityID]
                  GROUP BY [p].[BusinessEntityID], [v].[AccountNumber], [v].[Name]
                  HAVING COUNT(*) > 3
                  ORDER BY COUNT(*)
            */
        }
    }
}
