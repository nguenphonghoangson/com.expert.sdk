using System;
using System.Collections.Generic;

namespace SDK.Domain.Firebase
{
    [Serializable]
    public sealed class RemoteConfigEntry
    {
        public string Key;
        public string Value;
    }

    [Serializable]
    public sealed class RemoteConfigPayload
    {
        public List<RemoteConfigEntry> Entries = new List<RemoteConfigEntry>();

        /// <summary>
        /// Returns the value for a key from the payload or fallback when missing.
        /// </summary>
        /// <param name="key">Config key.</param>
        /// <param name="fallback">Fallback value.</param>
        /// <returns>Config value or fallback.</returns>
        public string Get(string key, string fallback = "")
        {
            for (var i = 0; i < Entries.Count; i++)
            {
                if (string.Equals(Entries[i].Key, key, StringComparison.Ordinal))
                {
                    return Entries[i].Value;
                }
            }

            return fallback;
        }
    }
}
