using System.Threading;
using System;
using Cysharp.Threading.Tasks;

namespace SDK.Domain.Ads
{
    public interface IAdNetworkAdapter
    {
        AdProvider Provider { get; }
        UniTask InitializeAsync(string[] selectiveInitAdUnitIds,CancellationToken cancellationToken);
        UniTask<bool> LoadAsync(string adUnitId, AdFormat format, CancellationToken cancellationToken);
        bool IsReady(string unitId, AdFormat format);
        UniTask<AdShowResult> ShowAsync(string adUnitId, AdFormat format, CancellationToken cancellationToken);
    }

    public interface IRewardStatusProvider
    {
        bool ConsumeRewardResult(string unitId);
    }

    public interface IAdRevenueEventProvider
    {
        event Action<AdRevenueSignal> RevenuePaid;
    }

    public interface ISelectiveInitAdUnitsConfigurator
    {
        void SetSelectiveInitAdUnitIds(string[] adUnitIds);
    }
}
