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
