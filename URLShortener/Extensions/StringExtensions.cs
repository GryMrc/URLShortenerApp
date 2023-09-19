using System.Text.RegularExpressions;

namespace URLShortener.Extensions
{
    public static class StringExtensions
    {
        public static string UrlFormat(this string code,HttpContext httpContext)
        {
            return $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/{code}";
        }

        public static bool isBase62(this string code)
        {
            string pattern = "^[a-zA-Z0-9]*$";

            return Regex.IsMatch(code, pattern);
        }

        public static bool isUrlValid(this string originalUrl) => Uri.TryCreate(originalUrl, UriKind.Absolute, out _);
    }
}
