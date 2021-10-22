using AdventureWorks.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityFrameworkCoreQuery.Queries
{
    public class Quantifiers
    {
        // Example of query that generates WHERE IN statement
        public void ContainsIn()
        {
            using (var context = new AdventureWorksDbContext())
            {
                string[] productClass = { "H", "M" };

                var query = context.Product
                    .Where(x => productClass.Contains(x.Class))
                    .Select(x => new { x.ProductId, x.Name, x.Class, x.ProductNumber });

                var result = query.OrderBy(x => x.Name).Take(10).ToList();

                ObjectDumper.Write(result);

                /*
                info: Microsoft.EntityFrameworkCore.Database.Command[20101]
                      Executed DbCommand (39ms) [Parameters=[@__p_1='?' (DbType = Int32)], CommandType='Text', CommandTimeout='30']
                      SELECT TOP(@__p_1) [p].[ProductID] AS [ProductId], [p].[Name], [p].[Class], [p].[ProductNumber]
                      FROM [Production].[Product] AS [p]
                      WHERE [p].[Class] IN (N'H', N'M')
                      ORDER BY [p].[Name]
                 */
            }
        }

        public void ContainsInExample2()
        {
            using (var context = new AdventureWorksDbContext())
            {
                var productClass = context.Product
                    .Where(x => x.Class != null)
                    .Select(x => x.Class)
                    .Distinct()
                    .ToArray();

                var query = context.Product
                    .Where(x => productClass.Contains(x.Class))
                    .Select(x => new { x.ProductId, x.Name, x.Class, x.ProductNumber });

                var result = query
                    .OrderBy(x => x.Name)
                    .Take(10)
                    .ToList();

                ObjectDumper.Write(result);

                /*
                info: Microsoft.EntityFrameworkCore.Database.Command[20101]
                      Executed DbCommand (19ms) [Parameters=[@__p_1='?' (DbType = Int32)], CommandType='Text', CommandTimeout='30']
                      SELECT TOP(@__p_1) [p].[ProductID] AS [ProductId], [p].[Name], [p].[Class], [p].[ProductNumber]
                      FROM [Production].[Product] AS [p]
                      WHERE [p].[Class] IN (N'H ', N'L ', N'M ')
                      ORDER BY [p].[Name]
                 */
            }
        }
    }
}
