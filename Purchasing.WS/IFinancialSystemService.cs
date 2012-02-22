using Purchasing.Core.Domain;

namespace Purchasing.WS
{
    public interface IFinancialSystemService
    {
        /// <summary>
        /// Submits the order to the campus financial system
        /// </summary>
        /// <param name="order">Order to be submitted</param>
        /// <param name="userId">Kerb id of user to save the document to</param>
        void SubmitOrder(Order order, string userId);
    }
}
