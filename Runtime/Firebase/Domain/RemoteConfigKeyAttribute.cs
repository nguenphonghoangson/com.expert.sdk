using System;

namespace SDK.Domain.Firebase
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class RemoteConfigKeyAttribute : Attribute
    {
        public string Key { get; }
        public object DefaultValue { get; set; }

        public RemoteConfigKeyAttribute(string key)
        {
            Key = key;
        }

        public RemoteConfigKeyAttribute(string key, object defaultValue)
        {
            Key = key;
            DefaultValue = defaultValue;
        }
    }
}
