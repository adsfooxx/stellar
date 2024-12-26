using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ApplicationCore.LinePay.Helpers
{
    public static class RequestHelper
    {
        public static IEnumerable<KeyValuePair<string, string>> GenerateParameters<TRequestParameter>(
            TRequestParameter parameterObj)
        {
            if (parameterObj == null)
                yield break;
            var type = parameterObj.GetType();
            foreach (var property in type.GetProperties())
            {
                var propValue = property.GetValue(parameterObj);
                if (propValue == null)
                    continue;
                var propName = property.GetCustomAttributes<JsonPropertyNameAttribute>().FirstOrDefault()?.Name ??
                               throw new CustomAttributeFormatException("JsonPropertyNameAttribute not found.");
                if (propValue is IEnumerable items)
                {
                    foreach (var item in items)
                    {
                        yield return new KeyValuePair<string, string>(propName, item.ToString());
                    }
                }
                else
                {
                    yield return new KeyValuePair<string, string>(propName, propValue.ToString());
                }
            }
        }
    }
}
