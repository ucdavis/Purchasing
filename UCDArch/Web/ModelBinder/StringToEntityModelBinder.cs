using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace UCDArch.Web.ModelBinder
{
    public class EntityModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.IsCollectionType && ValueBinderHelper.IsEntityType(context.Metadata.ElementType))
            {
                return new EntityCollectionModelBinder(context.Metadata.ModelType);
            }
            else if (ValueBinderHelper.IsEntityType(context.Metadata.ModelType))
            {
                return new EntityModelBinder(context.Metadata.ModelType);
            }

            return null;
        }
    }

    public class EntityModelBinder : IModelBinder
    {
        private readonly Type _entityType;

        public EntityModelBinder(Type entityType)
        {
            _entityType = entityType;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (!string.IsNullOrWhiteSpace(valueProviderResult.FirstValue))
            {
                var entity = ValueBinderHelper.GetEntity(_entityType, valueProviderResult.FirstValue);
                bindingContext.Result = ModelBindingResult.Success(entity);
            }

            return Task.CompletedTask;
        }
    }

    public class EntityCollectionModelBinder : IModelBinder
    {
        private readonly Type _collectionType;

        public EntityCollectionModelBinder(Type collectionType)
        {
            _collectionType = collectionType;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            var values = valueProviderResult.Values.Where(v => !string.IsNullOrWhiteSpace(v)).ToList();

            if (values.Count > 0)
            {
                var entities = ValueBinderHelper.GetEntityCollection(_collectionType, values);
                bindingContext.Result = ModelBindingResult.Success(entities);
            }

            return Task.CompletedTask;
        }
    }
}
