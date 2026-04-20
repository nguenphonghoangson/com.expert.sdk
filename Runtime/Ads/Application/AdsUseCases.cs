using System.Threading;
using Cysharp.Threading.Tasks;
using SDK.Domain.Ads;

namespace SDK.Application.Ads
{
    public sealed class PreloadAdsUseCase
    {
        private readonly IAdsService _adsService;

        public PreloadAdsUseCase(IAdsService adsService)
        {
            _adsService = adsService;
        }

        public UniTask ExecuteAsync(string adUnitId, AdFormat format, CancellationToken cancellationToken)
        {
            return _adsService.PreloadAsync(adUnitId, format, cancellationToken);
        }
    }

    public sealed class ShowAdsUseCase
    {
        private readonly IAdsService _adsService;

        public ShowAdsUseCase(IAdsService adsService)
        {
            _adsService = adsService;
        }

        public UniTask<AdShowResult> ExecuteInterstitialAsync(string adUnitId, CancellationToken cancellationToken)
        {
            return _adsService.ShowInterstitialAsync(adUnitId, cancellationToken);
        }

        public UniTask<AdShowResult> ExecuteRewardedAsync(string adUnitId, CancellationToken cancellationToken)
        {
            return _adsService.ShowRewardedAsync(adUnitId, cancellationToken);
        }

        public UniTask<AdShowResult> ExecuteBannerAsync(string adUnitId, CancellationToken cancellationToken)
        {
            return _adsService.ShowBannerAsync(adUnitId, cancellationToken);
        }
    }
}
