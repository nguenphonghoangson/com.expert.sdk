using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using SDK.Domain.Common;
using SDK.Domain.Firebase;
using SDK.Infrastructure.Reactive;
using UnityEngine;
#if FIREBASE_REMOTE_CONFIG
using Firebase.RemoteConfig;
#endif

namespace SDK.Infrastructure.Firebase
{
    public sealed class FirebaseRemoteConfigAdapter : IRemoteConfigService
    {
        private readonly ObservableStream<RemoteConfigPayload> _onConfigUpdated = new ObservableStream<RemoteConfigPayload>();
        public IObservableStream<RemoteConfigPayload> OnConfigUpdated => _onConfigUpdated;

        /// <summary>
        /// Applies default remote config values.
        /// </summary>
        /// <param name="defaults">Default key/value pairs.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async UniTask InitializeAsync(IReadOnlyDictionary<string, string> defaults, CancellationToken cancellationToken)
        {
#if FIREBASE_REMOTE_CONFIG
            if (defaults != null)
            {
                var firebaseDefaults = new Dictionary<string, object>();
                foreach (var pair in defaults) firebaseDefaults[pair.Key] = pair.Value;
                await FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(firebaseDefaults);
            }
#endif
            await UniTask.CompletedTask;
        }

        /// <summary>
        /// Fetches and activates latest remote config values.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Current remote config payload.</returns>
        public async UniTask<RemoteConfigPayload> FetchAsync(CancellationToken cancellationToken)
        {
#if FIREBASE_REMOTE_CONFIG
            try
            {
                await FirebaseRemoteConfig.DefaultInstance.FetchAndActivateAsync();
                var payload = ToPayload();
                _onConfigUpdated.Publish(payload);
                return payload;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[FirebaseRemoteConfig] Fetch failed: {ex.Message}");
                return new RemoteConfigPayload();
            }
#else
            await UniTask.Delay(100, cancellationToken: cancellationToken);
            return new RemoteConfigPayload();
#endif
        }

        /// <summary>
        /// Gets a string config value.
        /// </summary>
        /// <param name="key">Config key.</param>
        /// <param name="fallback">Fallback value.</param>
        /// <returns>String value.</returns>
        public string GetString(string key, string fallback = "")
        {
#if FIREBASE_REMOTE_CONFIG
            return FirebaseRemoteConfig.DefaultInstance.GetValue(key).StringValue;
#else
            return fallback;
#endif
        }

        /// <summary>
        /// Gets an integer config value.
        /// </summary>
        /// <param name="key">Config key.</param>
        /// <param name="fallback">Fallback value.</param>
        /// <returns>Integer value.</returns>
        public int GetInt(string key, int fallback = 0)
        {
#if FIREBASE_REMOTE_CONFIG
            return (int)FirebaseRemoteConfig.DefaultInstance.GetValue(key).LongValue;
#else
            return fallback;
#endif
        }

        /// <summary>
        /// Gets a float config value.
        /// </summary>
        /// <param name="key">Config key.</param>
        /// <param name="fallback">Fallback value.</param>
        /// <returns>Float value.</returns>
        public float GetFloat(string key, float fallback = 0f)
        {
#if FIREBASE_REMOTE_CONFIG
            return (float)FirebaseRemoteConfig.DefaultInstance.GetValue(key).DoubleValue;
#else
            return fallback;
#endif
        }

        /// <summary>
        /// Gets a boolean config value.
        /// </summary>
        /// <param name="key">Config key.</param>
        /// <param name="fallback">Fallback value.</param>
        /// <returns>Boolean value.</returns>
        public bool GetBool(string key, bool fallback = false)
        {
#if FIREBASE_REMOTE_CONFIG
            return FirebaseRemoteConfig.DefaultInstance.GetValue(key).BooleanValue;
#else
            return fallback;
#endif
        }

        private RemoteConfigPayload ToPayload()
        {
            var payload = new RemoteConfigPayload();
#if FIREBASE_REMOTE_CONFIG
            foreach (var key in FirebaseRemoteConfig.DefaultInstance.Keys)
            {
                payload.Entries.Add(new RemoteConfigEntry
                {
                    Key = key,
                    Value = FirebaseRemoteConfig.DefaultInstance.GetValue(key).StringValue,
                });
            }
#endif
            return payload;
        }
    }
}
