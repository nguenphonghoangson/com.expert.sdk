using System;

namespace SDK.Domain.Ads
{
    public sealed class AdLoadEvent
    {
        public string AdUnitId;
        public string UnitId;
        public AdProvider Provider;
        public AdFormat Format;
        public bool Success;
        public string Error;
    }

    public sealed class AdShowEvent
    {
        public string AdUnitId;
        public string UnitId;
        public AdProvider Provider;
        public AdFormat Format;
        public AdShowResult Result;
        public bool RewardGranted;
        public string Error;
    }

    public sealed class AdRevenueEvent
    {
        public string AdUnitId;
        public string UnitId;
        public AdProvider Provider;
        public AdFormat Format;
        public string AdSource;
        public string CountryCode;
        public double RevenueUsd;
        public DateTime TimestampUtc;
    }

    public sealed class AdRevenueSignal
    {
        public string UnitId;
        public AdProvider Provider;
        public AdFormat Format;
        public string AdSource;
        public double RevenueUsd;
    }

    public sealed class AdUnitRuntimeState
    {
        public DateTime LastShownUtc;
        public AdLoadState LastLoadState;
        public string LastError;
    }
}
