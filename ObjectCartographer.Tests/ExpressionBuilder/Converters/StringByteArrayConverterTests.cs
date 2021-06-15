using ObjectCartographer.ExpressionBuilder.Converters;
using ObjectCartographer.Tests.BaseClasses;

namespace ObjectCartographer.Tests.ExpressionBuilder.Converters
{
    public class StringByteArrayConverterTests : TestBaseClass<StringByteArrayConverter>
    {
        public StringByteArrayConverterTests()
        {
            TestObject = new StringByteArrayConverter();
        }
    }
}