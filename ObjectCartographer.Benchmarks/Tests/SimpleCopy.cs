using AutoMapper;
using BenchmarkDotNet.Attributes;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using Nelibur.ObjectMapper;

namespace ObjectCartographer.Benchmarks
{
    [RankColumn, MemoryDiagnoser]
    public class SimpleCopy
    {
        private IMapper AutoMapperMapper { get; set; }
        private TestClass Object1 { get; } = new TestClass();
        private TestClass2 Object2 { get; } = new TestClass2();
        private DataMapper ObjectCartographerMapper { get; set; }

        [Benchmark]
        public void AutoMapper()
        {
            _ = AutoMapperMapper.Map(Object1, Object2);
        }

        [Benchmark]
        public void ByHand()
        {
            Object2.A = Object1.A;
            Object2.B = Object1.B;
        }

        [Benchmark]
        public void Mapster()
        {
            _ = Object1.Adapt<TestClass2>();
        }

        [Benchmark(Baseline = true)]
        public void ObjectCartographer()
        {
            _ = ObjectCartographerMapper.Copy(Object1, Object2);
        }

        [GlobalSetup]
        public void Setup()
        {
            TinyMapper.Bind<TestClass, TestClass2>();
            var Provider = new ServiceCollection().AddCanisterModules().BuildServiceProvider();
            ObjectCartographerMapper = Provider.GetService<DataMapper>();
            ObjectCartographerMapper.AutoMap<TestClass, TestClass2>();

            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TestClass, TestClass2>();
            }, null);
            AutoMapperMapper = configuration.CreateMapper();
        }

        [Benchmark]
        public void TinyMapperTest()
        {
            _ = TinyMapper.Map(Object1, Object2);
        }

        public class TestClass
        {
            public string A { get; set; }

            public int B { get; set; }

            public float C { get; set; }
        }

        public class TestClass2
        {
            public string A { get; set; }

            public int B { get; set; }
        }
    }
}