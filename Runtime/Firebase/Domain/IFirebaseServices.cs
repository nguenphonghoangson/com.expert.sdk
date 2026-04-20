using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using SDK.Domain.Common;

namespace SDK.Domain.Firebase
{
    public interface IFirebaseInitializer
    {
        UniTask InitializeAsync(CancellationToken cancellationToken);
    }

    public interface IFirebaseAnalyticsService
    {
        void LogEvent(string eventName, IReadOnlyDictionary<string, object> parameters = null);
        void SetUserProperty(string name, string value);
        void SetCurrentScreen(string screenName);
    }

    public interface IRemoteConfigService
    {
        IObservableStream<RemoteConfigPayload> OnConfigUpdated { get; }
        UniTask InitializeAsync(IReadOnlyDictionary<string, string> defaults, CancellationToken cancellationToken);
        UniTask<RemoteConfigPayload> FetchAsync(CancellationToken cancellationToken);
        string GetString(string key, string fallback = "");
        int GetInt(string key, int fallback = 0);
        float GetFloat(string key, float fallback = 0f);
        bool GetBool(string key, bool fallback = false);
    }

    public interface ICrashlyticsService
    {
        void Initialize();
        void Log(string message);
        void SetUserId(string userId);
        void SetCustomKey(string key, string value);
        void RecordException(System.Exception exception);
    }
}
