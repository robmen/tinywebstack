using System;

namespace TinyWebStack
{
    /// <summary>
    /// Interface that can be resolved using the IoC <see cref="Container"/> to access web application state.
    /// </summary>
    public interface IApplicationState
    {
        /// <summary>
        /// Gets a previously stored value from the application state.
        /// </summary>
        /// <param name="key">Key of the value.</param>
        /// <returns>Object stored in the application state.</returns>
        object Get(string key);

        /// <summary>
        /// Gets a previously stored value from the application state.
        /// </summary>
        /// <param name="key">Key of the value.</param>
        /// <returns>Object stored in the application state.</returns>
        T Get<T>(string key);

        /// <summary>
        /// Tries to get a previously stored value from the application state.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">Key of the value.</param>
        /// <param name="value">Value stored in the application state.</param>
        /// <returns>True if the key is found and the value is the appropriate type.</returns>
        bool TryGet<T>(string key, out T value);

        void Set(string key, object value, DateTime? expires = null, TimeSpan? sliding = null);

        /// <summary>
        /// Removes a stored value from the applciation state.
        /// </summary>
        /// <param name="key">Key of the value.</param>
        /// <returns>Object previously stored in the application state or null if nothing was stored with that key.</returns>
        object Remove(string key);
    }
}
