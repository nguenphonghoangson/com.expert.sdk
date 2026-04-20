using System.Threading;
using Cysharp.Threading.Tasks;
using SDK.Domain.Ads;
using SDK.Application.Ads;
using Reflex.Attributes;
using UnityEngine;

namespace SDK.Presentation.Game
{
    public sealed class GameAdsExample : MonoBehaviour
    {
        [SerializeField] private string rewardedAdUnitId = "rewarded_unit_id";
        [SerializeField] private string interstitialAdUnitId = "interstitial_unit_id";

        [Inject] private PreloadAdsUseCase _preloadAds;
        [Inject] private ShowAdsUseCase _showAds;

        private async void Start()
        {
            if (_preloadAds == null || _showAds == null)
            {
                Debug.LogWarning("Ads Use Cases are not initialized.");
                return;
            }

            await _preloadAds.ExecuteAsync(rewardedAdUnitId, AdFormat.Rewarded, CancellationToken.None);
            await _preloadAds.ExecuteAsync(interstitialAdUnitId, AdFormat.Interstitial, CancellationToken.None);
        }

        public async UniTask<bool> TryShowReviveAdAsync()
        {
            if (_showAds == null) return false;
            var result = await _showAds.ExecuteRewardedAsync(rewardedAdUnitId, CancellationToken.None);
            if (result == AdShowResult.Success)
            {
                GrantRevive();
                return true;
            }

            return false;
        }

        public async UniTask ShowLevelEndInterstitialAsync()
        {
            if (_showAds != null)
                await _showAds.ExecuteInterstitialAsync(interstitialAdUnitId, CancellationToken.None);
        }

        private void GrantRevive()
        {
            Debug.Log("Revive granted.");
        }
    }
}
