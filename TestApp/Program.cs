using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ObjectCartographer;
using System;
using System.Collections.Generic;
using System.Data;
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
            //Mapper.UseMethod(TestMethod).Build();
            //var Result = DataMapper.Copy<TestType2>(new TestType1 { A = 10 });
            //Console.WriteLine(Result.A);
            //var Result2 = DataMapper.Copy(new TestType1 { A = 20 }, new TestType2());
            //Console.WriteLine(Result2.A);
            ////Mapper.AddMapping(x => x.A, x => x.A).Build();
            //DataMapper.AutoMap<TestType1, TestType2>();

            //var Result3 = DataMapper.Copy(new TestType2 { A = 30 }, new TestType1());
            //Console.WriteLine(Result3.A);

            //DataMapper.AutoMap<ExpandoObject, Dictionary<string, object>>();
            //var TempDictionary = new Dictionary<string, object>();
            //TempDictionary["A"] = 55;
            //var ExpandoResult = (IDictionary<string, object>)DataMapper.Copy<ExpandoObject>(TempDictionary);
            //Console.WriteLine(ExpandoResult["A"]);

            KeyValuePair<int, int> A = new KeyValuePair<int, int>(1, 2);
            var Temp = DataMapper.Copy<KeyValuePair<string, object>>(A);
            Console.WriteLine(Temp.Key);
            Console.WriteLine(Temp.Value);
            A = DataMapper.Copy<KeyValuePair<int, int>>(Temp);
            Console.WriteLine(A.Key);
            Console.WriteLine(A.Value);

            Console.WriteLine(DataMapper.Copy<string>(DataMapper.Copy<byte[]>("ASDf")));

            dynamic ExpandoResult2 = new ExpandoObject();
            ExpandoResult2.A = 55;
            var FinalResult = DataMapper.Copy<Dictionary<string, object>>(ExpandoResult2);
            Console.WriteLine(FinalResult["A"]);
            var ExpandoResult = (IDictionary<string, object>)DataMapper.Copy<ExpandoObject>(FinalResult);
            Console.WriteLine(ExpandoResult["A"]);
            var TestType1Result = DataMapper.Copy<TestType1>(ExpandoResult);
            Console.WriteLine(TestType1Result.A);

            DataMapper.AutoMap<TestType2, TestType1>();
            var Val = new TestType1
            {
                A = 100,
                B = 40.1f,
                C = 1,
                D = "1/1/2020",
                E = MyEnum.Option3,
                F = "0:0:1",
                G = new TestType3() { A = 11 }
            };
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

            Console.WriteLine();
            ExpandoResult = (IDictionary<string, object>)DataMapper.Copy<ExpandoObject>(Val);
            Console.WriteLine(ExpandoResult["A"]);
            Console.WriteLine(ExpandoResult["B"]);
            Console.WriteLine(ExpandoResult["C"]);
            Console.WriteLine(ExpandoResult["D"]);
            Console.WriteLine(ExpandoResult["E"]);
            Console.WriteLine(ExpandoResult["F"]);

            var ArrayTemp = new TestType1[3] { new TestType1(), null, null };
            var Result = DataMapper.Copy<TestType2[]>(ArrayTemp);
            Console.WriteLine(Result[0].A);
            Console.WriteLine(Result[0].B);
            Console.WriteLine(Result[0].C);
            Console.WriteLine(Result[0].D);
            Console.WriteLine(Result[0].E);
            Console.WriteLine(Result[0].F);
            Console.WriteLine(Result[0].G);

            Console.WriteLine(DataMapper.Copy<SqlDbType>(DbType.String));
            Console.WriteLine(DataMapper.Copy<Type>(DataMapper.Copy<SqlDbType>(DbType.String)));

            Result4 = Val.To<TestType2>();
            Console.WriteLine(Result4.A);
            Console.WriteLine(Result4.B);
            Console.WriteLine(Result4.C);
            Console.WriteLine(Result4.D);
            Console.WriteLine(Result4.E);
            Console.WriteLine(Result4.F);
            Console.WriteLine(Result4.G.A);

            Result4 = (TestType2)Val.To(typeof(TestType2), null);
            Console.WriteLine(Result4.A);
            Console.WriteLine(Result4.B);
            Console.WriteLine(Result4.C);
            Console.WriteLine(Result4.D);
            Console.WriteLine(Result4.E);
            Console.WriteLine(Result4.F);
            Console.WriteLine(Result4.G.A);
        }

        private static TestType2 TestMethod(TestType1 arg1, TestType2 arg2)
        {
            arg2 ??= new TestType2(10);
            arg2.A = (int)arg1.A;
            return arg2;
        }
    }
}