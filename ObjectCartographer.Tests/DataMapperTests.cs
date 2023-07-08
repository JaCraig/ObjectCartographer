using Microsoft.Extensions.DependencyInjection;
using ObjectCartographer.Tests.BaseClasses;
using Xunit;

namespace ObjectCartographer.Tests
{
    public class DataMapperTests : TestBaseClass<DataMapper>
    {
        public DataMapperTests()
        {
            ServiceProvider? Provider = new ServiceCollection().AddCanisterModules()?.BuildServiceProvider();
            TestObject = Provider?.GetService<DataMapper>();
        }

        [Fact]
        public void CopyTest()
        {
            var Result = TestObject.Copy<byte[]>("Example");
            Assert.NotNull(Result);
            Assert.Equal(7, Result.Length);
            Assert.Equal("Example", TestObject.Copy<string>(Result));
        }
    }
}