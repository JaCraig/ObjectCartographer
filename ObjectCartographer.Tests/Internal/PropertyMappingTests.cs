using ObjectCartographer.Internal;
using ObjectCartographer.Tests.BaseClasses;
using System;

namespace ObjectCartographer.Tests.Internal
{
    public class PropertyMappingTests : TestBaseClass
    {
        protected override Type ObjectType { get; set; } = typeof(PropertyMapping<MyTestClass, MyTestClass2>);
    }
}