using AdventureWorks.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace EntityFrameworkCoreQuery.Queries
{
    public class JoinOperators
    {

        /// <summary>
        /// This sample uses Include/ThenInclude to Join related tables together.
        /// It can automatically determine the type of join neeed, in this case is a LEFT JOIN
        /// Adding a Take() like query.Take(10).ToList() generates an undesirable SQL statement
        /// as it creates a subquery.  
        /// 
        /// Use <see cref="UsingQueryMethod">UsingQueryMethod</see> to get better SQL statement results.
        /// </summary>
        public void UsingInclude()
        {
            using (var context = new AdventureWorksDbContext())
            {
                var query = context.Product
                    .Include(a => a.ProductSubcategory)
                        .ThenInclude(b => b.ProductCategory)
                        .AsNoTracking();

                var result = query.ToList();

                //ObjectDumper.Write(result);
            }
            /*
            info: Microsoft.EntityFrameworkCore.Database.Command[20101]
                  Executed DbCommand (31ms) [Parameters=[], CommandType='Text', CommandTimeout='30']                                      
                  SELECT [p].[ProductID], [p].[Class], [p].[Color], [p].[DaysToManufacture], [p].[DiscontinuedDate], [p].[FinishedGoodsFlag], [p].[ListPrice], [p].[MakeFlag], [p].[ModifiedDate], [p].[Name], [p].[ProductLine], [p].[ProductModelID], [p].[ProductNumber], [p].[ProductSubcategoryID], [p].[ReorderPoint], [p].[rowguid], [p].[SafetyStockLevel], [p].[SellEndDate], [p].[SellStartDate], [p].[Size], [p].[SizeUnitMeasureCode], [p].[StandardCost], [p].[Style], [p].[Weight], [p].[WeightUnitMeasureCode], [p0].[ProductSubcategoryID], [p0].[ModifiedDate], [p0].[Name], [p0].[ProductCategoryID], [p0].[rowguid], [p1].[ProductCategoryID], [p1].[ModifiedDate], [p1].[Name], [p1].[rowguid]
                  FROM [Production].[Product] AS [p]
                  LEFT JOIN [Production].[ProductSubcategory] AS [p0] ON [p].[ProductSubcategoryID] = [p0].[ProductSubcategoryID]
                  LEFT JOIN [Production].[ProductCategory] AS [p1] ON [p0].[ProductCategoryID] = [p1].[ProductCategoryID]
             */
        }

        public void UsingQueryMethod()
        {
            using (var context = new AdventureWorksDbContext())
            {
                var query = from product in context.Product
                            join productSubCat in context.ProductSubcategory
                               on product.ProductSubcategoryId equals productSubCat.ProductSubcategoryId into psc
                            from c in psc.DefaultIfEmpty()
                            join productCat in context.ProductCategory
                               on c.ProductCategoryId equals productCat.ProductCategoryId
                            select new
                            {
                                product,
                                c,
                                productCat
                            };

                var result = query.ToList();
                /*
                info: Microsoft.EntityFrameworkCore.Database.Command[20101]
                      Executed DbCommand (29ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
                      SELECT [p].[ProductID], [p].[Class], [p].[Color], [p].[DaysToManufacture], [p].[DiscontinuedDate], [p].[FinishedGoodsFlag], [p].[ListPrice], [p].[MakeFlag], [p].[ModifiedDate], [p].[Name], [p].[ProductLine], [p].[ProductModelID], [p].[ProductNumber], [p].[ProductSubcategoryID], [p].[ReorderPoint], [p].[rowguid], [p].[SafetyStockLevel], [p].[SellEndDate], [p].[SellStartDate], [p].[Size], [p].[SizeUnitMeasureCode], [p].[StandardCost], [p].[Style], [p].[Weight], [p].[WeightUnitMeasureCode], [p0].[ProductSubcategoryID], [p0].[ModifiedDate], [p0].[Name], [p0].[ProductCategoryID], [p0].[rowguid], [p1].[ProductCategoryID], [p1].[ModifiedDate], [p1].[Name], [p1].[rowguid]
                      FROM [Production].[Product] AS [p]
                      LEFT JOIN [Production].[ProductSubcategory] AS [p0] ON [p].[ProductSubcategoryID] = [p0].[ProductSubcategoryID]
                      INNER JOIN [Production].[ProductCategory] AS [p1] ON [p0].[ProductCategoryID] = [p1].[ProductCategoryID]             
                 */

                var topResult = query.Take(10).ToList();
                ObjectDumper.Write(topResult);

                /*
                info: Microsoft.EntityFrameworkCore.Database.Command[20101]
                      Executed DbCommand (42ms) [Parameters=[@__p_0='?' (DbType = Int32)], CommandType='Text', CommandTimeout='30']
                      SELECT TOP(@__p_0) [p].[ProductID], [p].[Class], [p].[Color], [p].[DaysToManufacture], [p].[DiscontinuedDate], [p].[FinishedGoodsFlag], [p].[ListPrice], [p].[MakeFlag], [p].[ModifiedDate], [p].[Name], [p].[ProductLine], [p].[ProductModelID], [p].[ProductNumber], [p].[ProductSubcategoryID], [p].[ReorderPoint], [p].[rowguid], [p].[SafetyStockLevel], [p].[SellEndDate], [p].[SellStartDate], [p].[Size], [p].[SizeUnitMeasureCode], [p].[StandardCost], [p].[Style], [p].[Weight], [p].[WeightUnitMeasureCode], [p0].[ProductSubcategoryID], [p0].[ModifiedDate], [p0].[Name], [p0].[ProductCategoryID], [p0].[rowguid], [p1].[ProductCategoryID], [p1].[ModifiedDate], [p1].[Name], [p1].[rowguid]
                      FROM [Production].[Product] AS [p]
                      LEFT JOIN [Production].[ProductSubcategory] AS [p0] ON [p].[ProductSubcategoryID] = [p0].[ProductSubcategoryID]
                      INNER JOIN [Production].[ProductCategory] AS [p1] ON [p0].[ProductCategoryID] = [p1].[ProductCategoryID]                 
                      */
            }
        }

        public void Union()
        {
            using (var context = new AdventureWorksDbContext())
            {
                var countryC = context.CountryRegion.Where(x => x.Name.StartsWith("C")).AsQueryable();
                var countryP = context.CountryRegion.Where(x => x.Name.StartsWith("P")).AsQueryable();

                // UNION
                var result = countryC.Union(countryP).ToList();

                // UNION ALL
                var result2 = countryC.Concat(countryP).ToList();

                ObjectDumper.Write(result);
                /*
                info: Microsoft.EntityFrameworkCore.Database.Command[20101]
                      Executed DbCommand (156ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
                      SELECT [c].[CountryRegionCode], [c].[ModifiedDate], [c].[Name]
                      FROM [Person].[CountryRegion] AS [c]
                      WHERE [c].[Name] LIKE N'C%'
                      UNION
                      SELECT [c0].[CountryRegionCode], [c0].[ModifiedDate], [c0].[Name]
                      FROM [Person].[CountryRegion] AS [c0]
                      WHERE [c0].[Name] LIKE N'P%'
                info: Microsoft.EntityFrameworkCore.Database.Command[20101]
                      Executed DbCommand (21ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
                      SELECT [c].[CountryRegionCode], [c].[ModifiedDate], [c].[Name]
                      FROM [Person].[CountryRegion] AS [c]
                      WHERE [c].[Name] LIKE N'C%'
                      UNION ALL
                      SELECT [c0].[CountryRegionCode], [c0].[ModifiedDate], [c0].[Name]
                      FROM [Person].[CountryRegion] AS [c0]
                      WHERE [c0].[Name] LIKE N'P%'
                 */

            }
        }

        public void UnionDifferentTablesSameTypes()
        {
            using (var context = new AdventureWorksDbContext())
            {
                //Name is NVARCHAR(50)
                var shipMethod = context.ShipMethod.Select(x => new Item { Id = x.ShipMethodId, Name = x.Name }).AsQueryable();

                //Name is NVARCHAR(50)
                var productCat = context.ProductCategory.Select(x => new Item { Id = x.ProductCategoryId, Name = x.Name }).AsQueryable();

                var query = shipMethod.Concat(productCat);

                var result3 = query.OrderBy(x => x.Name).Skip(1).Take(10).ToList();

                /* Generates:
                 * 
                info: Microsoft.EntityFrameworkCore.Database.Command[20101]
                      Executed DbCommand (47ms) [Parameters=[@__p_0='?' (DbType = Int32), @__p_1='?' (DbType = Int32)], CommandType='Text', CommandTimeout='30']
                      SELECT [t].[Id], [t].[Name]
                      FROM (
                          SELECT [s].[ShipMethodID] AS [Id], [s].[Name]
                          FROM [Purchasing].[ShipMethod] AS [s]
                          UNION ALL
                          SELECT [p].[ProductCategoryID] AS [Id], [p].[Name]
                          FROM [Production].[ProductCategory] AS [p]
                      ) AS [t]
                      ORDER BY [t].[Name]
                      OFFSET @__p_0 ROWS FETCH NEXT @__p_1 ROWS ONLY
                 */

                //
                // Syntax method, same SQL statement generated
                //

                //Name is NVARCHAR(50)
                var shipMeth = from ship in context.ShipMethod
                               select new Item { Id = ship.ShipMethodId, Name = ship.Name };

                //Name is NVARCHAR(50)
                var productCateg = from prodcat in context.ProductCategory
                                   select new Item { Id = prodcat.ProductCategoryId, Name = prodcat.Name };

                var query4 = shipMeth.Union(productCateg);

                var result4 = query4.OrderBy(x => x.Name).Skip(1).Take(10).ToList();

                /*
                info: Microsoft.EntityFrameworkCore.Database.Command[20101]
                      Executed DbCommand (37ms) [Parameters=[@__p_0='?' (DbType = Int32), @__p_1='?' (DbType = Int32)], CommandType='Text', CommandTimeout='30']
                      SELECT [t].[Id], [t].[Name]
                      FROM (
                          SELECT [s].[ShipMethodID] AS [Id], [s].[Name]
                          FROM [Purchasing].[ShipMethod] AS [s]
                          UNION ALL
                          SELECT [p].[ProductCategoryID] AS [Id], [p].[Name]
                          FROM [Production].[ProductCategory] AS [p]
                      ) AS [t]
                      ORDER BY [t].[Name]
                      OFFSET @__p_0 ROWS FETCH NEXT @__p_1 ROWS ONLY
                 */
            }
        }



        public class Item
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        /// <summary>
        /// This doesn't work; EF cannot translate Description and Name since they have different length
        /// </summary>
        public void UnionDifferentStoreTypes()
        {
            using (var context = new AdventureWorksDbContext())
            {
                //Description is NVARCHAR(255)
                var specialOff = from offer in context.SpecialOffer
                                 select new Item { Id = offer.SpecialOfferId, Name = "'" + offer.DiscountPct.ToString() + "." };

                //Name is NVARCHAR(50)
                var productCat = from prodcat in context.ProductCategory
                                 select new Item { Id = prodcat.ProductCategoryId, Name = "'" + prodcat.Name + "." };

                var query = specialOff.Union(productCat);

                var result3 = query.OrderBy(x => x.Name).Skip(1).Take(10).ToList();

                /* Generates:
                 * 
                info: Microsoft.EntityFrameworkCore.Database.Command[20101]
                      Executed DbCommand (5ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
                      SELECT [s].[ShipMethodID] AS [Id], [s].[Name]
                      FROM [Purchasing].[ShipMethod] AS [s]
                      UNION ALL
                      SELECT [p].[ProductCategoryID] AS [Id], [p].[Name]
                      FROM [Production].[ProductCategory] AS [p]
                 */
            }
        }

        public void RelatedData()
        {
            //using (var context = new AdventureWorksDbContext())
            //{
            //    var stateProv = context.StateProvince.ToList();
            //    var result = context.Address.ToList();
            //}

            //using (var context = new AdventureWorksDbContext())
            //{
            //    var result = context.Address.ToList();
            //}

            using (var context = new AdventureWorksDbContext())
            {
                var result = context.Address
                    .Where(a => a.StateProvince.StateProvinceCode == "WA")
                    .ToList();
            }

        }
        
    }
}
