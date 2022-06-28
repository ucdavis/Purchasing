using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using NHibernate.Proxy;

namespace UCDArch.Web.ModelBinder
{
    public class SuppressNHibernateProxyValidationAttribute : Attribute, IPropertyValidationFilter
    {
        public bool ShouldValidateEntry(ValidationEntry entry, ValidationEntry parentEntry)
        {
            if (typeof(INHibernateProxy).IsAssignableFrom(parentEntry.Model.GetType()))
            {
                return false;
            }
            return true;
        }
    }
}
