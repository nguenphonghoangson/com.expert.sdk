namespace SDK.Domain.Ads
{
    public enum AdFormat
    {
        Banner = 0,
        Interstitial = 1,
        Rewarded = 2,
    }

    public enum AdProvider
    {
        AdMob = 0,
        AppLovinMax = 1,
    }

    public enum AdLoadState
    {
        Idle = 0,
        Loading = 1,
        Loaded = 2,
        Failed = 3,
    }

    public enum AdShowResult
    {
        Success = 0,
        NotReady = 1,
        Capped = 2,
        Failed = 3,
    }
}
