using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using SDK.Domain.Firebase;
using UnityEngine;
#if FIREBASE_SDK
using Firebase;
#endif

namespace SDK.Infrastructure.Firebase
{
    public sealed class FirebaseInitializer : IFirebaseInitializer
    {
        private bool _isInitialized;

        public async UniTask InitializeAsync(CancellationToken cancellationToken)
        {
            if (_isInitialized) return;

#if FIREBASE_SDK
            try
            {
                var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync().AsUniTask();
                if (dependencyStatus == DependencyStatus.Available)
                {
                    _isInitialized = true;
                    Debug.Log("[Firebase] Initialized successfully.");
                }
                else
                {
                    Debug.LogError($"[Firebase] Could not resolve dependencies: {dependencyStatus}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Firebase] Initialization exception: {ex.Message}");
            }
#else
            await UniTask.Delay(100, cancellationToken: cancellationToken);
            _isInitialized = true;
            Debug.Log("[Firebase] (Mock) Initialized.");
#endif
        }
    }
}
