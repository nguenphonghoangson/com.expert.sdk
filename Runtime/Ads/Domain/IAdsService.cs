using System.Threading;
using Cysharp.Threading.Tasks;
using SDK.Domain.Common;

namespace SDK.Domain.Ads
{
    public interface IAdsService
    {
        IObservableStream<AdLoadEvent> OnAdLoaded { get; }
        IObservableStream<AdShowEvent> OnAdShown { get; }
        IObservableStream<AdRevenueEvent> OnRevenuePaid { get; }
        bool IsInitialized { get; }

        /// <summary>
        /// Initializes ads service and underlying ad network.
        /// </summary>
        /// <param name="selectiveInitAdUnitIds">Optional ad unit ids used for selective initialization.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        UniTask InitializeAsync(string[] selectiveInitAdUnitIds,CancellationToken cancellationToken);

        /// <summary>
        /// Preloads an ad for a given unit and format.
        /// </summary>
        /// <param name="adUnitId">Ad unit identifier.</param>
        /// <param name="format">Ad format.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        UniTask PreloadAsync(string adUnitId, AdFormat format, CancellationToken cancellationToken);

        /// <summary>
        /// Shows an interstitial ad.
        /// </summary>
        /// <param name="adUnitId">Ad unit identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Result of the show operation.</returns>
        UniTask<AdShowResult> ShowInterstitialAsync(string adUnitId, CancellationToken cancellationToken);

        /// <summary>
        /// Shows a rewarded ad.
        /// </summary>
        /// <param name="adUnitId">Ad unit identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Result of the show operation.</returns>
        UniTask<AdShowResult> ShowRewardedAsync(string adUnitId, CancellationToken cancellationToken);

        /// <summary>
        /// Shows a banner ad.
        /// </summary>
        /// <param name="adUnitId">Ad unit identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Result of the show operation.</returns>
        UniTask<AdShowResult> ShowBannerAsync(string adUnitId, CancellationToken cancellationToken);

        /// <summary>
        /// Returns whether an ad is ready to be shown.
        /// </summary>
        /// <param name="adUnitId">Ad unit identifier.</param>
        /// <param name="format">Ad format.</param>
        /// <returns>True when ad is ready.</returns>
        bool IsReady(string adUnitId, AdFormat format);

        /// <summary>
        /// Gets runtime state for an ad unit.
        /// </summary>
        /// <param name="adUnitId">Ad unit identifier.</param>
        /// <returns>Runtime state object.</returns>
        AdUnitRuntimeState GetState(string adUnitId);
    }
}
