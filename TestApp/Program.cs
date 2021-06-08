using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ObjectCartographer;
using System;
using System.Dynamic;

namespace TestApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var Collection = new ServiceCollection().AddLogging(Builder => Builder.AddConsole()).AddCanisterModules().BuildServiceProvider();
            var DataMapper = Collection.GetRequiredService<DataMapper>();
            var Mapper = DataMapper.Map<TestType1, TestType2>();
            Mapper.AddMapping(x => x.A, x => x.A);
            //Mapper.UseMethod(TestMethod).Build();
            var Result = DataMapper.Copy<TestType2>(new TestType1 { A = 10 });
            Console.WriteLine(Result.A);
            var Result2 = DataMapper.Copy(new TestType1 { A = 20 }, new TestType2());
            Console.WriteLine(Result2.A);
            //Mapper.AddMapping(x => x.A, x => x.A).Build();
            DataMapper.AutoMap<TestType1, TestType2>();

            var Result3 = DataMapper.Copy(new TestType2 { A = 30 }, new TestType1());
            Console.WriteLine(Result3.A);

            DataMapper.AutoMap<ExpandoObject, ExpandoObject>();
            dynamic Val = new ExpandoObject();
            Val.A = 40;
            var Result4 = DataMapper.Copy<ExpandoObject>(Val);
            Console.WriteLine(Result4.A);
        }

        private static TestType2 TestMethod(TestType1 arg1, TestType2 arg2)
        {
            arg2 ??= new TestType2(10);
            arg2.A = (int)arg1.A;
            return arg2;
        }
    }
}