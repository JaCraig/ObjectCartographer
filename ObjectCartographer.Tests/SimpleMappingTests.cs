using Mecha.xUnit;
using ObjectCartographer.Tests.BaseClasses;
using System;
using Xunit;

namespace ObjectCartographer.Tests
{
    public class ClassToClassMappingTests : TestBaseClass
    {
        protected override Type ObjectType { get; set; }

        [Property]
        public void Creation(MappingA mappingA)
        {
            var Result = mappingA.To<MappingB>().To<MappingA>();
            Assert.Equal(mappingA?.Item1, Result?.Item1);
            Assert.Equal(mappingA?.Item2, Result?.Item2);
        }

        public class MappingA
        {
            public static string StaticItem1 { get; set; }
            public int Item1 { get; set; }

            public string Item2 { get; set; }
        }

        public class MappingB
        {
            public int Item1 { get; set; }

            public string Item2 { get; set; }
        }
    }
}