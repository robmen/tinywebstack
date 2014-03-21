using System;
using System.Web;

namespace TinyWebStack.Extensions
{
    internal static class StringExtensions
    {
        public static string HtmlDecode(this string s)
        {
            return HttpUtility.HtmlDecode(s);
        }

        public static string HtmlEncode(this string s)
        {
            return HttpUtility.HtmlEncode(s);
        }

        public static string UrlDecode(this string s)
        {
            return HttpUtility.UrlDecode(s);
        }

        public static string UrlEncode(this string s)
        {
            return HttpUtility.UrlEncode(s);
        }

        public static string WithTrailingSlash(this string s)
        {
            return s.EndsWith("/", StringComparison.Ordinal) ? s : s + "/";
        }
    }
}