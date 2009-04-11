namespace Spring.Data.Generic
{
    /// <summary>
    /// Factory for <see cref="IBatchExecutor"/>.
    /// </summary>
    /// <author>Kenneth Xu</author>
    public interface IBatchExecutorFactory
    {
        /// <summary>
        /// Get an instance of <see cref="IBatchExecutor"/>.
        /// </summary>
        /// <returns>An instance of <see cref="IBatchExecutor"/>.</returns>
        IBatchExecutor GetExecutor();
    }
}