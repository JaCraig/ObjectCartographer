using ObjectCartographer.ExpressionBuilder.Converters;
using ObjectCartographer.Tests.BaseClasses;

namespace ObjectCartographer.Tests
{
    public class TypeMappingTests : TestBaseClass<TypeMapping<MyTestClass, MyTestClass2>>
    {
        public TypeMappingTests()
        {
            TestObject = new TypeMapping<MyTestClass, MyTestClass2>(
                new ObjectCartographer.Internal.TypeTuple(typeof(MyTestClass), typeof(MyTestClass2)),
                null,
                new ObjectCartographer.ExpressionBuilder.ExpressionBuilderManager(new[] { new DefaultConverter() }));
        }
    }
}