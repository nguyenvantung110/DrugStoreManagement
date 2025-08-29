using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace drug_store_api.systemcommon.Transaction
{
    /// <summary>
    /// TransactionScopeManager.
    /// </summary>
    public static class TransactionScopeManager
    {
        private static readonly TransactionOptions DefaultTransactionOptions = new() { IsolationLevel = IsolationLevel.ReadCommitted };

        /// <summary>
        /// CreateTransactionScope.
        /// </summary>
        /// <param name="scopeOption">scopeOption.</param>
        /// <param name="transactionOptions">transactionOptions.</param>
        /// <param name="asyncFlowOption">asyncFlowOption.</param>
        /// <returns>TransactionScope.</returns>
        public static TransactionScope CreateTransactionScope(
        TransactionScopeOption scopeOption = TransactionScopeOption.RequiresNew,
        TransactionOptions? transactionOptions = null,
        TransactionScopeAsyncFlowOption asyncFlowOption = TransactionScopeAsyncFlowOption.Enabled)
        {
            if (transactionOptions == null)
            {
                transactionOptions = DefaultTransactionOptions;
            }

            return new TransactionScope(scopeOption, (TransactionOptions)transactionOptions, asyncFlowOption);
        }
    }
}
