using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using SDK.Domain.Ads;
using UnityEngine;

namespace SDK.Infrastructure.Ads
{
    public sealed class AppLovinMaxAdapter : IAdNetworkAdapter, IRewardStatusProvider, IAdRevenueEventProvider, ISelectiveInitAdUnitsConfigurator, IDisposable
    {
        private const int CallbackTimeoutMs = 12000;
        private static readonly string[] EmptyAdUnitIds = Array.Empty<string>();
        private readonly Dictionary<string, UniTaskCompletionSource<bool>> _loadWaiters = new Dictionary<string, UniTaskCompletionSource<bool>>();
        private readonly Dictionary<string, UniTaskCompletionSource<AdShowResult>> _showWaiters = new Dictionary<string, UniTaskCompletionSource<AdShowResult>>();
        private readonly HashSet<string> _loadedInterstitials = new HashSet<string>();
        private readonly HashSet<string> _loadedRewarded = new HashSet<string>();
        private readonly HashSet<string> _createdBanners = new HashSet<string>();
        private readonly Dictionary<string, bool> _rewardGrantedByUnit = new Dictionary<string, bool>();
        private UniTaskCompletionSource<bool> _initializeWaiter;
        private string[] _selectiveInitAdUnitIds = EmptyAdUnitIds;
        private bool _initialized;
        private bool _initializeRequested;
        private bool _callbacksRegistered;
        private bool _disposed;
        public event Action<AdRevenueSignal> RevenuePaid;

        public AdProvider Provider => AdProvider.AppLovinMax;

        public async UniTask InitializeAsync(string[] selectiveInitAdUnitIds,CancellationToken cancellationToken)
        {
            if (_disposed)
            {
                return;
            }

#if APPLOVIN_MAX_SDK
            if (_initialized)
            {
                return;
            }

            if (!_initializeRequested)
            {
                RegisterCallbacks();
                _initializeWaiter = new UniTaskCompletionSource<bool>();
                _initializeRequested = true;
                InitializeMaxSdk(selectiveInitAdUnitIds);
            }

            if (_initializeWaiter != null)
            {
                await _initializeWaiter.Task;
            }
#else
            Debug.LogWarning("APPLOVIN_MAX_SDK is not defined. AppLovin MAX adapter is running in fallback mode.");
#endif
        }

        public void SetSelectiveInitAdUnitIds(string[] adUnitIds)
        {
            _selectiveInitAdUnitIds = adUnitIds == null || adUnitIds.Length == 0
                ? EmptyAdUnitIds
                : adUnitIds;
        }

        public async UniTask<bool> LoadAsync(string adUnitId, AdFormat format, CancellationToken cancellationToken)
        {
            if (_disposed)
            {
                return false;
            }

#if APPLOVIN_MAX_SDK
            var key = BuildKey(adUnitId, format);
            var waiter = CreateLoadWaiter(key, cancellationToken);

            switch (format)
            {
                case AdFormat.Banner:
                    if (!_createdBanners.Contains(adUnitId))
                    {
                        MaxSdk.CreateBanner(adUnitId, MaxSdkBase.BannerPosition.BottomCenter);
                        _createdBanners.Add(adUnitId);
                    }
                    break;
                case AdFormat.Interstitial:
                    MaxSdk.LoadInterstitial(adUnitId);
                    break;
                case AdFormat.Rewarded:
                    MaxSdk.LoadRewardedAd(adUnitId);
                    break;
                default:
                    return false;
            }

            return await waiter.Task;
#else
            await UniTask.Delay(20, cancellationToken: cancellationToken);
            return true;
#endif
        }

        public bool IsReady(string unitId, AdFormat format)
        {
            if (_disposed)
            {
                return false;
            }

#if APPLOVIN_MAX_SDK
            return format switch
            {
                AdFormat.Banner => _createdBanners.Contains(unitId),
                AdFormat.Interstitial => MaxSdk.IsInterstitialReady(unitId) || _loadedInterstitials.Contains(unitId),
                AdFormat.Rewarded => MaxSdk.IsRewardedAdReady(unitId) || _loadedRewarded.Contains(unitId),
                _ => false,
            };
#else
            return true;
#endif
        }

        public async UniTask<AdShowResult> ShowAsync(string adUnitId, AdFormat format, CancellationToken cancellationToken)
        {
            if (_disposed)
            {
                return AdShowResult.Failed;
            }

#if APPLOVIN_MAX_SDK
            if (!IsReady(adUnitId, format))
            {
                return AdShowResult.NotReady;
            }

            if (format == AdFormat.Banner)
            {
                MaxSdk.ShowBanner(adUnitId);
                return AdShowResult.Success;
            }

            var key = BuildKey(adUnitId, format);
            var waiter = CreateShowWaiter(key, cancellationToken);

            if (format == AdFormat.Interstitial)
            {
                MaxSdk.ShowInterstitial(adUnitId);
            }
            else if (format == AdFormat.Rewarded)
            {
                _rewardGrantedByUnit[adUnitId] = false;
                MaxSdk.ShowRewardedAd(adUnitId);
            }
            else
            {
                return AdShowResult.Failed;
            }

            return await waiter.Task;
#else
            await UniTask.Delay(20, cancellationToken: cancellationToken);
            if (format == AdFormat.Rewarded)
            {
                _rewardGrantedByUnit[adUnitId] = true;
            }
            return AdShowResult.Success;
#endif
        }

        public bool ConsumeRewardResult(string unitId)
        {
            if (_disposed)
            {
                return false;
            }

            if (_rewardGrantedByUnit.TryGetValue(unitId, out var granted))
            {
                _rewardGrantedByUnit[unitId] = false;
                return granted;
            }

            return false;
        }

#if APPLOVIN_MAX_SDK
        private void RegisterCallbacks()
        {
            MaxSdkCallbacks.OnSdkInitializedEvent += OnSdkInitialized;
            MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerLoaded;
            MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerLoadFailed;

            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoaded;
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailed;
            MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayed;
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHidden;
            MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialDisplayFailed;
            MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClicked;
            MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnInterstitialRevenuePaid;

            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedLoaded;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedLoadFailed;
            MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedDisplayed;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedReceivedReward;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedHidden;
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedDisplayFailed;
            MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedClicked;
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedRevenuePaid;

            _callbacksRegistered = true;
        }

        private void UnregisterCallbacks()
        {
            if (!_callbacksRegistered)
            {
                return;
            }

            MaxSdkCallbacks.OnSdkInitializedEvent -= OnSdkInitialized;
            MaxSdkCallbacks.Banner.OnAdLoadedEvent -= OnBannerLoaded;
            MaxSdkCallbacks.Banner.OnAdLoadFailedEvent -= OnBannerLoadFailed;

            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent -= OnInterstitialLoaded;
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent -= OnInterstitialLoadFailed;
            MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent -= OnInterstitialDisplayed;
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent -= OnInterstitialHidden;
            MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent -= OnInterstitialDisplayFailed;
            MaxSdkCallbacks.Interstitial.OnAdClickedEvent -= OnInterstitialClicked;
            MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent -= OnInterstitialRevenuePaid;

            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent -= OnRewardedLoaded;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent -= OnRewardedLoadFailed;
            MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent -= OnRewardedDisplayed;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent -= OnRewardedReceivedReward;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent -= OnRewardedHidden;
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent -= OnRewardedDisplayFailed;
            MaxSdkCallbacks.Rewarded.OnAdClickedEvent -= OnRewardedClicked;
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent -= OnRewardedRevenuePaid;

            _callbacksRegistered = false;
        }

        private void OnSdkInitialized(MaxSdkBase.SdkConfiguration sdkConfiguration)
        {
            _initialized = true;
            _initializeWaiter?.TrySetResult(true);
        }

        private void OnBannerLoaded(string unitId, MaxSdkBase.AdInfo adInfo)
        {
            ResolveLoad(BuildKey(unitId, AdFormat.Banner), true);
        }

        private void OnBannerLoadFailed(string unitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            ResolveLoad(BuildKey(unitId, AdFormat.Banner), false);
        }

        private void OnInterstitialLoaded(string unitId, MaxSdkBase.AdInfo adInfo)
        {
            _loadedInterstitials.Add(unitId);
            ResolveLoad(BuildKey(unitId, AdFormat.Interstitial), true);
        }

        private void OnInterstitialLoadFailed(string unitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            ResolveLoad(BuildKey(unitId, AdFormat.Interstitial), false);
        }

        private void OnInterstitialDisplayed(string unitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log($"[MAX] Interstitial displayed. unitId={unitId}");
        }

        private void OnInterstitialHidden(string unitId, MaxSdkBase.AdInfo adInfo)
        {
            ResolveShow(BuildKey(unitId, AdFormat.Interstitial), AdShowResult.Success);
        }

        private void OnInterstitialDisplayFailed(string unitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            ResolveShow(BuildKey(unitId, AdFormat.Interstitial), AdShowResult.Failed);
        }

        private void OnInterstitialClicked(string unitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log($"[MAX] Interstitial clicked. unitId={unitId}");
        }

        private void OnInterstitialRevenuePaid(string unitId, MaxSdkBase.AdInfo adInfo)
        {
            EmitRevenue(unitId, AdFormat.Interstitial, adInfo.NetworkName, adInfo.Revenue);
        }

        private void OnRewardedLoaded(string unitId, MaxSdkBase.AdInfo adInfo)
        {
            _loadedRewarded.Add(unitId);
            ResolveLoad(BuildKey(unitId, AdFormat.Rewarded), true);
        }

        private void OnRewardedLoadFailed(string unitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            ResolveLoad(BuildKey(unitId, AdFormat.Rewarded), false);
        }

        private void OnRewardedDisplayed(string unitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log($"[MAX] Rewarded displayed. unitId={unitId}");
        }

        private void OnRewardedReceivedReward(string unitId, MaxSdkBase.Reward reward, MaxSdkBase.AdInfo adInfo)
        {
            _rewardGrantedByUnit[unitId] = true;
        }

        private void OnRewardedHidden(string unitId, MaxSdkBase.AdInfo adInfo)
        {
            ResolveShow(BuildKey(unitId, AdFormat.Rewarded), AdShowResult.Success);
        }

        private void OnRewardedDisplayFailed(string unitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            ResolveShow(BuildKey(unitId, AdFormat.Rewarded), AdShowResult.Failed);
        }

        private void OnRewardedClicked(string unitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log($"[MAX] Rewarded clicked. unitId={unitId}");
        }

        private void OnRewardedRevenuePaid(string unitId, MaxSdkBase.AdInfo adInfo)
        {
            EmitRevenue(unitId, AdFormat.Rewarded, adInfo.NetworkName, adInfo.Revenue);
        }

        private UniTaskCompletionSource<bool> CreateLoadWaiter(string key, CancellationToken cancellationToken)
        {
            var tcs = new UniTaskCompletionSource<bool>();
            _loadWaiters[key] = tcs;
            cancellationToken.Register(() => tcs.TrySetCanceled(cancellationToken));
            TimeoutLoadWaiterAsync(key, tcs, cancellationToken).Forget();
            return tcs;
        }

        private UniTaskCompletionSource<AdShowResult> CreateShowWaiter(string key, CancellationToken cancellationToken)
        {
            var tcs = new UniTaskCompletionSource<AdShowResult>();
            _showWaiters[key] = tcs;
            cancellationToken.Register(() => tcs.TrySetCanceled(cancellationToken));
            TimeoutShowWaiterAsync(key, tcs, cancellationToken).Forget();
            return tcs;
        }

        private async UniTaskVoid TimeoutLoadWaiterAsync(string key, UniTaskCompletionSource<bool> waiter, CancellationToken cancellationToken)
        {
            var canceled = await UniTask.Delay(CallbackTimeoutMs, cancellationToken: cancellationToken).SuppressCancellationThrow();
            if (canceled)
            {
                return;
            }

            if (_loadWaiters.Remove(key))
            {
                waiter.TrySetResult(false);
            }
        }

        private async UniTaskVoid TimeoutShowWaiterAsync(string key, UniTaskCompletionSource<AdShowResult> waiter, CancellationToken cancellationToken)
        {
            var canceled = await UniTask.Delay(CallbackTimeoutMs, cancellationToken: cancellationToken).SuppressCancellationThrow();
            if (canceled)
            {
                return;
            }

            if (_showWaiters.Remove(key))
            {
                waiter.TrySetResult(AdShowResult.Failed);
            }
        }

        private void ResolveLoad(string key, bool success)
        {
            if (_loadWaiters.TryGetValue(key, out var waiter))
            {
                _loadWaiters.Remove(key);
                waiter.TrySetResult(success);
            }
        }

        private void ResolveShow(string key, AdShowResult result)
        {
            if (_showWaiters.TryGetValue(key, out var waiter))
            {
                _showWaiters.Remove(key);
                waiter.TrySetResult(result);
            }
        }
#endif

        private void InitializeMaxSdk(string[] selectiveInitAdUnitIds)
        {
#if APPLOVIN_MAX_SDK
            MaxSdk.InitializeSdk(selectiveInitAdUnitIds);
#endif
        }

        private void EmitRevenue(string unitId, AdFormat format, string adSource, double revenueUsd)
        {
            RevenuePaid?.Invoke(new AdRevenueSignal
            {
                UnitId = unitId,
                Provider = Provider,
                Format = format,
                AdSource = adSource,
                RevenueUsd = revenueUsd,
            });
        }

        private static string BuildKey(string unitId, AdFormat format)
        {
            return $"{unitId}:{format}";
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
#if APPLOVIN_MAX_SDK
            UnregisterCallbacks();
#endif
            _initializeWaiter?.TrySetCanceled();
            _initializeWaiter = null;
            _initializeRequested = false;
            _loadWaiters.Clear();
            _showWaiters.Clear();
            _loadedInterstitials.Clear();
            _loadedRewarded.Clear();
            _createdBanners.Clear();
            _rewardGrantedByUnit.Clear();
            RevenuePaid = null;
        }
    }
}
