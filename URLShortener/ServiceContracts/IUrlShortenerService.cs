using URLShortener.Models;

namespace URLShortener.ServiceContracts
{
    public interface IUrlShortenerService
    {
        Task<ServiceResponse> Shorter(string originalUrl);
        Task<ServiceResponse> CustomUrl(string originalUrl, string customCode);
        Task<string?> GetOriginalUrl(string shortCode);
    }
}
