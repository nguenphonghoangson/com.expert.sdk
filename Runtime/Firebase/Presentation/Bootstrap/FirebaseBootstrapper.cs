using System;
using System.Threading;
using Reflex.Attributes;
using SDK.Domain.Firebase;
using SDK.Infrastructure.Config;
using SDK.Infrastructure.Firebase;
using Reflex.Core;
using Reflex.Enums;
using UnityEngine;
using Resolution = Reflex.Enums.Resolution;

namespace SDK.Presentation.Bootstrap
{
    public sealed class FirebaseBootstrapper : MonoBehaviour, IInstaller
    {
        [Header("Firebase Config")]
        [SerializeField] private FirebaseConfigScriptableObject firebaseConfig;
        [SerializeField] private bool autoInitialize = true;

        private CancellationTokenSource _cancellationTokenSource;

        [Inject] private IFirebaseInitializer _initializer;
        [Inject] private IRemoteConfigService _remoteConfigService;
        [Inject] private ICrashlyticsService _crashlyticsService;
        [Inject] private FirebaseDefaultsProvider _defaultsProvider;

        /// <summary>
        /// Registers Firebase-related dependencies into the DI container.
        /// </summary>
        /// <param name="builder">Container builder.</param>
        public void InstallBindings(ContainerBuilder builder)
        {
            if (firebaseConfig == null)
            {
                throw new InvalidOperationException("FirebaseBootstrapper requires FirebaseConfig asset.");
            }

            builder.RegisterValue(firebaseConfig);

            builder.RegisterType(typeof(FirebaseDefaultsProvider), Lifetime.Singleton, Resolution.Lazy);
            builder.RegisterType(typeof(GameRemoteConfig), new[] { typeof(IGameRemoteConfig) }, Lifetime.Singleton, Resolution.Lazy);
            builder.RegisterType(typeof(FirebaseInitializer), new[] { typeof(IFirebaseInitializer) }, Lifetime.Singleton, Resolution.Lazy);
            builder.RegisterType(typeof(FirebaseAnalyticsAdapter), new[] { typeof(IFirebaseAnalyticsService) }, Lifetime.Singleton, Resolution.Lazy);
            builder.RegisterType(typeof(FirebaseRemoteConfigAdapter), new[] { typeof(IRemoteConfigService) }, Lifetime.Singleton, Resolution.Lazy);
            builder.RegisterType(typeof(FirebaseCrashlyticsAdapter), new[] { typeof(ICrashlyticsService) }, Lifetime.Singleton, Resolution.Lazy);
        }

        private async void Start()
        {
            if (!autoInitialize) return;

            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                var defaults = _defaultsProvider.GetDefaults();
                await _initializer.InitializeAsync(_cancellationTokenSource.Token);
                _crashlyticsService.Initialize();
                await _remoteConfigService.InitializeAsync(defaults, _cancellationTokenSource.Token);
                await _remoteConfigService.FetchAsync(_cancellationTokenSource.Token);
                _crashlyticsService.Log("Firebase Initialized");

                Debug.Log("[FirebaseBootstrapper] Initialization complete.");
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }

        private void OnDestroy()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }
    }
}
