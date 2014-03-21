using System;

namespace TinyWebStack
{
    /// <summary>
    /// Interface that can be resolved using the IoC <see cref="Container"/> to access web application state.
    /// </summary>
    public interface IApplicationState
    {
        /// <summary>
        /// Gets a previously stored value from the applciation state.
        /// </summary>
        /// <param name="key">Key of the value.</param>
        /// <returns>Object stored in the application state.</returns>
        object Get(string key);

        /// <summary>
        /// Gets a previously stored value from the applciation state.
        /// </summary>
        /// <param name="key">Key of the value.</param>
        /// <returns>Object stored in the application state.</returns>
        T Get<T>(string key);

        void Set(string key, object value, DateTime? expires = null, TimeSpan? sliding = null);

        /// <summary>
        /// Removes a stored value from the applciation state.
        /// </summary>
        /// <param name="key">Key of the value.</param>
        /// <returns>Object previously stored in the application state or null if nothing was stored with that key.</returns>
        object Remove(string key);
    }
}
