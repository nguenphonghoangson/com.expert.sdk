using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using SDK.Domain.Firebase;

namespace SDK.Application.Firebase
{
    public sealed class FirebaseInitializationUseCase
    {
        private readonly IFirebaseInitializer _initializer;
        private readonly IRemoteConfigService _remoteConfigService;
        private readonly IFirebaseAnalyticsService _analytics;
        private readonly ICrashlyticsService _crashlytics;

        public FirebaseInitializationUseCase(
            IFirebaseInitializer initializer, 
            IRemoteConfigService remoteConfigService,
            IFirebaseAnalyticsService analytics,
            ICrashlyticsService crashlytics)
        {
            _initializer = initializer;
            _remoteConfigService = remoteConfigService;
            _analytics = analytics;
            _crashlytics = crashlytics;
        }

        public async UniTask ExecuteAsync(IReadOnlyDictionary<string, string> defaults, CancellationToken cancellationToken)
        {
            await _initializer.InitializeAsync(cancellationToken);
            
            _crashlytics.Initialize();
            
            await _remoteConfigService.InitializeAsync(defaults, cancellationToken);
            await _remoteConfigService.FetchAsync(cancellationToken);
            
            _crashlytics.Log("Firebase Initialized");
        }
    }

    public sealed class LogAnalyticsUseCase
    {
        private readonly IFirebaseAnalyticsService _analytics;

        public LogAnalyticsUseCase(IFirebaseAnalyticsService analytics)
        {
            _analytics = analytics;
        }

        public void LogLevelStart(int level)
        {
            _analytics.LogEvent(FirebaseConstants.Events.LevelStart, new Dictionary<string, object>
            {
                { FirebaseConstants.Params.Level, level }
            });
        }

        public void LogLevelComplete(int level, int score)
        {
            _analytics.LogEvent(FirebaseConstants.Events.LevelComplete, new Dictionary<string, object>
            {
                { FirebaseConstants.Params.Level, level },
                { FirebaseConstants.Params.Score, score },
                { FirebaseConstants.Params.Success, 1 }
            });
        }

        public void LogEvent(string eventName, IReadOnlyDictionary<string, object> parameters = null)
        {
            _analytics.LogEvent(eventName, parameters);
        }
    }

    public sealed class GetRemoteConfigUseCase
    {
        private readonly IRemoteConfigService _remoteConfig;

        public GetRemoteConfigUseCase(IRemoteConfigService remoteConfig)
        {
            _remoteConfig = remoteConfig;
        }
        public string GetString(string key, string fallback = "") => _remoteConfig.GetString(key, fallback);
        public int GetInt(string key, int fallback = 0) => _remoteConfig.GetInt(key, fallback);
        public float GetFloat(string key, float fallback = 0f) => _remoteConfig.GetFloat(key, fallback);
        public bool GetBool(string key, bool fallback = false) => _remoteConfig.GetBool(key, fallback);
    }

    public sealed class BindRemoteConfigUseCase
    {
        private readonly IRemoteConfigService _remoteConfig;
        private readonly RemoteConfigBinder _binder;

        public BindRemoteConfigUseCase(IRemoteConfigService remoteConfig, RemoteConfigBinder binder)
        {
            _remoteConfig = remoteConfig;
            _binder = binder;
        }

        public void Execute(object target)
        {
            _binder.Bind(target, _remoteConfig);
        }
    }

    public sealed class ReportCrashUseCase
    {
        private readonly ICrashlyticsService _crashlytics;

        public ReportCrashUseCase(ICrashlyticsService crashlytics)
        {
            _crashlytics = crashlytics;
        }

        public void Log(string message) => _crashlytics.Log(message);
        public void SetUserId(string userId) => _crashlytics.SetUserId(userId);
        public void SetCustomKey(string key, string value) => _crashlytics.SetCustomKey(key, value);
        public void CaptureException(System.Exception ex) => _crashlytics.RecordException(ex);
    }
}
