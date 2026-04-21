using SDK.Domain.Firebase;

namespace SDK.Infrastructure.Config
{
    public sealed class GameRemoteConfig : IGameRemoteConfig
    {
        private readonly IRemoteConfigService _remoteConfigService;
        private readonly FirebaseDefaultsProvider _defaultsProvider;

        public GameRemoteConfig(IRemoteConfigService remoteConfigService, FirebaseDefaultsProvider defaultsProvider)
        {
            _remoteConfigService = remoteConfigService;
            _defaultsProvider = defaultsProvider;
        }

        /// <summary>
        /// Gets interstitial interval from remote config.
        /// </summary>
        public int InterstitialInterval => _remoteConfigService.GetInt(
            FirebaseConstants.Configs.InterstitialInterval,
            _defaultsProvider.GetInt(FirebaseConstants.Configs.InterstitialInterval));

        /// <summary>
        /// Gets starting coins from remote config.
        /// </summary>
        public int StartingCoins => _remoteConfigService.GetInt(
            FirebaseConstants.Configs.StartingCoins,
            _defaultsProvider.GetInt(FirebaseConstants.Configs.StartingCoins));

        /// <summary>
        /// Gets gameplay difficulty multiplier from remote config.
        /// </summary>
        public float DifficultyMultiplier => _remoteConfigService.GetFloat(
            FirebaseConstants.Configs.DifficultyMultiplier,
            _defaultsProvider.GetFloat(FirebaseConstants.Configs.DifficultyMultiplier));
    }
}
