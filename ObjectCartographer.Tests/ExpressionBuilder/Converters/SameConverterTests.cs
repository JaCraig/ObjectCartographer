using ObjectCartographer.ExpressionBuilder.Converters;
using ObjectCartographer.Tests.BaseClasses;

namespace ObjectCartographer.Tests.ExpressionBuilder.Converters
{
    public class SameConverterTests : TestBaseClass<SameConverter>
    {
        public SameConverterTests()
        {
            TestObject = new SameConverter();
        }
    }
}