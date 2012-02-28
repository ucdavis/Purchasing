using Purchasing.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Purchasing.Web.Models
{
    /// <summary>
    /// ViewModel for the Faq class
    /// </summary>
    public class FaqViewModel
    {
        public Faq Faq { get; set; }
 
        public static FaqViewModel Create(IRepository repository)
        {
            Check.Require(repository != null, "Repository must be supplied");
			
            var viewModel = new FaqViewModel {Faq = new Faq()};
 
            return viewModel;
        }
    }
}