using ObjectCartographer.ExpressionBuilder;
using ObjectCartographer.ExpressionBuilder.Converters;
using ObjectCartographer.Tests.BaseClasses;

namespace ObjectCartographer.Tests.ExpressionBuilder
{
    public class ExpressionBuilderManagerTests : TestBaseClass<ExpressionBuilderManager>
    {
        public ExpressionBuilderManagerTests()
        {
            TestObject = new ExpressionBuilderManager(new[] { new DefaultConverter() });
        }
    }
}