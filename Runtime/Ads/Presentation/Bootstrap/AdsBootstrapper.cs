using System;
using Cysharp.Threading.Tasks;
using SDK.Domain.Ads;
using SDK.Application.Ads;
using SDK.Infrastructure.Ads;
using SDK.Infrastructure.Config;
using Reflex.Core;
using Reflex.Enums;
using Reflex.Attributes;
using UnityEngine;
using Resolution = Reflex.Enums.Resolution;

namespace SDK.Presentation.Bootstrap
{
    public sealed class AdsBootstrapper : MonoBehaviour, IInstaller
    {
        [Header("Ads Config")]
        [SerializeField] private AdsConfigScriptableObject adsConfig;
        [SerializeField] private bool autoInitialize = true;

        [Inject] private IAdsService _adsService;

        public void InstallBindings(ContainerBuilder builder)
        {
            if (adsConfig == null)
            {
                throw new InvalidOperationException("AdsBootstrapper requires AdsConfig asset.");
            }

            builder.RegisterValue(adsConfig);
            RegisterSingleton<AdNetworkAdapterFactory, IAdNetworkAdapterFactory>(builder);
            RegisterSingleton<AdsService, IAdsService>(builder);

            // Use Cases
            RegisterTransient<PreloadAdsUseCase>(builder);
            RegisterTransient<ShowAdsUseCase>(builder);
        }

        private void Start()
        {
            if (!autoInitialize) return;

            var adUnitIds = adsConfig.GetSelectiveInitAdUnitIds();
            _adsService.InitializeAsync(adUnitIds, this.destroyCancellationToken).Forget();
        }

        [ContextMenu("Initialize Ads SDK")]
        public void InitializeAdsSdk()
        {
            var adUnitIds = adsConfig != null ? adsConfig.GetSelectiveInitAdUnitIds() : null;
            _adsService?.InitializeAsync(adUnitIds, this.destroyCancellationToken).Forget();
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
