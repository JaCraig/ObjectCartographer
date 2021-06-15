using ObjectCartographer.ExpressionBuilder.Converters;
using ObjectCartographer.Tests.BaseClasses;

namespace ObjectCartographer.Tests.ExpressionBuilder.Converters
{
    public class KeyValuePairConverterTests : TestBaseClass<KeyValuePairConverter>
    {
        public KeyValuePairConverterTests()
        {
            TestObject = new KeyValuePairConverter();
        }
    }
}