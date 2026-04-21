using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using SDK.Domain.Ads;
using SDK.Domain.Common;
using SDK.Infrastructure.Reactive;

namespace SDK.Infrastructure.Ads
{
    public sealed class AdsService : IAdsService, IDisposable
    {
        private readonly IAdNetworkAdapter _adapter;
        private readonly Dictionary<string, AdUnitRuntimeState> _runtimeStateByAdUnit = new Dictionary<string, AdUnitRuntimeState>();
        private readonly ObservableStream<AdLoadEvent> _onAdLoaded = new ObservableStream<AdLoadEvent>();
        private readonly ObservableStream<AdShowEvent> _onAdShown = new ObservableStream<AdShowEvent>();
        private readonly ObservableStream<AdRevenueEvent> _onRevenuePaid = new ObservableStream<AdRevenueEvent>();
        private bool _initialized;
        private bool _disposed;

        /// <summary>
        /// Creates an ads service with a concrete ad network adapter.
        /// </summary>
        /// <param name="adapter">Ad network adapter.</param>
        public AdsService(IAdNetworkAdapter adapter)
        {
            _adapter = adapter;
            if (_adapter is IAdRevenueEventProvider revenueProvider)
            {
                revenueProvider.RevenuePaid += HandleRevenuePaid;
            }
        }

        public IObservableStream<AdLoadEvent> OnAdLoaded => _onAdLoaded;
        public IObservableStream<AdShowEvent> OnAdShown => _onAdShown;
        public IObservableStream<AdRevenueEvent> OnRevenuePaid => _onRevenuePaid;
        public bool IsInitialized => _initialized;

        /// <summary>
        /// Initializes the underlying ad network once.
        /// </summary>
        /// <param name="selectiveInitAdUnitIds">Optional ad unit ids for selective initialization.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public UniTask InitializeAsync(string[] selectiveInitAdUnitIds,CancellationToken cancellationToken)
        {
            return InitializedAsync(selectiveInitAdUnitIds,cancellationToken);
        }

        /// <summary>
        /// Preloads an ad and publishes load state.
        /// </summary>
        /// <param name="adUnitId">Ad unit identifier.</param>
        /// <param name="format">Ad format.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async UniTask PreloadAsync(string adUnitId, AdFormat format, CancellationToken cancellationToken)
        {
            if (_disposed || string.IsNullOrWhiteSpace(adUnitId))
            {
                return;
            }
            var success = await _adapter.LoadAsync(adUnitId, format, cancellationToken);
            PublishLoad(adUnitId, format, success, success ? null : "Load failed");
            GetState(adUnitId).LastLoadState = success ? AdLoadState.Loaded : AdLoadState.Failed;
        }

        /// <summary>
        /// Shows an interstitial ad.
        /// </summary>
        /// <param name="adUnitId">Ad unit identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Show result.</returns>
        public UniTask<AdShowResult> ShowInterstitialAsync(string adUnitId, CancellationToken cancellationToken)
        {
            return ShowInternalAsync(adUnitId, AdFormat.Interstitial, cancellationToken);
        }

        /// <summary>
        /// Shows a rewarded ad.
        /// </summary>
        /// <param name="adUnitId">Ad unit identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Show result.</returns>
        public UniTask<AdShowResult> ShowRewardedAsync(string adUnitId, CancellationToken cancellationToken)
        {
            return ShowInternalAsync(adUnitId, AdFormat.Rewarded, cancellationToken);
        }

        /// <summary>
        /// Shows a banner ad.
        /// </summary>
        /// <param name="adUnitId">Ad unit identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Show result.</returns>
        public UniTask<AdShowResult> ShowBannerAsync(string adUnitId, CancellationToken cancellationToken)
        {
            return ShowInternalAsync(adUnitId, AdFormat.Banner, cancellationToken);
        }

        /// <summary>
        /// Returns whether an ad is ready to show.
        /// </summary>
        /// <param name="adUnitId">Ad unit identifier.</param>
        /// <param name="format">Ad format.</param>
        /// <returns>True when ready.</returns>
        public bool IsReady(string adUnitId, AdFormat format)
        {
            if (_disposed || string.IsNullOrWhiteSpace(adUnitId))
            {
                return false;
            }

            return _adapter.IsReady(adUnitId, format);
        }

        /// <summary>
        /// Returns mutable runtime state for an ad unit.
        /// </summary>
        /// <param name="adUnitId">Ad unit identifier.</param>
        /// <returns>Runtime state instance.</returns>
        public AdUnitRuntimeState GetState(string adUnitId)
        {
            if (_disposed)
            {
                return new AdUnitRuntimeState
                {
                    LastError = "Ads service disposed",
                };
            }

            if (!_runtimeStateByAdUnit.TryGetValue(adUnitId, out var state))
            {
                state = new AdUnitRuntimeState();
                _runtimeStateByAdUnit[adUnitId] = state;
            }

            return state;
        }

        /// <summary>
        /// Unsubscribes events and disposes adapter resources.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            if (_adapter is IAdRevenueEventProvider revenueProvider)
            {
                revenueProvider.RevenuePaid -= HandleRevenuePaid;
            }

            if (_adapter is IDisposable disposable)
            {
                disposable.Dispose();
            }

            _runtimeStateByAdUnit.Clear();
        }

        private async UniTask<AdShowResult> ShowInternalAsync(string adUnitId, AdFormat format, CancellationToken cancellationToken)
        {
            if (_disposed || string.IsNullOrWhiteSpace(adUnitId))
            {
                return AdShowResult.Failed;
            }
            if (!_adapter.IsReady(adUnitId, format))
            {
                var loaded = await _adapter.LoadAsync(adUnitId, format, cancellationToken);
                PublishLoad(adUnitId, format, loaded, loaded ? null : "Load before show failed");
                if (!loaded)
                {
                    GetState(adUnitId).LastError = "Load before show failed";
                    return AdShowResult.NotReady;
                }
            }

            var result = await _adapter.ShowAsync(adUnitId, format, cancellationToken);
            var rewardGranted = format == AdFormat.Rewarded &&
                                result == AdShowResult.Success &&
                                (_adapter as IRewardStatusProvider)?.ConsumeRewardResult(adUnitId) == true;

            PublishShow(adUnitId, format, result, rewardGranted, result == AdShowResult.Success ? null : "Show failed");

            var state = GetState(adUnitId);
            if (result == AdShowResult.Success)
            {
                state.LastShownUtc = DateTime.UtcNow;
                state.LastError = null;
            }
            else
            {
                state.LastError = "Show failed";
            }

            return result;
        }

        private async UniTask InitializedAsync(string[] selectiveInitAdUnitIds,CancellationToken cancellationToken)
        {
            if (_initialized) return;
            await _adapter.InitializeAsync(selectiveInitAdUnitIds,cancellationToken);
            _initialized = true;
        }

        private void PublishLoad(string adUnitId, AdFormat format, bool success, string error)
        {
            _onAdLoaded.Publish(new AdLoadEvent
            {
                AdUnitId = adUnitId,
                Provider = _adapter.Provider,
                Format = format,
                Success = success,
                Error = error,
            });
        }

        private void PublishShow(string adUnitId, AdFormat format, AdShowResult result, bool rewardGranted, string error)
        {
            _onAdShown.Publish(new AdShowEvent
            {
                AdUnitId = adUnitId,
                Provider = _adapter.Provider,
                Format = format,
                Result = result,
                RewardGranted = rewardGranted,
                Error = error,
            });
        }

        private void HandleRevenuePaid(AdRevenueSignal signal)
        {
            if (_disposed)
            {
                return;
            }

            _onRevenuePaid.Publish(new AdRevenueEvent
            {
                AdUnitId = signal.AdUnitId,
                Provider = signal.Provider,
                Format = signal.Format,
                AdSource = signal.AdSource,
                CountryCode = "N/A",
                RevenueUsd = signal.RevenueUsd,
                TimestampUtc = DateTime.UtcNow,
            });
        }
    }
}
