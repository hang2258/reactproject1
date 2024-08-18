using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Data;
using UrlShortener.Models;

namespace UrlShortener.Services
{
    public class UrlShortenerService
    {
        private readonly UrlDbContext _context;
        private readonly IDistributedCache _cache;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UrlShortenerService(UrlDbContext context, IDistributedCache cache, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _cache = cache;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> ShortenUrlAsync(string originalUrl)
        {
            if (!Uri.IsWellFormedUriString(originalUrl, UriKind.Absolute))
                throw new ArgumentException("Invalid URL");

            var shortCode = GenerateShortCode();

            var scheme = _httpContextAccessor.HttpContext.Request.Scheme;
            var host = _httpContextAccessor.HttpContext.Request.Host;
            var shortUrl = $"{scheme}://{host}/api/Url/{shortCode}";

            var urlEntity = new UrlEntity
            {
                OriginalUrl = originalUrl,
                ShortUrl = shortUrl,
                CreatedAt = DateTime.UtcNow
            };

            _context.Urls.Add(urlEntity);
            await _context.SaveChangesAsync();

            return shortUrl;
        }

        private string GenerateShortCode()
        {
            var guid = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 6);
            return ConvertToBase62(guid);
        }

        private string ConvertToBase62(string input)
        {
            const string alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var result = new StringBuilder();
            var number = new BigInteger(Encoding.UTF8.GetBytes(input));

            while (number > 0)
            {
                result.Insert(0, alphabet[(int)(number % 62)]);
                number /= 62;
            }

            return result.ToString();
        }

        public async Task<string> GetOriginalUrlAsync(string shortCode)
        {
            var cachedUrl = await _cache.GetStringAsync(shortCode);
            if (cachedUrl != null)
                return cachedUrl;

            var urlEntity = await _context.Urls.SingleOrDefaultAsync(u => u.ShortUrl.EndsWith(shortCode));
            if (urlEntity != null)
            {
                await _cache.SetStringAsync(shortCode, urlEntity.OriginalUrl, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                });
            }

            return urlEntity?.OriginalUrl;
        }

        public async Task TestRedisConnectionAsync()
        {
            var testKey = "testKey";
            var testValue = "testValue";
            await _cache.SetStringAsync(testKey, testValue);
            var value = await _cache.GetStringAsync(testKey);
            if (value != testValue)
            {
                throw new Exception("Redis connection test failed");
            }
        }
    }
}
