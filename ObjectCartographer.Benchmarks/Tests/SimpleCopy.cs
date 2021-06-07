using AutoMapper;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ObjectCartographer.Benchmarks
{
    [RankColumn, MemoryDiagnoser]
    public class SimpleCopy
    {
        private IMapper AutoMapperMapper { get; set; }
        private TestClass Object1 { get; } = new TestClass();
        private TestClass2 Object2 { get; } = new TestClass2();
        private DataMapper ObjectCartographerMapper { get; set; }
        private Func<object>? CachedMethod;

        [Benchmark]
        public void AutoMapper()
        {
            _ = AutoMapperMapper.Map<TestClass2>(Object1);
        }

        [Benchmark]
        public void ByHand()
        {
            Object2.A = Object1.A;
            Object2.B = Object1.B;
        }

        [Benchmark(Baseline = true)]
        public void ObjectCartographer()
        {
            _ = ObjectCartographerMapper.Copy(Object1, Object2);
        }

        [GlobalSetup]
        public void Setup()
        {
            new ServiceCollection().AddCanisterModules();
            ObjectCartographerMapper = Canister.Builder.Bootstrapper.Resolve<DataMapper>();
            ObjectCartographerMapper.AutoMap<TestClass, TestClass2>();

            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TestClass, TestClass2>();
            });
            AutoMapperMapper = configuration.CreateMapper();
        }

        private class TestClass
        {
            public string A { get; set; }

            public int B { get; set; }

            public float C { get; set; }
        }

        private class TestClass2
        {
            public string A { get; set; }

            public int B { get; set; }
        }
    }
}