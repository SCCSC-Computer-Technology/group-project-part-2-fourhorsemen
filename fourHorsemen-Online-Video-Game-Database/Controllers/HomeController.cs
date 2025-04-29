using System.Data;
using System.Diagnostics;
using fourHorsemen_Online_Video_Game_Database.Models;
using fourHorsemen_Online_Video_Game_Database.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace fourHorsemen_Online_Video_Game_Database.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly GameNewsService _newsService;

        private readonly GameImportService _gameImportService;

        public HomeController(ILogger<HomeController> logger, GameNewsService newsService, GameImportService gameImportService)
        {
            _logger = logger;
            _newsService = newsService;
            _gameImportService = gameImportService;
        }

        public async Task<IActionResult> Index(int page = 1)
        {


            int itemsPerPage = 20; //number of news items per page
            var allNews = await _newsService.GetNewsAsync();
            var totalItems = allNews.Count;
            var totalPages = (int)Math.Ceiling(totalItems / (double)itemsPerPage);

            // Get the news items for the current page
            var newsForPage = allNews.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

            // Create a pagination object to pass to the view
            var pagination = new PaginationViewModel
            {
                CurrentPage = page,
                TotalPages = totalPages
            };

            // Pass the news items and pagination to the view
            ViewBag.Pagination = pagination;

            return View(newsForPage);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult History()
        {
            return View();
        }

        public IActionResult Facts()
        {
            var categorizedFacts = FunFactsService.GetCategorizedFacts();
            return View(categorizedFacts);
        }

        [Authorize(Roles = "Administrators")]
        public IActionResult Admin()
        {

            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
