using ObjectCartographer.ExpressionBuilder.Converters;
using ObjectCartographer.Tests.BaseClasses;

namespace ObjectCartographer.Tests.ExpressionBuilder.Converters
{
    public class ToStringConverterTests : TestBaseClass<ToStringConverter>
    {
        public ToStringConverterTests()
        {
            TestObject = new ToStringConverter();
        }
    }
}