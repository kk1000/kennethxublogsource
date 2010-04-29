namespace Spring.Transaction.Interceptor
{
    /// <summary>
    /// An <see cref="ITransactionStatusProvider"/> that obtains the current
    /// transaction status created by by declarative transaction management.
    /// </summary>
    public class TransactionInterceptorStatusProvider : ITransactionStatusProvider
    {
        /// <summary>
        /// Returns the <see cref="ITransactionStatus"/> of the current method invocation.
        /// </summary>
        /// <exception cref="NoTransactionException">
        /// If the transaction info cannot be found, because the method was
        /// invoked outside of an AOP invocation context.
        /// </exception>
        public ITransactionStatus CurrentTransactionStatus
        {
            get { return TransactionInterceptor.CurrentTransactionStatus; }
        }
    }
}
