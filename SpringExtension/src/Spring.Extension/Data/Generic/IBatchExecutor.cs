namespace Spring.Data.Generic
{
    /// <summary>
    /// Interface to use <see cref="Data.IAdoOperations"/> for batch update.
    /// </summary>
    /// <author>Kenneth Xu</author>
    public interface IBatchExecutor
    {
        /// <summary>
        /// Executes batch of non queries with common command and different
        /// parameters. 
        /// </summary>
        /// <typeparam name="T">
        /// The type of the data object.
        /// </typeparam>
        /// <param name="operation">
        /// An <see cref="Spring.Data.IAdoOperations"/> object to perform
        /// database updates.
        /// </param>
        /// <param name="cmdType">
        /// The type of command.
        /// </param>
        /// <param name="cmdText">
        /// The text of command.
        /// </param>
        /// <param name="data">
        /// A collection of data object to be updated in batch.
        /// </param>
        /// <param name="dataToParameters">
        /// Delegate that converts data object to parameters.
        /// </param>
        /// <returns>
        /// The total updated count if 0 or positive. When -1 is returned,
        /// it indicates that the update count cannot be obtained due the
        /// the limitation of the batch implementation.
        /// </returns>
        int ExecuteNonQuery<T>(
            IAdoOperations operation,
            System.Data.CommandType cmdType,
            string cmdText,
            System.Collections.Generic.ICollection<T> data,
            System.Converter<T, Common.IDbParameters> dataToParameters);
    }
}