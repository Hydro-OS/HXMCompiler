using System.Collections.Generic;

namespace HXMCompiler
{
    /// <summary>
    /// Extension methods used by the HXM compiler.
    /// </summary>
    internal static class Extensions
    {
        /// <summary>
        /// Gets an element from a dictionary, and if it doesn't exists, returns the specified default value.
        /// </summary>
        /// <typeparam name="TKey">The dictionary key type.</typeparam>
        /// <typeparam name="TValue">The dictionary value type.</typeparam>
        /// <param name="dictionary">The target dictionary.</param>
        /// <param name="key">The target key to get the value from.</param>
        /// <param name="defaultValue">If the key does not exist, this value will be returned instead.</param>
        /// <returns>If exists, the value for the specified key, and otherwise the default value.</returns>
        internal static TValue GetValueOrDefault<TKey, TValue>
            (this IDictionary<TKey, TValue> dictionary,
             TKey key,
             TValue defaultValue)
        {
            return dictionary.TryGetValue(key, out TValue value) ? value : defaultValue;
        }

        /// <summary>
        /// Converts a string to a array of bytes. A character will be 1 byte.
        /// </summary>
        /// <param name="str">The target string.</param>
        /// <returns>A byte array, with it's length being equal to the length of the string.</returns>
        internal static byte[] ToAsciiBytes(this string str)
        {
            byte[] bytes = new byte[str.Length];

            for (int i = 0; i < str.Length; i++)
            {
                bytes[i] = (byte)str[i];
            }

            return bytes;
        }
    }
}
