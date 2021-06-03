namespace ObjectCartographer.Interfaces
{
    /// <summary>
    /// Type mapping interface
    /// </summary>
    public interface ITypeMapping
    {
        /// <summary>
        /// Automatically maps the two types.
        /// </summary>
        /// <returns>This.</returns>
        ITypeMapping AutoMap();

        /// <summary>
        /// Builds this instance.
        /// </summary>
        void Build();
    }
}