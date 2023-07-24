using ObjectCartographer.Tests.BaseClasses;
using System;
using System.Dynamic;
using System.Text.Json;
using Xunit;

namespace ObjectCartographer.Tests
{
    public class SystemTextJsonTests : TestBaseClass
    {
        protected override Type ObjectType { get; set; } = null;

        [Fact]
        public void TestSerializedData()
        {
            var Data = JsonSerializer.Serialize(new TestClass { A = "This is a test", B = 10, C = true, D = 12.5555, E = new[] { 5, 4, 3, 2, 1 } });
            var Result = JsonSerializer.Deserialize<ExpandoObject>(Data);
            var FinalData = Result.To<TestClass>();
            Assert.Equal("This is a test", FinalData.A);
            Assert.Equal(10, FinalData.B);
            Assert.True(FinalData.C);
            Assert.Equal(12.5555, FinalData.D);
            Assert.Equal(new[] { 5, 4, 3, 2, 1 }, FinalData.E);
        }

        private class TestClass
        {
            public string A { get; set; }
            public int B { get; set; }
            public bool C { get; set; }
            public double D { get; set; }

            public int[] E { get; set; }
        }
    }
}