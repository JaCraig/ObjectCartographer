using Microsoft.Extensions.DependencyInjection;
using ObjectCartographer;

namespace TestApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var Collection = new ServiceCollection().AddCanisterModules().BuildServiceProvider();
            var DataMapper = Collection.GetRequiredService<DataMapper>();
            var Mapper = DataMapper.Map<TestType1, TestType2>();
            Mapper.AddMapping(x => x.A, x => x.A).Build();
            DataMapper.AutoMap<TestType1, TestType2>();
        }
    }
}