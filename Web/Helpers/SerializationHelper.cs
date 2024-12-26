using System.Text.Json;

namespace Web.Helpers
{
    public static  class SerializationHelper
    {
        public static byte[] ObjectToByteArray(object obj)
        {
            return JsonSerializer.SerializeToUtf8Bytes(obj);
        }

        public static T ByteArrayToObj<T>(byte[] byteArr) where T : class
        {
            return byteArr is null ? null : JsonSerializer.Deserialize<T>(byteArr);
        }
    }   
}
