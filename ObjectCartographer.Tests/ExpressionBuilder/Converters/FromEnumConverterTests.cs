using ObjectCartographer.ExpressionBuilder.Converters;
using ObjectCartographer.Tests.BaseClasses;

namespace ObjectCartographer.Tests.ExpressionBuilder.Converters
{
    public class FromEnumConverterTests : TestBaseClass<FromEnumConverter>
    {
        public FromEnumConverterTests()
        {
            TestObject = new FromEnumConverter();
        }
    }
}