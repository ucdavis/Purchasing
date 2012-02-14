using Purchasing.Core;

namespace Purchasing.Web.Controllers
{
    public class HistoryController : ApplicationController
    {
        private readonly IRepositoryFactory _repositoryFactory;

        public HistoryController(IRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }


    }
}