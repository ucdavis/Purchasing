using System;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace UCDArch.Web.ModelBinder
{
    public class EmptyStringMetadataProvider : IMetadataDetailsProvider, IDisplayMetadataProvider
    {
        public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
        {
            if (context.Key.ModelType == typeof(string) &&
               (context.Key.MetadataKind == ModelMetadataKind.Property || context.Key.MetadataKind == ModelMetadataKind.Parameter))
            {
                context.DisplayMetadata.ConvertEmptyStringToNull = false;
            }
        }
    }
}
