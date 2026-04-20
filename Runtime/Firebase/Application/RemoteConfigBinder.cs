using System;
using System.Reflection;
using SDK.Domain.Firebase;
using UnityEngine;

namespace SDK.Application.Firebase
{
    public sealed class RemoteConfigBinder
    {
        public void Bind(object target, IRemoteConfigService configService)
        {
            if (target == null) return;

            var type = target.GetType();
            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            // Fields
            var fields = type.GetFields(flags);
            foreach (var field in fields)
            {
                var attr = field.GetCustomAttribute<RemoteConfigKeyAttribute>();
                if (attr != null)
                {
                    var value = GetValue(configService, attr.Key, field.FieldType, attr.DefaultValue);
                    field.SetValue(target, value);
                }
            }

            // Properties
            var properties = type.GetProperties(flags);
            foreach (var prop in properties)
            {
                if (!prop.CanWrite) continue;
                var attr = prop.GetCustomAttribute<RemoteConfigKeyAttribute>();
                if (attr != null)
                {
                    var value = GetValue(configService, attr.Key, prop.PropertyType, attr.DefaultValue);
                    prop.SetValue(target, value);
                }
            }
        }

        private object GetValue(IRemoteConfigService configService, string key, Type targetType, object defaultValue)
        {
            try
            {
                if (targetType == typeof(string)) return configService.GetString(key, defaultValue?.ToString() ?? "");
                if (targetType == typeof(int)) return configService.GetInt(key, defaultValue is int i ? i : 0);
                if (targetType == typeof(float)) return configService.GetFloat(key, defaultValue is float f ? f : 0f);
                if (targetType == typeof(bool)) return configService.GetBool(key, defaultValue is bool b ? b : false);
                
                // Fallback for double if needed
                if (targetType == typeof(double)) return (double)configService.GetFloat(key, (float)(defaultValue is double d ? d : 0.0));
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[RemoteConfigBinder] Failed to get value for {key}: {ex.Message}. Using default.");
            }

            return defaultValue;
        }
    }
}
