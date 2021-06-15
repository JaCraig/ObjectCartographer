using ObjectCartographer.ExpressionBuilder.Converters;
using ObjectCartographer.Tests.BaseClasses;

namespace ObjectCartographer.Tests.ExpressionBuilder.Converters
{
    public class StringParsingConverterTests : TestBaseClass<StringParsingConverter>
    {
        public StringParsingConverterTests()
        {
            TestObject = new StringParsingConverter();
        }
    }
}