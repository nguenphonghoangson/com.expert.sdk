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
        UniTask InitializeAsync(string[] selectiveInitAdUnitIds,CancellationToken cancellationToken);
        UniTask PreloadAsync(string adUnitId, AdFormat format, CancellationToken cancellationToken);
        UniTask<AdShowResult> ShowInterstitialAsync(string adUnitId, CancellationToken cancellationToken);
        UniTask<AdShowResult> ShowRewardedAsync(string adUnitId, CancellationToken cancellationToken);
        UniTask<AdShowResult> ShowBannerAsync(string adUnitId, CancellationToken cancellationToken);
        bool IsReady(string adUnitId, AdFormat format);
        AdUnitRuntimeState GetState(string adUnitId);
    }
}
