using AdventureWorks.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace EntityFrameworkCoreQuery.Queries;

public class AggregateOperators
{
    public static void CountSample()
    {
        // This is an example of getting a list of ProductSubcategory.Names
        // and how many Products are attached are using it

        using (var context = new AdventureWorksDbContext())
        {
            var result = context.ProductSubcategory
                .Include(x => x.Product)
                .OrderBy(x => x.Name)
                .Select(x => new { x.Name, x.Product.Count })
                .AsNoTracking()
                .ToList();

            ObjectDumper.Write(result);
        };

        /* 
        We assume that it would generate a Sql Statement like this: 

        select b.[Name], count(*) from Production.Product a
         inner join Production.ProductSubcategory b
	            on a.ProductSubcategoryID = b.ProductSubcategoryID
        group by b.Name
        order by b.Name

        But it generates this instead:

        info: Microsoft.EntityFrameworkCore.Database.Command[20101]
              Executed DbCommand (32ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
              SELECT [p0].[Name], (
                  SELECT COUNT(*)
                  FROM [Production].[Product] AS [p]
                  WHERE [p0].[ProductSubcategoryID] = [p].[ProductSubcategoryID]) AS [Count]
              FROM [Production].[ProductSubcategory] AS [p0]
              ORDER BY [p0].[Name]             

        Check out the next method below for a better SqlStatement
         */
    }

    public static void CountSampleBetterSqlStatement()
    {
        // This code is better than the one above this as it generates
        // the expected proper Sql Statement for it

        using (var context = new AdventureWorksDbContext())
        {
            var result2 = (from a in context.Product
                           join b in context.ProductSubcategory
                             on a.ProductSubcategoryId equals b.ProductSubcategoryId
                           group b by new { b.Name } into g
                           select new
                           {
                               g.Key.Name,
                               Count = g.Count()
                           })
                           .OrderBy(x => x.Name)
                           .ToList();

            ObjectDumper.Write(result2);
        };

        /*
        info: Microsoft.EntityFrameworkCore.Database.Command[20101]
                Executed DbCommand (27ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
                SELECT [p0].[Name], COUNT(*) AS [Count]
                FROM [Production].[Product] AS [p]
                INNER JOIN [Production].[ProductSubcategory] AS [p0] ON [p].[ProductSubcategoryID] = [p0].[ProductSubcategoryID]
                GROUP BY [p0].[Name]
                ORDER BY [p0].[Name]
         */
    }

    public static void CountSampleV2()
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

    public static void GroupByHavingCount()
    {
        using var context = new AdventureWorksDbContext();

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

    public static void GroupByMultipleColumns()
    {
        using var context = new AdventureWorksDbContext();

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

        var result = query.AsNoTracking()
            .OrderBy(x => x.Count)
            .ToList();

        ObjectDumper.Write(result);

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
