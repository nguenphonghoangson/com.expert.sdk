using System;
using System.Collections.Generic;
using UnityEngine;

namespace SDK.Infrastructure.Config
{
    [CreateAssetMenu(menuName = "SDK/Config/Firebase Config", fileName = "FirebaseConfig")]
    public sealed class FirebaseConfigScriptableObject : ScriptableObject
    {
        [SerializeField] private bool enableAnalytics = true;
        [SerializeField] private bool enableRemoteConfig = true;
        [SerializeField] private List<RemoteDefaultEntry> defaults = new List<RemoteDefaultEntry>();

        public bool EnableAnalytics => enableAnalytics;
        public bool EnableRemoteConfig => enableRemoteConfig;
        public IReadOnlyList<RemoteDefaultEntry> Defaults => defaults;

        [Serializable]
        public sealed class RemoteDefaultEntry
        {
            public string Key;
            public string Value;
        }
    }
}
