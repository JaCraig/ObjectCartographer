using ObjectCartographer.Tests.BaseClasses;

namespace ObjectCartographer.Tests
{
    public class DataMapperTests : TestBaseClass<DataMapper>
    {
        public DataMapperTests()
        {
            TestObject = Canister.Builder.Bootstrapper.Resolve<DataMapper>();
        }
    }
}