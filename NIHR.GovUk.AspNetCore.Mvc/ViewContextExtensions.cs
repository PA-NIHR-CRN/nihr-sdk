using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace NIHR.GovUk
{
    public static class ViewContextExtensions
    {
        public static IEnumerable<KeyValuePair<string, ModelStateEntry>> Errors(this ViewContext viewContext)
        {
            var modelErrors = viewContext.ViewData.ModelState
                .Where(ms => ms.Value.Errors.Count > 0)
                .DistinctBy(x => x.Value.Errors.FirstOrDefault()?.ErrorMessage);

            var s = viewContext.ViewData.ModelExplorer;
            var modelProperties = viewContext.ViewData.Model?.GetType().GetProperties();

            var errors = new List<KeyValuePair<List<int>, KeyValuePair<string, ModelStateEntry>>>();
            foreach (var errorKeyValuePair in modelErrors)
            {
                var modelProperty = modelProperties?.FirstOrDefault(o => o.Name == errorKeyValuePair.Key);
                if (modelProperty is not null)
                {
                    var displayAttribute =
                        modelProperty?.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault() as
                            DisplayAttribute;
                    var propertyOrder = displayAttribute?.GetOrder() ?? int.MaxValue;
                    errors.Add(new([propertyOrder], errorKeyValuePair));
                }
                else
                {
                    var pathToErrorModelProperties = errorKeyValuePair.Key.Split('.');
                    var nestedModelObject = pathToErrorModelProperties.Length > 1;
                    if (!nestedModelObject)
                    {
                        errors.Add(new([int.MaxValue], errorKeyValuePair));
                    }

                    errors.Add(GetNestedPropertyOrder(errorKeyValuePair, pathToErrorModelProperties, modelProperties));
                }
            }

            return errors
                .OrderBy(o => o.Key, new ListOfIntComparer())
                .Select(o => o.Value)
                .ToList();
        }

        private static KeyValuePair<List<int>, KeyValuePair<string, ModelStateEntry>> GetNestedPropertyOrder(KeyValuePair<string, ModelStateEntry> errorKeyValuePair, string[] pathToErrorModelProperties, PropertyInfo[] parentModelProperties)
        {
            var nestedPropertyOrder = new List<int>();
            //First get the parent property 
            var parentPropertyName = pathToErrorModelProperties[0];
            var parentProperty = parentModelProperties?.FirstOrDefault(o => o.Name == parentPropertyName);
            if (parentProperty is not null) // TODO: investigate missing postcode model error message in form-group.
            {
                nestedPropertyOrder.Add(GetOrderFromDisplayAttribute(parentProperty));
                var nestedType = parentProperty.PropertyType;
                //Then iterate through the model
                for (var i = 1; i < pathToErrorModelProperties.Length; i++)
                {
                    var nestedProperties = nestedType.GetProperties();
                    var nestedProperty = nestedProperties.FirstOrDefault(o => o.Name == pathToErrorModelProperties[i]);
                    nestedPropertyOrder.Add(GetOrderFromDisplayAttribute(nestedProperty));
                    nestedType = nestedProperty.PropertyType;
                }
            }

            return new(nestedPropertyOrder, errorKeyValuePair);
        }

        private static int GetOrderFromDisplayAttribute(PropertyInfo property)
        {
            var displayAttribute = property?.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault() as DisplayAttribute;
            return displayAttribute?.GetOrder() ?? int.MaxValue;
        }

        private class ListOfIntComparer : IComparer<List<int>>
        {
            public int Compare(List<int> x, List<int> y)
            {
                if (x == null || y == null)
                {
                    return (x == null) ? ((y == null) ? 0 : -1) : 1;
                }

                int minLength = Math.Min(x.Count, y.Count);
                for (int i = 0; i < minLength; i++)
                {
                    int result = x[i].CompareTo(y[i]);
                    if (result != 0)
                    {
                        return result;
                    }
                }

                // If all compared elements are equal, the shorter list is considered less
                return x.Count.CompareTo(y.Count);
            }
        }
    }
}