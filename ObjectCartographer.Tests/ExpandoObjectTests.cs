using ObjectCartographer.Tests.BaseClasses;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text.Json;
using Xunit;

namespace ObjectCartographer.Tests
{
    public class ExpandoObjectTests : TestBaseClass
    {
        protected override Type? ObjectType { get; set; } = null;

        [Fact]
        public void ConvertFrom()
        {
            IDictionary<string, object> Result = new TestClass { A = "This is a test", B = 10, C = new Uri("http://B") }.To<ExpandoObject>();
            Assert.Equal(10, Result["B"]);
            Assert.Equal("This is a test", Result["A"]);
            Assert.Equal(new Uri("http://B"), Result["C"]);
        }

        [Fact]
        public void ConvertTo()
        {
            IDictionary<string, object> TestObject = new ExpandoObject();
            TestObject["A"] = "This is a test";
            TestObject["B"] = 10;
            TestObject["C"] = "http://a";
            TestClass Result = TestObject.To<TestClass>();
            Assert.Equal(10, Result.B);
            Assert.Equal("This is a test", Result.A);
            Assert.Equal("http://a/", Result.C.ToString());
        }

        [Fact]
        public void SerializationTest()
        {
            ExpandoObject? DeserializedData = JsonSerializer.Deserialize<ExpandoObject>(JsonSerializer.Serialize(new TestClass22
            {
                A = MyTestEnum.Option2,
                B = 10,
                C = new Uri("http://B"),
                D = "This is a test",
                E = "http://a"
            }));
            TestClass22 Final = DeserializedData.To<TestClass22>();

            Assert.Equal(MyTestEnum.Option2, Final.A);
            Assert.Equal(10, Final.B);
            Assert.Equal(new Uri("http://B"), Final.C);
            Assert.Equal("This is a test", Final.D);
            Assert.Equal("http://a", Final.E);
        }

        public class TestClass
        {
            public string? A { get; set; }

            public int B { get; set; }

            public Uri? C { get; set; }
        }
    }

    public class TestClass22
    {
        public MyTestEnum A { get; set; }
        public int B { get; set; }

        public Uri? C { get; set; }

        public string? D { get; set; }

        public string? E { get; set; }
    }

    public enum MyTestEnum
    {
        Option1,
        Option2,
        Option3
    }
}