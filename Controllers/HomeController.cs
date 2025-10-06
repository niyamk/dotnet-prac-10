using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory; // Required for IMemoryCache

namespace CachingDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMemoryCache _memoryCache;

        // Inject IMemoryCache via the constructor
        public HomeController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public IActionResult Index()
        {
            return View();
        }

        //== Demo 1: Response Caching ==//
        // The [ResponseCache] attribute tells the middleware to cache this response.
        // Duration: How long to cache the response (in seconds).
        // Location: Where the cache can be stored (Any = browser, proxy, server).
        [ResponseCache(Duration = 10, Location = ResponseCacheLocation.Any, NoStore = false)]
        public IActionResult ResponseCacheDemo()
        {
            // We pass the current time to the view to see when the page was generated.
            ViewBag.Timestamp = DateTime.Now.ToString("HH:mm:ss");
            return View();
        }


        //== Demo 2: Data Caching with IMemoryCache ==//
        public IActionResult MemoryCacheDemo()
        {
            string cacheKey = "MyCachedTimestamp";

            // Try to get the timestamp from the cache.
            // The "out" variable will be populated if the key is found.
            if (!_memoryCache.TryGetValue(cacheKey, out string? cachedTimestamp))
            {
                // If the key is NOT in the cache, generate the data.
                cachedTimestamp = DateTime.Now.ToString("HH:mm:ss");

                // Configure cache options.
                var cacheOptions = new MemoryCacheEntryOptions()
                    // Set an absolute expiration time of 20 seconds from now.
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(20));

                // Store the data in the cache with the specified key and options.
                _memoryCache.Set(cacheKey, cachedTimestamp, cacheOptions);
            }

            // Pass both the cached time and the current server time to the view.
            ViewBag.CachedTimestamp = cachedTimestamp;
            ViewBag.CurrentTimestamp = DateTime.Now.ToString("HH:mm:ss");

            return View();
        }
    }
}