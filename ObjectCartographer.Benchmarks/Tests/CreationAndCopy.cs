using AutoMapper;
using BenchmarkDotNet.Attributes;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using Nelibur.ObjectMapper;

namespace ObjectCartographer.Benchmarks.Tests
{
    [RankColumn, MemoryDiagnoser]
    public class CreationAndCopy
    {
        private IMapper AutoMapperMapper { get; set; }
        private TestClass Object1 { get; } = new TestClass();
        private DataMapper ObjectCartographerMapper { get; set; }

        [Benchmark]
        public void AutoMapper()
        {
            _ = AutoMapperMapper.Map<TestClass2>(Object1);
        }

        [Benchmark]
        public void ByHand()
        {
            _ = new TestClass2
            {
                A = Object1.A,
                B = Object1.B
            };
        }

        [Benchmark]
        public void Mapster()
        {
            _ = Object1.Adapt<TestClass2>();
        }

        [Benchmark(Baseline = true)]
        public void ObjectCartographer()
        {
            _ = ObjectCartographerMapper.Copy<TestClass2>(Object1);
        }

        [Benchmark]
        public void ObjectCartographerExtensionMethod()
        {
            _ = Object1.To<TestClass2>();
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
            });
            AutoMapperMapper = configuration.CreateMapper();
        }

        [Benchmark]
        public void TinyMapperTest()
        {
            _ = TinyMapper.Map<TestClass2>(Object1);
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