using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using ApplicationCore.LinePay.Converters;

namespace ApplicationCore.LinePay.Extensions
{
    public static class JsonExtension
    {
        private static readonly JsonSerializerOptions SerializerOptions = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters =
            {
                new DecimalWithoutTrailingZeroJsonConverter()
            }
        };

        public static string ToJson(this object obj)
        {
            return JsonSerializer.Serialize(obj, SerializerOptions);
        }

        public static bool TryParseJson<T>(this string json, out T obj, out string errMsg) where T : class
        {
            errMsg = string.Empty;
            try
            {
                obj = JsonSerializer.Deserialize<T>(json);
                return true;
            }
            catch (Exception e)
            {
                obj = default;
                errMsg = e.Message;
                return false;
            }
        }
    }
}
