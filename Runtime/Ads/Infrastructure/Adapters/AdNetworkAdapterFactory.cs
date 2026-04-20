using System;
using SDK.Domain.Ads;

namespace SDK.Infrastructure.Ads
{
    public sealed class AdNetworkAdapterFactory : IAdNetworkAdapterFactory
    {
        public IAdNetworkAdapter Create(AdProvider provider)
        {
            return provider switch
            {
                AdProvider.AppLovinMax => new AppLovinMaxAdapter(),
                AdProvider.AdMob => throw new NotSupportedException("AdMob adapter is not implemented in this MAX-only setup."),
                _ => throw new ArgumentOutOfRangeException(nameof(provider), provider, "Unknown ad provider"),
            };
        }
    }
}
