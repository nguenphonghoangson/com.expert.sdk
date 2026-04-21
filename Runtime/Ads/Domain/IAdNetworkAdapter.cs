using System.Threading;
using System;
using Cysharp.Threading.Tasks;

namespace SDK.Domain.Ads
{
    public interface IAdNetworkAdapter
    {
        AdProvider Provider { get; }

        /// <summary>
        /// Initializes the ad network SDK.
        /// </summary>
        /// <param name="selectiveInitAdUnitIds">Optional ad unit ids used for selective initialization.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        UniTask InitializeAsync(string[] selectiveInitAdUnitIds,CancellationToken cancellationToken);

        /// <summary>
        /// Loads an ad for a specific unit and format.
        /// </summary>
        /// <param name="adUnitId">Ad unit identifier.</param>
        /// <param name="format">Requested ad format.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>True when load succeeds.</returns>
        UniTask<bool> LoadAsync(string adUnitId, AdFormat format, CancellationToken cancellationToken);

        /// <summary>
        /// Indicates whether a specific ad unit is currently ready to show.
        /// </summary>
        /// <param name="unitId">Ad unit identifier.</param>
        /// <param name="format">Ad format.</param>
        /// <returns>True when the ad is ready.</returns>
        bool IsReady(string unitId, AdFormat format);

        /// <summary>
        /// Shows an ad for a specific unit and format.
        /// </summary>
        /// <param name="adUnitId">Ad unit identifier.</param>
        /// <param name="format">Ad format.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Result of the show operation.</returns>
        UniTask<AdShowResult> ShowAsync(string adUnitId, AdFormat format, CancellationToken cancellationToken);
    }

    public interface IRewardStatusProvider
    {
        /// <summary>
        /// Returns and clears the reward status for a rewarded ad unit.
        /// </summary>
        /// <param name="unitId">Ad unit identifier.</param>
        /// <returns>True when reward was granted for the last show.</returns>
        bool ConsumeRewardResult(string unitId);
    }

    public interface IAdRevenueEventProvider
    {
        /// <summary>
        /// Fired when ad revenue is reported by the network.
        /// </summary>
        event Action<AdRevenueSignal> RevenuePaid;
    }
}
