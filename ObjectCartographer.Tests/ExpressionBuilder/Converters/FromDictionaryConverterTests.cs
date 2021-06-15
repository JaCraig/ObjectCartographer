using ObjectCartographer.ExpressionBuilder.Converters;
using ObjectCartographer.Tests.BaseClasses;

namespace ObjectCartographer.Tests.ExpressionBuilder.Converters
{
    public class FromDictionaryConverterTests : TestBaseClass<FromDictionaryConverter>
    {
        public FromDictionaryConverterTests()
        {
            TestObject = new FromDictionaryConverter();
        }
    }
}