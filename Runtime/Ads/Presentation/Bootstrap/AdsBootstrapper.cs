using System;
using Cysharp.Threading.Tasks;
using SDK.Domain.Ads;
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

        /// <summary>
        /// Registers ads-related dependencies into the DI container.
        /// </summary>
        /// <param name="builder">Container builder.</param>
        public void InstallBindings(ContainerBuilder builder)
        {
            if (adsConfig == null)
            {
                throw new InvalidOperationException("AdsBootstrapper requires AdsConfig asset.");
            }

            builder.RegisterValue(adsConfig);
            builder.RegisterType(typeof(AppLovinMaxAdapter), new[] { typeof(IAdNetworkAdapter) }, Lifetime.Singleton, Resolution.Lazy);
            builder.RegisterType(typeof(AdsService), new[] { typeof(IAdsService) }, Lifetime.Singleton, Resolution.Lazy);
        }

        private void Start()
        {
            if (!autoInitialize) return;

            var adUnitIds = adsConfig.GetSelectiveInitAdUnitIds();
            _adsService.InitializeAsync(adUnitIds, this.destroyCancellationToken).Forget();
        }

        /// <summary>
        /// Manually initializes the ads SDK from the current config.
        /// </summary>
        [ContextMenu("Initialize Ads SDK")]
        public void InitializeAdsSdk()
        {
            var adUnitIds = adsConfig != null ? adsConfig.GetSelectiveInitAdUnitIds() : null;
            _adsService?.InitializeAsync(adUnitIds, this.destroyCancellationToken).Forget();
        }
    }
}
