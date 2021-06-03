using ObjectCartographer.ExpressionBuilder.Interfaces;
using System.Collections.Generic;

namespace ObjectCartographer.ExpressionBuilder
{
    /// <summary>
    /// Expression builder manager
    /// </summary>
    public class ExpressionBuilderManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionBuilderManager"/> class.
        /// </summary>
        /// <param name="builders">The builders.</param>
        public ExpressionBuilderManager(IEnumerable<IExpressionBuilder> builders)
        {
            Builders = builders;
        }

        /// <summary>
        /// Gets the builders.
        /// </summary>
        /// <value>The builders.</value>
        private IEnumerable<IExpressionBuilder> Builders { get; }

        //public
    }
}