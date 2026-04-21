using System.Threading;
using Cysharp.Threading.Tasks;
using SDK.Domain.Ads;
using Reflex.Attributes;
using UnityEngine;

namespace SDK.Presentation.Game
{
    public sealed class GameAdsExample : MonoBehaviour
    {
        [SerializeField] private string rewardedAdUnitId = "rewarded_unit_id";
        [SerializeField] private string interstitialAdUnitId = "interstitial_unit_id";

        [Inject] private IAdsService _adsService;

        private async void Start()
        {
            if (_adsService == null)
            {
                Debug.LogWarning("Ads service is not initialized.");
                return;
            }

            await _adsService.PreloadAsync(rewardedAdUnitId, AdFormat.Rewarded, CancellationToken.None);
            await _adsService.PreloadAsync(interstitialAdUnitId, AdFormat.Interstitial, CancellationToken.None);
        }

        /// <summary>
        /// Attempts to show a rewarded ad and grants revive when successful.
        /// </summary>
        /// <returns>True when revive should be granted.</returns>
        public async UniTask<bool> TryShowReviveAdAsync()
        {
            if (_adsService == null) return false;
            var result = await _adsService.ShowRewardedAsync(rewardedAdUnitId, CancellationToken.None);
            if (result == AdShowResult.Success)
            {
                GrantRevive();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Shows an interstitial ad at level end when possible.
        /// </summary>
        public async UniTask ShowLevelEndInterstitialAsync()
        {
            if (_adsService != null)
                await _adsService.ShowInterstitialAsync(interstitialAdUnitId, CancellationToken.None);
        }

        private void GrantRevive()
        {
            Debug.Log("Revive granted.");
        }
    }
}
