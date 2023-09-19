using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using URLShortener.Extensions;
using URLShortener.Models;
using URLShortener.ServiceContracts;

namespace URLShortener.Services
{
    public class UrlShortenerService : IUrlShortenerService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly Random _random;
        private const string _base62 = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const int codeLength = 6;

        public UrlShortenerService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
            _random = new();
        }
        public async Task<ServiceResponse> CustomUrl(string originalUrl, string customCode)
        {
            if (!originalUrl.isUrlValid())
            {
                return ServiceResponse.FailedResponse($"Invalid Url({originalUrl})");
            }

            if (customCode.Length > codeLength)
            {
                return ServiceResponse.FailedResponse($"Custom code length  cannot be greater than {codeLength}");
            }

            if (!customCode.isBase62())
            {
                return ServiceResponse.FailedResponse($"Custom code({customCode}) must be alpha-numeric");
            }

            string? cachedValue = await _distributedCache.GetStringAsync(customCode);

            if(cachedValue != null)
            {
                return ServiceResponse.FailedResponse($"Custom code({customCode}) already registered. Try different code");
            }

            await _distributedCache.SetStringAsync(customCode, originalUrl);

            return ServiceResponse.SuccessfulResponse(customCode);
        }

        public async Task<ServiceResponse> Shorter(string originalUrl)
        {
            if (!originalUrl.isUrlValid())
            {
                return ServiceResponse.FailedResponse($"Invalid Url({originalUrl})");
            }
            // Maybe can check this originalUrl already registered
            string shortenCode;
            string? cachedValue;

            do
            {
                //Perform once
                shortenCode = generateCode();
                cachedValue = await _distributedCache.GetStringAsync(shortenCode);

            } while (cachedValue is not null);

            await _distributedCache.SetStringAsync(shortenCode, originalUrl);
            return ServiceResponse.SuccessfulResponse(shortenCode);
        }

        public async Task<string?> GetOriginalUrl(string shortCode)
        {
            return await _distributedCache.GetStringAsync(shortCode);
        }

        private string generateCode()
        {
            StringBuilder code = new StringBuilder();

            for (int i = 0; i < codeLength; i++)
            {
                var randomIndex = _random.Next(_base62.Length);
                code.Append(_base62[randomIndex]);
            }

            return code.ToString();
        }
    }
}
