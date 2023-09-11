using ObjectCartographer.Tests.BaseClasses;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text.Json;
using Xunit;

namespace ObjectCartographer.Tests
{
    public class SystemTextJsonTests : TestBaseClass
    {
        protected override Type ObjectType { get; set; } = null;

        [Fact]
        public void JsonDocumentToClass()
        {
            var Data = JsonSerializer.Serialize(new TestClass
            {
                A = "This is a test",
                B = 10,
                C = true,
                D = 12.5555,
                E = new[] { 5, 4, 3, 2, 1 },
                MyProperty = new List<SubTestClass> { new SubTestClass { MyProperty = 5 } },
                MyProperty2 = new List<SubTestClass2> { new SubTestClass2 { MyProperty = 5, Value = "Test" } }
            });
            var Document = JsonDocument.Parse(Data);
            var ResultObject = new TestClass();
            var FinalData = Document.To(ResultObject);
            Assert.Equal("This is a test", FinalData.A);
            Assert.Equal(10, FinalData.B);
            Assert.True(FinalData.C);
            Assert.Equal(12.5555, FinalData.D);
            Assert.Equal(new[] { 5, 4, 3, 2, 1 }, FinalData.E);
            Assert.Single(FinalData.MyProperty);
            Assert.Equal(5, FinalData.MyProperty[0].MyProperty);
            Assert.Single(FinalData.MyProperty2);
            Assert.Equal(5, FinalData.MyProperty2[0].MyProperty);
            Assert.Equal("Test", FinalData.MyProperty2[0].Value);
        }

        [Fact]
        public void TestSerializedData()
        {
            var Data = JsonSerializer.Serialize(new TestClass
            {
                A = "This is a test",
                B = 10,
                C = true,
                D = 12.5555,
                E = new[] { 5, 4, 3, 2, 1 },
                MyProperty = new List<SubTestClass> { new SubTestClass { MyProperty = 5 } },
                MyProperty2 = new List<SubTestClass2> { new SubTestClass2 { MyProperty = 5, Value = "Test" } }
            });
            var Result = JsonSerializer.Deserialize<ExpandoObject>(Data);
            var FinalData = Result.To<TestClass>();
            Assert.Equal("This is a test", FinalData.A);
            Assert.Equal(10, FinalData.B);
            Assert.True(FinalData.C);
            Assert.Equal(12.5555, FinalData.D);
            Assert.Equal(new[] { 5, 4, 3, 2, 1 }, FinalData.E);
            Assert.Single(FinalData.MyProperty);
            Assert.Equal(5, FinalData.MyProperty[0].MyProperty);
            Assert.Single(FinalData.MyProperty2);
            Assert.Equal(5, FinalData.MyProperty2[0].MyProperty);
            Assert.Equal("Test", FinalData.MyProperty2[0].Value);
        }

        private class SubTestClass
        {
            public int MyProperty { get; set; }
        }

        private class SubTestClass2
        {
            public long? MyProperty { get; set; }

            public string Value { get; set; }
        }

        private class TestClass
        {
            public string A { get; set; }
            public int B { get; set; }
            public bool C { get; set; }
            public double D { get; set; }

            public int[] E { get; set; }

            public List<SubTestClass> MyProperty { get; set; } = new List<SubTestClass>();

            public List<SubTestClass2> MyProperty2 { get; set; } = new List<SubTestClass2>();
        }
    }
}