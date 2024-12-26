using System;
using System.Text.Json;

namespace Web.Helpers
{
    public static class Base64SerializerHelpers
    {
        public static T DeSerializeParameter<T>(string parameter) where T : class
        {
            try
            {
                var byteArr = Convert.FromBase64String(parameter);
                var obj = JsonSerializer.Deserialize<T>(byteArr);
                return obj;
            }
            catch(Exception ex)
            {
                return null;
            };
            }
        public static string SerializeInput<T>(T input) where T : class
        {
            var byteArr = JsonSerializer.SerializeToUtf8Bytes(input);
            var base64Str = Convert.ToBase64String(byteArr);
            return base64Str;
       
        }
    }
}
