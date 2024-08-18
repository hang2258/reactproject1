using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Services;
using UrlShortener.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Data;

namespace UrlShortener.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UrlController : ControllerBase
    {
        private readonly UrlShortenerService _urlShortenerService;
        private readonly UrlDbContext _dbContext;

        public UrlController(UrlShortenerService urlShortenerService, UrlDbContext dbContext)
        {
            _urlShortenerService = urlShortenerService;
            _dbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> CreateShortUrl([FromBody] CreateUrlRequest request)
        {
            if (string.IsNullOrEmpty(request.OriginalUrl))
                return BadRequest("Original URL is required");

            try
            {
                var shortUrl = await _urlShortenerService.ShortenUrlAsync(request.OriginalUrl);
                return Ok(new { ShortUrl = shortUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{shortUrl}")]
        public async Task<IActionResult> GetOriginalUrl(string shortUrl)
        {
            var originalUrl = await _urlShortenerService.GetOriginalUrlAsync(shortUrl);
            if (originalUrl == null)
                return NotFound();

            return Redirect(originalUrl);
        }

        [HttpGet("GetAllUrls")]
        public async Task<IActionResult> GetAllUrls()
        {
            try
            {
                var urls = await _dbContext.Urls.ToListAsync();
                return Ok(urls);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("DropUrlTable")]
        public async Task<IActionResult> DropUrlTable()
        {
            try
            {
                // Using ExecuteSqlRaw instead of ExecuteSqlRawAsync
                await _dbContext.Database.ExecuteSqlRawAsync("DROP TABLE IF EXISTS [dbo].[Urls]");
                return Ok("Dropped successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}