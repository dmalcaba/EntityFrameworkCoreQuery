using EntityFrameworkCoreQuery.Queries;
using System;
using System.Threading.Tasks;

namespace EntityFrameworkCoreQuery.App;

class Program
{
    static void Main()
    {
        Console.WriteLine("Entity Framework Core Query App");

        //AggregateOperators.CountSample();
        //AggregateOperators.CountSampleBetterSqlStatement();
        //AggregateOperators.CountSampleV2();
        //AggregateOperators.GroupByHavingCount();
        //AggregateOperators.GroupByMultipleColumns();

        //JoinOperators.UsingInclude();
        //JoinOperators.UsingIncludeWithFind();
        //JoinOperators.UsingIncludeWithFirstOrDefault();
        //JoinOperators.UsingQueryMethod();
        //JoinOperators.Union();
        //JoinOperators.UnionDifferentTablesSameTypes();
        //JoinOperators.UnionDifferentStoreTypes();

        //Quantifiers.ContainsIn();
        //Quantifiers.ContainsInExample2();
        //Quantifiers.ContainsInExample3();

    }
}
