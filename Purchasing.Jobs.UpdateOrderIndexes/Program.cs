using System;
using Ninject;
using Purchasing.Core.Services;
using Purchasing.Jobs.Common;

namespace Purchasing.Jobs.UpdateOrderIndexes
{
    class Program : WebJobBase
    {
        static void Main(string[] args)
        {
            var kernel = ConfigureServices();

            var indexService = kernel.Get<IIndexService>();

            try
            {
                indexService.UpdateOrderIndexes();
                indexService.UpdateCommentsIndex();

                Console.WriteLine("Order indexes updated successfully at {0}", DateTime.Now);
            }
            catch (Exception ex)
            {
                Console.WriteLine("FAILED: Indexes failed because {0}", ex.Message);
            }
        }
    }
}
