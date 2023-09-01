using ObjectCartographer.ExpressionBuilder.Converters;
using ObjectCartographer.Tests.BaseClasses;

namespace ObjectCartographer.Tests.ExpressionBuilder.Converters
{
    /// <summary>
    /// Json element converter tests
    /// </summary>
    /// <seealso cref="TestBaseClass&lt;JsonElementConverter&gt;" />
    public class JsonElementConverterTests : TestBaseClass<JsonElementConverter>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonElementConverterTests"/> class.
        /// </summary>
        public JsonElementConverterTests()
        {
            TestObject = new JsonElementConverter();
        }
    }
}