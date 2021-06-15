using ObjectCartographer.ExpressionBuilder.Converters;
using ObjectCartographer.Tests.BaseClasses;

namespace ObjectCartographer.Tests.ExpressionBuilder.Converters
{
    public class EnumConverterTests : TestBaseClass<EnumConverter>
    {
        public EnumConverterTests()
        {
            TestObject = new EnumConverter();
        }
    }
}