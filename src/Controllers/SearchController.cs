using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;
using System.Text;
using YandexSearchApp.Models;
using YandexSearchApp.Services;

namespace YandexSearchApp.Controllers
{
    public class SearchController : Controller
    {
        private readonly SearchService _searchService;

        public SearchController(SearchService searchService) =>
            _searchService = searchService;


        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(SearchRequest request)
        {
            var stopwatch = Stopwatch.StartNew();
            var results = await _searchService.PerformSearch(request);
            stopwatch.Stop();
            ViewBag.TotalTimeTaken = stopwatch.Elapsed.TotalSeconds;

            return View(results);
        }

        [HttpPost]
        public IActionResult ExportToJson([FromBody]List<SearchResult> results)
        {
            if (results == null || !results.Any())
            {
                return BadRequest("No results provided.");
            }

            var json = JsonSerializer.Serialize(results, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            var bytes = Encoding.UTF8.GetBytes(json);

            return File(bytes, "application/json", "results.json");
        }
    }
}
