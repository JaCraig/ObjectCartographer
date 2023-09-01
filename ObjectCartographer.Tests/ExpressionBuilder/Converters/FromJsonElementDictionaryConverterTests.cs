using ObjectCartographer.ExpressionBuilder.Converters;
using ObjectCartographer.Tests.BaseClasses;

namespace ObjectCartographer.Tests.ExpressionBuilder.Converters
{
    /// <summary>
    /// From json element dictionary converter tests
    /// </summary>
    /// <seealso cref="TestBaseClass&lt;FromJsonElementDictionaryConverter&gt;" />
    public class FromJsonElementDictionaryConverterTests : TestBaseClass<FromJsonElementDictionaryConverter>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FromJsonElementDictionaryConverterTests"/> class.
        /// </summary>
        public FromJsonElementDictionaryConverterTests()
        {
            TestObject = new FromJsonElementDictionaryConverter();
        }
    }
}