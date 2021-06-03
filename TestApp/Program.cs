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
            DataMapper.AutoMap<TestType1, TestType2>();
            DataMapper.AutoMap<TestType1, TestType2>();
        }
    }
}