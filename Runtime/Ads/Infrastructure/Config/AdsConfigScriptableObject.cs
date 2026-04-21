using System;
using UnityEngine;

namespace SDK.Infrastructure.Config
{
    [CreateAssetMenu(menuName = "SDK/Config/Ads Config", fileName = "AdsConfig")]
    public sealed class AdsConfigScriptableObject : ScriptableObject
    {
        [Header("AppLovin MAX")]
        [SerializeField] private string sdkKey = "";

        [Header("Ad Units")]
        [SerializeField] private AdUnitEntry[] adUnits = Array.Empty<AdUnitEntry>();

        public string SdkKey => sdkKey;
        public AdUnitEntry[] AdUnits => adUnits;

        /// <summary>
        /// Returns all configured ad unit ids for selective SDK initialization.
        /// </summary>
        /// <returns>Array of ad unit ids.</returns>
        public string[] GetSelectiveInitAdUnitIds()
        {
            var ids = new string[adUnits.Length];
            for (var i = 0; i < adUnits.Length; i++)
                ids[i] = adUnits[i].AdUnitId;
            return ids;
        }

        [Serializable]
        public sealed class AdUnitEntry
        {
            public string Label;
            public string AdUnitId;
            public SDK.Domain.Ads.AdFormat Format;
        }
    }
}
