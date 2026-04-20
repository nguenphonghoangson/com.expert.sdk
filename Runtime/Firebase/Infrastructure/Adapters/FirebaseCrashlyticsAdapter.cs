using SDK.Domain.Firebase;
using UnityEngine;
#if FIREBASE_CRASHLYTICS
using Firebase.Crashlytics;
#endif

namespace SDK.Infrastructure.Firebase
{
    public sealed class FirebaseCrashlyticsAdapter : ICrashlyticsService
    {
        public void Initialize()
        {
#if FIREBASE_CRASHLYTICS
            Crashlytics.ReportUncaughtExceptionsAsFatal = true;
            Crashlytics.IsCrashlyticsCollectionEnabled = true;
            Crashlytics.SetUserId(SystemInfo.deviceUniqueIdentifier);
#else
            Debug.Log("[Crashlytics] (Mock) Initialized with Fatal Exception Reporting.");
#endif
        }

        public void Log(string message)
        {
#if FIREBASE_CRASHLYTICS
            Crashlytics.Log(message);
#else
            Debug.Log($"[Crashlytics] (Mock) Log: {message}");
#endif
        }

        public void SetUserId(string userId)
        {
#if FIREBASE_CRASHLYTICS
            Crashlytics.SetUserId(userId);
#else
            Debug.Log($"[Crashlytics] (Mock) SetUserId: {userId}");
#endif
        }

        public void SetCustomKey(string key, string value)
        {
#if FIREBASE_CRASHLYTICS
            Crashlytics.SetCustomKey(key, value);
#else
            Debug.Log($"[Crashlytics] (Mock) SetCustomKey: {key}={value}");
#endif
        }

        public void RecordException(System.Exception exception)
        {
#if FIREBASE_CRASHLYTICS
            Crashlytics.LogException(exception);
#else
            Debug.LogWarning($"[Crashlytics] (Mock) Exception Recorded: {exception.Message}");
#endif
        }
    }
}
