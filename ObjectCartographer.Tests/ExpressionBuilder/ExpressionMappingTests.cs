using ObjectCartographer.ExpressionBuilder;
using ObjectCartographer.Tests.BaseClasses;
using System.Dynamic;

namespace ObjectCartographer.Tests.ExpressionBuilder
{
    public class ExpressionMappingTests : TestBaseClass<ExpressionMapping<ExpandoObject, ExpandoObject>>
    {
        public ExpressionMappingTests()
        {
            TestObject = new ExpressionMapping<ExpandoObject, ExpandoObject>();
        }
    }
}