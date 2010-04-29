namespace Spring.Transaction
{
    /// <summary>
    /// An interface to query for the current transaction status.
    /// mainly intended for code that wants to set the current transaction
    /// rollback-only but not throw an application exception.
    /// </summary>
    /// <remarks>
    /// This is useful to make your unit testable by replacing
    /// <code>TransactionInterceptor.CurrentTransactionStatus.SetRollbackOnly();</code>
    /// with an injectable <see cref="ITransactionStatusProvider"/> instance.
    /// </remarks>
    public interface ITransactionStatusProvider
    {
        /// <summary>
        /// Returns the <see cref="ITransactionStatus"/> of the current method invocation.
        /// </summary>
        /// <exception cref="NoTransactionException">
        /// If the transaction info cannot be found.
        /// </exception>
        ITransactionStatus CurrentTransactionStatus { get; }
    }
}
