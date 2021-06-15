using ObjectCartographer.ExpressionBuilder.Converters;
using ObjectCartographer.Tests.BaseClasses;

namespace ObjectCartographer.Tests.ExpressionBuilder.Converters
{
    public class ClassToClassConverterTests : TestBaseClass<ClassToClassConverter>
    {
        public ClassToClassConverterTests()
        {
            TestObject = new ClassToClassConverter();
        }
    }
}