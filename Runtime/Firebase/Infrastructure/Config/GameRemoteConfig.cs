using SDK.Domain.Firebase;

namespace SDK.Infrastructure.Config
{
    public sealed class GameRemoteConfig : IGameRemoteConfig
    {
        private readonly IRemoteConfigService _remoteConfigService;

        public GameRemoteConfig(IRemoteConfigService remoteConfigService)
        {
            _remoteConfigService = remoteConfigService;
        }

        /// <summary>
        /// Gets interstitial interval from remote config.
        /// </summary>
        public int InterstitialInterval => _remoteConfigService.GetInt(FirebaseConstants.Configs.InterstitialInterval, 3);

        /// <summary>
        /// Gets starting coins from remote config.
        /// </summary>
        public int StartingCoins => _remoteConfigService.GetInt(FirebaseConstants.Configs.StartingCoins, 0);

        /// <summary>
        /// Gets gameplay difficulty multiplier from remote config.
        /// </summary>
        public float DifficultyMultiplier => _remoteConfigService.GetFloat(FirebaseConstants.Configs.DifficultyMultiplier, 1f);
    }
}
