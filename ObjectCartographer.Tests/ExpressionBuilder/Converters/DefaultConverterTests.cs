using ObjectCartographer.ExpressionBuilder.Converters;
using ObjectCartographer.Tests.BaseClasses;

namespace ObjectCartographer.Tests.ExpressionBuilder.Converters
{
    public class DefaultConverterTests : TestBaseClass<DefaultConverter>
    {
        public DefaultConverterTests()
        {
            TestObject = new DefaultConverter();
        }
    }
}