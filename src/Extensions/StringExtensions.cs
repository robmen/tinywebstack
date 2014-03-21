using System;

namespace TinyWebStack.Extensions
{
    internal static class StringExtensions
    {
        public static string WithTrailingSlash(this string s)
        {
            return s.EndsWith("/", StringComparison.Ordinal) ? s : s + "/";
        }
    }
}