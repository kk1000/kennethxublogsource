#region License

/*
 * Copyright (C) 2009 the original author or authors.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#endregion

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
