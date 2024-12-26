namespace Web.Extensions
{
    public static class HttpRequestExtensions
    {
        public static bool IsAjaxRequest(this HttpRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            // AJAX 請求會有一個特殊的 Header "X-Requested-With"，值為 "XMLHttpRequest"
            if (request.Headers != null)
            {
                return request.Headers["X-Requested-With"] == "XMLHttpRequest";
            }

            return false;
        }
    }
}

