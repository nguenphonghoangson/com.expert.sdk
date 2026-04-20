namespace SDK.Domain.Ads
{
    public interface IAdNetworkAdapterFactory
    {
        IAdNetworkAdapter Create(AdProvider provider);
    }
}
