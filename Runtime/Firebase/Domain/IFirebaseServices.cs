using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using SDK.Domain.Common;

namespace SDK.Domain.Firebase
{
    public interface IFirebaseInitializer
    {
        /// <summary>
        /// Initializes Firebase dependencies and runtime services.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        UniTask InitializeAsync(CancellationToken cancellationToken);
    }

    public interface IFirebaseAnalyticsService
    {
        /// <summary>
        /// Logs an analytics event.
        /// </summary>
        /// <param name="eventName">Event name.</param>
        /// <param name="parameters">Optional event parameters.</param>
        void LogEvent(string eventName, IReadOnlyDictionary<string, object> parameters = null);

        /// <summary>
        /// Sets a Firebase user property.
        /// </summary>
        /// <param name="name">Property name.</param>
        /// <param name="value">Property value.</param>
        void SetUserProperty(string name, string value);

        /// <summary>
        /// Sets the current screen context for analytics.
        /// </summary>
        /// <param name="screenName">Screen name.</param>
        void SetCurrentScreen(string screenName);
    }

    public interface IRemoteConfigService
    {
        IObservableStream<RemoteConfigPayload> OnConfigUpdated { get; }

        /// <summary>
        /// Applies default values used by remote config.
        /// </summary>
        /// <param name="defaults">Key/value defaults.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        UniTask InitializeAsync(IReadOnlyDictionary<string, string> defaults, CancellationToken cancellationToken);

        /// <summary>
        /// Fetches and activates remote config values.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Fetched payload snapshot.</returns>
        UniTask<RemoteConfigPayload> FetchAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Returns a config string value.
        /// </summary>
        /// <param name="key">Config key.</param>
        /// <param name="fallback">Fallback value.</param>
        string GetString(string key, string fallback = "");

        /// <summary>
        /// Returns a config integer value.
        /// </summary>
        /// <param name="key">Config key.</param>
        /// <param name="fallback">Fallback value.</param>
        int GetInt(string key, int fallback = 0);

        /// <summary>
        /// Returns a config float value.
        /// </summary>
        /// <param name="key">Config key.</param>
        /// <param name="fallback">Fallback value.</param>
        float GetFloat(string key, float fallback = 0f);

        /// <summary>
        /// Returns a config boolean value.
        /// </summary>
        /// <param name="key">Config key.</param>
        /// <param name="fallback">Fallback value.</param>
        bool GetBool(string key, bool fallback = false);
    }

    public interface ICrashlyticsService
    {
        /// <summary>
        /// Initializes Crashlytics settings.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Logs a diagnostic message.
        /// </summary>
        /// <param name="message">Log message.</param>
        void Log(string message);

        /// <summary>
        /// Sets the user id used by crash reports.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        void SetUserId(string userId);

        /// <summary>
        /// Adds a custom key/value pair to crash reports.
        /// </summary>
        /// <param name="key">Custom key.</param>
        /// <param name="value">Custom value.</param>
        void SetCustomKey(string key, string value);

        /// <summary>
        /// Records a handled exception.
        /// </summary>
        /// <param name="exception">Exception instance.</param>
        void RecordException(System.Exception exception);
    }

    public interface IGameRemoteConfig
    {
        /// <summary>
        /// Gets interstitial interval from remote config.
        /// </summary>
        int InterstitialInterval { get; }

        /// <summary>
        /// Gets starting coins from remote config.
        /// </summary>
        int StartingCoins { get; }

        /// <summary>
        /// Gets gameplay difficulty multiplier from remote config.
        /// </summary>
        float DifficultyMultiplier { get; }
    }
}
