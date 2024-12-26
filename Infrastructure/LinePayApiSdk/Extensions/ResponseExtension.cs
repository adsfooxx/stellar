using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationCore.LinePay.Dtos;

namespace ApplicationCore.LinePay.Extensions
{
    public static class HttpClientExtension
    {
        public static async Task<TResponse> ToResult<TResponse>(this Task<HttpResponseMessage> responseTask)
            where TResponse : LinePayApiResponseBase
        {
            var response = await responseTask;
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                var requestUri = response.RequestMessage?.RequestUri; // 獲取請求的 URI
                Console.WriteLine($"HTTP Request to {requestUri} failed with status code {response.StatusCode}: {errorContent}");
                throw new Exception($"HTTP Request failed with status code {response.StatusCode}: {errorContent}");
            }

            var isJsonFormat = (await response.Content.ReadAsStringAsync())
                .TryParseJson<TResponse>(out var jsonResult, out var errMsg);

            return isJsonFormat
                ? jsonResult
                : throw new InvalidOperationException(errMsg);
        }
    }
}
