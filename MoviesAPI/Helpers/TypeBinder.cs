using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoviesAPI.Helpers
{
    public class TypeBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var propName = bindingContext.ModelName;
            var valuesProvider = bindingContext.ValueProvider.GetValue(propName);

            if (valuesProvider == ValueProviderResult.None)
                return Task.CompletedTask;

            try
            {
                var deserializedValue = JsonConvert.DeserializeObject<ICollection<int>>(valuesProvider.FirstValue);
                bindingContext.Result = ModelBindingResult.Success(deserializedValue);
            }
            catch (Exception ex)
            {
                bindingContext.ModelState.TryAddModelError(propName, "Invalid value to type ICollection<int>");
            }

            return Task.CompletedTask;
        }
    }
}
