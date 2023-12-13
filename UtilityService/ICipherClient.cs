
namespace AZ_WebApp1.UtilityService
{
    public interface ICipherClient
    {
        void Dispose();
        Task<string> ProcessTextAsync(DataPackage textData);
    }
}