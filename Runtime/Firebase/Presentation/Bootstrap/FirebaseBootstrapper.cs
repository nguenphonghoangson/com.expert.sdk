using System;
using System.Threading;
using Reflex.Attributes;
using SDK.Application.Firebase;
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

        [Inject] private FirebaseInitializationUseCase _initializeUseCase;
        [Inject] private FirebaseDefaultsProvider _defaultsProvider;

        public void InstallBindings(ContainerBuilder builder)
        {
            if (firebaseConfig == null)
            {
                throw new InvalidOperationException("FirebaseBootstrapper requires FirebaseConfig asset.");
            }

            builder.RegisterValue(firebaseConfig);

            RegisterSingleton<FirebaseDefaultsProvider>(builder);
            RegisterSingleton<FirebaseInitializer, IFirebaseInitializer>(builder);
            RegisterSingleton<FirebaseAnalyticsAdapter, IFirebaseAnalyticsService>(builder);
            RegisterSingleton<FirebaseRemoteConfigAdapter, IRemoteConfigService>(builder);
            RegisterSingleton<FirebaseCrashlyticsAdapter, ICrashlyticsService>(builder);

            // Utils
            RegisterSingleton<RemoteConfigBinder>(builder);

            // Use Cases
            RegisterTransient<FirebaseInitializationUseCase>(builder);
            RegisterTransient<LogAnalyticsUseCase>(builder);
            RegisterTransient<GetRemoteConfigUseCase>(builder);
            RegisterTransient<BindRemoteConfigUseCase>(builder);
            RegisterTransient<ReportCrashUseCase>(builder);
        }

        private async void Start()
        {
            if (!autoInitialize) return;

            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                var defaults = _defaultsProvider.GetDefaults();
                await _initializeUseCase.ExecuteAsync(defaults, _cancellationTokenSource.Token);

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

        private static void RegisterSingleton<TConcrete>(ContainerBuilder builder)
        {
            builder.RegisterType(typeof(TConcrete), Lifetime.Singleton, Resolution.Lazy);
        }

        private static void RegisterSingleton<TConcrete, TContract>(ContainerBuilder builder)
        {
            builder.RegisterType(typeof(TConcrete), new[] { typeof(TContract) }, Lifetime.Singleton, Resolution.Lazy);
        }

        private static void RegisterTransient<TConcrete>(ContainerBuilder builder)
        {
            builder.RegisterType(typeof(TConcrete), Lifetime.Transient, Resolution.Lazy);
        }
    }
}
