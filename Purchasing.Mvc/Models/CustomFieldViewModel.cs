using Purchasing.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Purchasing.Web.Models
{
    /// <summary>
    /// ViewModel for the CustomField class
    /// </summary>
    public class CustomFieldViewModel
    {
        public CustomField CustomField { get; set; }
        public Organization Organization { get; set; }
 
        public static CustomFieldViewModel Create(IRepository repository, Organization organization, CustomField customField = null)
        {
            Check.Require(repository != null, "Repository must be supplied");
			
            var viewModel = new CustomFieldViewModel {Organization = organization, CustomField = customField ?? new CustomField(){Organization = organization}};
 
            // assign the rank of the new custom field
            viewModel.CustomField.Rank = organization.CustomFields.Count;

            return viewModel;
        }
    }

    public class CustomFieldOrderPostModel
    {
        public int CustomFieldId { get; set; }
        public int Order { get; set; }
    }
}