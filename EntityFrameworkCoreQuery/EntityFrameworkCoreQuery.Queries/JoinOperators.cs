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
    }
}
