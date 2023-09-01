using ObjectCartographer.ExpressionBuilder.Converters;
using ObjectCartographer.Tests.BaseClasses;
using System.Collections.Generic;
using System.Text.Json;
using Xunit;

namespace ObjectCartographer.Tests.ExpressionBuilder.Converters
{
    public class DefaultConverterTests : TestBaseClass<DefaultConverter>
    {
        public DefaultConverterTests()
        {
            TestObject = new DefaultConverter();
        }

        [Fact]
        public void JsonDocumentToClass()
        {
            var ObjectToSerialize = new TestClass { MyProperty = new List<SubTestClass> { new SubTestClass { MyProperty = 5 } } };
            var SerializedObject = JsonSerializer.Serialize(ObjectToSerialize);
            var Document = JsonDocument.Parse(SerializedObject);
            var ResultObject = new TestClass();
            var Result = TestObject.ConvertTo(Document, ResultObject, typeof(TestClass));
        }

        private class SubTestClass
        {
            public int MyProperty { get; set; }
        }

        private class TestClass
        {
            public List<SubTestClass> MyProperty { get; set; } = new List<SubTestClass>();
        }
    }
}