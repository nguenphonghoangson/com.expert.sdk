using System.Collections.Generic;

namespace SDK.Infrastructure.Config
{
    public sealed class FirebaseDefaultsProvider
    {
        private readonly FirebaseConfigScriptableObject _config;

        public FirebaseDefaultsProvider(FirebaseConfigScriptableObject config)
        {
            _config = config;
        }

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
    }
}
