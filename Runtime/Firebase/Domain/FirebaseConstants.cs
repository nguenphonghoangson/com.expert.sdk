namespace SDK.Domain.Firebase
{
    public static class FirebaseConstants
    {
        public static class Events
        {
            public const string LevelStart = "level_start";
            public const string LevelComplete = "level_complete";
            public const string LevelFail = "level_fail";
            public const string TutorialBegin = "tutorial_begin";
            public const string TutorialComplete = "tutorial_complete";
            public const string EarnVirtualCurrency = "earn_virtual_currency";
            public const string SpendVirtualCurrency = "spend_virtual_currency";
            public const string AdImpression = "ad_impression";
            public const string ScreenView = "screen_view";
        }

        public static class Params
        {
            public const string Level = "level";
            public const string LevelName = "level_name";
            public const string Success = "success";
            public const string Score = "score";
            public const string VirtualCurrencyName = "virtual_currency_name";
            public const string Value = "value";
            public const string AdPlatform = "ad_platform";
            public const string AdSource = "ad_source";
            public const string AdFormat = "ad_format";
            public const string AdUnitName = "ad_unit_name";
            public const string Currency = "currency";
            public const string ScreenName = "screen_name";
            public const string ScreenClass = "screen_class";
        }

        public static class UserProperties
        {
            public const string UserLevel = "user_level";
            public const string TotalSpend = "total_spend";
            public const string DaysPlayed = "days_played";
        }

        public static class Configs
        {
            public const string InterstitialInterval = "interstitial_interval";
            public const string StartingCoins = "starting_coins";
            public const string DifficultyMultiplier = "difficulty_multiplier";
        }
    }
}
