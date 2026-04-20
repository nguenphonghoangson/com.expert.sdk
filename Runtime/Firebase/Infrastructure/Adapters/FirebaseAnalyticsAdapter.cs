using System.Collections.Generic;
using SDK.Domain.Firebase;
using UnityEngine;
#if FIREBASE_ANALYTICS
using Firebase.Analytics;
#endif

namespace SDK.Infrastructure.Firebase
{
    public sealed class FirebaseAnalyticsAdapter : IFirebaseAnalyticsService
    {
        public void LogEvent(string eventName, IReadOnlyDictionary<string, object> parameters = null)
        {
#if FIREBASE_ANALYTICS
            if (parameters == null || parameters.Count == 0)
            {
                FirebaseAnalytics.LogEvent(eventName);
                return;
            }

            var firebaseParameters = new Parameter[parameters.Count];
            var index = 0;
            foreach (var pair in parameters)
            {
                if (pair.Value is long l) firebaseParameters[index] = new Parameter(pair.Key, l);
                else if (pair.Value is double d) firebaseParameters[index] = new Parameter(pair.Key, d);
                else if (pair.Value is string s) firebaseParameters[index] = new Parameter(pair.Key, s);
                else firebaseParameters[index] = new Parameter(pair.Key, pair.Value?.ToString() ?? "");
                index++;
            }

            FirebaseAnalytics.LogEvent(eventName, firebaseParameters);
#else
            Debug.Log($"[FirebaseAnalytics] (Mock) {eventName} params count: {parameters?.Count ?? 0}");
#endif
        }

        public void SetUserProperty(string name, string value)
        {
#if FIREBASE_ANALYTICS
            FirebaseAnalytics.SetUserProperty(name, value);
#else
            Debug.Log($"[FirebaseAnalytics] (Mock) UserProperty {name}={value}");
#endif
        }

        public void SetCurrentScreen(string screenName)
        {
#if FIREBASE_ANALYTICS
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventScreenView, 
                new Parameter(FirebaseAnalytics.ParameterScreenName, screenName));
#else
            Debug.Log($"[FirebaseAnalytics] (Mock) ScreenView: {screenName}");
#endif
        }
    }
}
