using System.Globalization;
using System.Collections.Generic;

namespace SDK.Infrastructure.Config
{
    public sealed class FirebaseDefaultsProvider
    {
        private readonly FirebaseConfigScriptableObject _config;

        /// <summary>
        /// Creates a provider from a Firebase config asset.
        /// </summary>
        /// <param name="config">Firebase config scriptable object.</param>
        public FirebaseDefaultsProvider(FirebaseConfigScriptableObject config)
        {
            _config = config;
        }

        /// <summary>
        /// Builds defaults dictionary from configured entries.
        /// </summary>
        /// <returns>Read-only key/value defaults.</returns>
        public IReadOnlyDictionary<string, string> GetDefaults()
        {
            var defaults = new Dictionary<string, string>();
            var entries = _config.Defaults;
            for (var i = 0; i < entries.Count; i++)
            {
                var entry = entries[i];
                if (!string.IsNullOrEmpty(entry.Key))
                {
                    defaults[entry.Key] = entry.Value ?? string.Empty;
                }
            }

            return defaults;
        }

        /// <summary>
        /// Gets a default string value for a key.
        /// </summary>
        /// <param name="key">Config key.</param>
        /// <param name="fallback">Fallback value when key is missing.</param>
        /// <returns>Configured default value.</returns>
        public string GetString(string key, string fallback = "")
        {
            var entries = _config.Defaults;
            for (var i = 0; i < entries.Count; i++)
            {
                var entry = entries[i];
                if (entry != null && string.Equals(entry.Key, key))
                {
                    return entry.Value ?? fallback;
                }
            }

            return fallback;
        }

        /// <summary>
        /// Gets a default integer value for a key.
        /// </summary>
        /// <param name="key">Config key.</param>
        /// <param name="fallback">Fallback value when key is missing or invalid.</param>
        /// <returns>Configured default value.</returns>
        public int GetInt(string key, int fallback = 0)
        {
            var value = GetString(key, null);
            if (!string.IsNullOrWhiteSpace(value) && int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed))
            {
                return parsed;
            }

            return fallback;
        }

        /// <summary>
        /// Gets a default float value for a key.
        /// </summary>
        /// <param name="key">Config key.</param>
        /// <param name="fallback">Fallback value when key is missing or invalid.</param>
        /// <returns>Configured default value.</returns>
        public float GetFloat(string key, float fallback = 0f)
        {
            var value = GetString(key, null);
            if (!string.IsNullOrWhiteSpace(value) && float.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var parsed))
            {
                return parsed;
            }

            return fallback;
        }
    }
}
