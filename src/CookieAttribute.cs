using System;

namespace TinyWebStack
{
    /// <summary>
    /// Attribute to indicate a property gets and/or sets a cookie.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class CookieAttribute : Attribute
    {
        public CookieAttribute(string name)
        {
            this.Name = name;
            this.HttpOnly = true;
        }

        /// <summary>
        /// Name of cookie.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets the time in seconds until the cookie expires. Only used when setting cookies.
        /// </summary>
        public int Expires { get; set; }

        /// <summary>
        /// Indicates whether the cookie is HTTP only. Default is true.
        /// </summary>
        public bool HttpOnly { get; set; }

        /// <summary>
        /// Gets or sets the path for the cookie. Default is "/".
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets whether the cookie only travels over secure channels.
        /// </summary>
        public bool Secure { get; set; }
    }
}
