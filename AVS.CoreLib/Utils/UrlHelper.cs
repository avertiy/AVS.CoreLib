namespace AVS.CoreLib.Utils
{
    public static class UrlHelper
    {
        public static string Combine(string baseUrl, string queryString)
        {
            var res = baseUrl;
            if (queryString.Length > 0)
            {
                if (!queryString.StartsWith("/"))
                    res += (baseUrl.Contains("?") ? "&" : "?");
                res += queryString;
            }
            return res;
        }
    }
}