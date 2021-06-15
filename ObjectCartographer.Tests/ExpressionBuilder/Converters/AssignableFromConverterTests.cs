using ObjectCartographer.ExpressionBuilder.Converters;
using ObjectCartographer.Tests.BaseClasses;

namespace ObjectCartographer.Tests.ExpressionBuilder.Converters
{
    public class AssignableFromConverterTests : TestBaseClass<AssignableFromConverter>
    {
        public AssignableFromConverterTests()
        {
            TestObject = new AssignableFromConverter();
        }
    }
}