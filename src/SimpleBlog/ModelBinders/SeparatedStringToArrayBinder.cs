using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections;

namespace SimpleBlog.ModelBinders;

public class SeparatedStringToArrayBinder : IModelBinder
{
    private const char SEPARATOR = ',';

    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null)
            throw new ArgumentNullException(nameof(bindingContext));

        var modelName = bindingContext.ModelName;
        var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

        if (valueProviderResult == ValueProviderResult.None)
            return Task.CompletedTask;

        bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

        var rawValue = valueProviderResult.FirstValue;

        var rawArray = rawValue?.Split(SEPARATOR, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (rawArray == null || rawArray.Length < 1)
            return Task.CompletedTask;

        var elementType = bindingContext.ModelMetadata.ElementType;

        if (elementType == null)
            throw new InvalidOperationException("ModelBinder do not support this type");

        IEnumerable? model = elementType switch
        {
            Type t when t == typeof(string) => rawArray,
            Type t when t == typeof(int) => TryParseArray(rawArray, (s) => (int.TryParse(s, out var x), x)),
            Type t when t == typeof(uint) => TryParseArray(rawArray, (s) => (uint.TryParse(s, out var x), x)),
            _ => throw new InvalidOperationException("ModelBinder do not support this type"),
        };

        if (model == null)
            bindingContext.Result = ModelBindingResult.Failed();
        else
            bindingContext.Result = ModelBindingResult.Success(model);

        return Task.CompletedTask;
    }

    private IEnumerable? TryParseArray<T>(IEnumerable<string> rawArray, Func<string, (bool, T)> converter)
    {
        var array = new List<T>();
        foreach (var item in rawArray)
        {
            var (result, value) = converter(item);

            if (!result)
                return null;

            array.Add(value);
        }
        return array;
    }
}