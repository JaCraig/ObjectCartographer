using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ObjectCartographer;
using System;

namespace TestApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var Collection = new ServiceCollection().AddLogging(Builder => Builder.AddConsole()).AddCanisterModules().BuildServiceProvider();
            var DataMapper = Collection.GetRequiredService<DataMapper>();
            var Mapper = DataMapper.Map<TestType1, TestType2>();
            Mapper.UseMethod(TestMethod).Build();
            var Result = DataMapper.Copy<TestType2>(new TestType1 { A = 10 });
            Console.WriteLine(Result.A);
            //Mapper.AddMapping(x => x.A, x => x.A).Build();
            DataMapper.AutoMap<TestType1, TestType2>();
        }

        private static TestType2 TestMethod(TestType1 arg1, TestType2 arg2)
        {
            arg2 ??= new TestType2();
            arg2.A = arg1.A;
            return arg2;
        }
    }
}