using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ObjectCartographer;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace TestApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var Collection = new ServiceCollection().AddLogging(Builder => Builder.AddConsole()).AddCanisterModules().BuildServiceProvider();
            var DataMapper = Collection.GetRequiredService<DataMapper>();
            //var Mapper = DataMapper.Map<TestType1, TestType2>();
            //Mapper.AddMapping(x => x.A, x => x.A);
            ////Mapper.UseMethod(TestMethod).Build();
            //var Result = DataMapper.Copy<TestType2>(new TestType1 { A = 10 });
            //Console.WriteLine(Result.A);
            //var Result2 = DataMapper.Copy(new TestType1 { A = 20 }, new TestType2());
            //Console.WriteLine(Result2.A);
            ////Mapper.AddMapping(x => x.A, x => x.A).Build();
            //DataMapper.AutoMap<TestType1, TestType2>();

            //var Result3 = DataMapper.Copy(new TestType2 { A = 30 }, new TestType1());
            //Console.WriteLine(Result3.A);

            DataMapper.AutoMap<ExpandoObject, Dictionary<string, object>>();
            var TempDictionary = new Dictionary<string, object>();
            TempDictionary["A"] = 55;
            var ExpandoResult = (IDictionary<string, object>)DataMapper.Copy<ExpandoObject>(TempDictionary);
            Console.WriteLine(ExpandoResult["A"]);

            DataMapper.AutoMap<TestType2, TestType1>();
            var Val = new TestType1();
            Val.A = 100;
            Val.B = 40.1f;
            Val.C = 1;
            Val.D = "1/1/2020";
            Val.E = MyEnum.Option3;
            Val.F = "0:0:1";
            Val.G = new TestType3() { A = 11 };
            var Result4 = DataMapper.Copy<TestType2>(Val);
            Console.WriteLine(Result4.A);
            Console.WriteLine(Result4.B);
            Console.WriteLine(Result4.C);
            Console.WriteLine(Result4.D);
            Console.WriteLine(Result4.E);
            Console.WriteLine(Result4.F);
            Console.WriteLine(Result4.G.A);

            Console.WriteLine();
            Val = DataMapper.Copy<TestType1>(Result4);
            Console.WriteLine(Val.A);
            Console.WriteLine(Val.B);
            Console.WriteLine(Val.C);
            Console.WriteLine(Val.D);
            Console.WriteLine(Val.E);
            Console.WriteLine(Val.F);
            Console.WriteLine(Val.G.A);
        }

        private static TestType2 TestMethod(TestType1 arg1, TestType2 arg2)
        {
            arg2 ??= new TestType2(10);
            arg2.A = (int)arg1.A;
            return arg2;
        }
    }
}