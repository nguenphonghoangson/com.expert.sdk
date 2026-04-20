using System;
using System.Collections.Generic;
using SDK.Domain.Ads;
using SDK.Domain.Firebase;
using Reflex.Attributes;
using UnityEngine;

namespace SDK.Integration
{
    public sealed class AdsFirebaseBridge : MonoBehaviour
    {
        [SerializeField] private bool syncRevenueToFirebase = true;

        [Inject] private IAdsService _adsService;
        [Inject] private IFirebaseAnalyticsService _analyticsService;

        private IDisposable _revenueSubscription;

        private void Start()
        {
            if (syncRevenueToFirebase && _adsService != null && _analyticsService != null)
            {
                _revenueSubscription = _adsService.OnRevenuePaid.Subscribe(OnRevenuePaid);
            }
        }

        private void OnDestroy()
        {
            _revenueSubscription?.Dispose();
        }

        private void OnRevenuePaid(AdRevenueEvent evt)
        {
            _analyticsService?.LogEvent(FirebaseConstants.Events.AdImpression, new Dictionary<string, object>
            {
                { FirebaseConstants.Params.AdPlatform,  evt.Provider.ToString() },
                { FirebaseConstants.Params.AdSource,    evt.AdSource },
                { FirebaseConstants.Params.AdFormat,    evt.Format.ToString() },
                { FirebaseConstants.Params.AdUnitName,  evt.AdUnitId },
                { FirebaseConstants.Params.Currency,    "USD" },
                { FirebaseConstants.Params.Value,       evt.RevenueUsd },
                { "country",                            evt.CountryCode },
            });
        }
    }
}
