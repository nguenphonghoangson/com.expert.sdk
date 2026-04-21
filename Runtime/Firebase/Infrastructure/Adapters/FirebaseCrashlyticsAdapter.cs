using SDK.Domain.Firebase;
using UnityEngine;
#if FIREBASE_CRASHLYTICS
using Firebase.Crashlytics;
#endif

namespace SDK.Infrastructure.Firebase
{
    public sealed class FirebaseCrashlyticsAdapter : ICrashlyticsService
    {
        /// <summary>
        /// Configures Crashlytics collection and default user id.
        /// </summary>
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

        /// <summary>
        /// Writes a message to Crashlytics logs.
        /// </summary>
        /// <param name="message">Log message.</param>
        public void Log(string message)
        {
#if FIREBASE_CRASHLYTICS
            Crashlytics.Log(message);
#else
            Debug.Log($"[Crashlytics] (Mock) Log: {message}");
#endif
        }

        /// <summary>
        /// Sets the user id associated with crash reports.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        public void SetUserId(string userId)
        {
#if FIREBASE_CRASHLYTICS
            Crashlytics.SetUserId(userId);
#else
            Debug.Log($"[Crashlytics] (Mock) SetUserId: {userId}");
#endif
        }

        /// <summary>
        /// Adds a custom key/value pair to crash reports.
        /// </summary>
        /// <param name="key">Custom key.</param>
        /// <param name="value">Custom value.</param>
        public void SetCustomKey(string key, string value)
        {
#if FIREBASE_CRASHLYTICS
            Crashlytics.SetCustomKey(key, value);
#else
            Debug.Log($"[Crashlytics] (Mock) SetCustomKey: {key}={value}");
#endif
        }

        /// <summary>
        /// Records a handled exception in Crashlytics.
        /// </summary>
        /// <param name="exception">Exception to record.</param>
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
