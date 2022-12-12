using Microsoft.Extensions.DependencyInjection;
using ObjectCartographer.Tests.BaseClasses;

namespace ObjectCartographer.Tests
{
    public class DataMapperTests : TestBaseClass<DataMapper>
    {
        public DataMapperTests()
        {
            var Provider = new ServiceCollection().AddCanisterModules()?.BuildServiceProvider();
            TestObject = Provider?.GetService<DataMapper>();
        }
    }
}