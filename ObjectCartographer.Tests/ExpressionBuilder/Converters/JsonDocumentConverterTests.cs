using ObjectCartographer.ExpressionBuilder.Converters;
using ObjectCartographer.Tests.BaseClasses;

namespace ObjectCartographer.Tests.ExpressionBuilder.Converters
{
    /// <summary>
    /// Json document converter tests
    /// </summary>
    /// <seealso cref="TestBaseClass&lt;JsonDocumentConverter&gt;" />
    public class JsonDocumentConverterTests : TestBaseClass<JsonDocumentConverter>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonDocumentConverterTests"/> class.
        /// </summary>
        public JsonDocumentConverterTests()
        {
            TestObject = new JsonDocumentConverter();
        }
    }
}