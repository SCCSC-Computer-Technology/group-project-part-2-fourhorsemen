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

        [HttpPost]
        [Authorize(Roles = "Administrators")]
        public async Task<IActionResult> ImportGames()
        {
            // Define a list of platforms and their corresponding CSV file paths
            var platforms = new Dictionary<string, string>
        {
            { "sony_ps1", "sony_ps1_games.csv" },
            { "sony_ps2", "sony_ps2_games.csv" },
            { "sony_ps3", "sony_ps3_games.csv" },
            { "sony_ps4", "sony_ps4_games.csv" },
            { "sony_ps5", "sony_ps5_games.csv" },
            { "sony_vita", "sony_vita_games.csv" },
            { "sony_psp", "sony_psp_games.csv" },
            { "microsoft_xbox", "microsoft_xbox_games.csv" },
            { "microsoft_xbox_360", "microsoft_xbox_360_games.csv" },
            { "microsoft_xbox_one", "microsoft_xbox_one_games.csv" },
            { "microsoft_xbox_series_sx", "microsoft_xbox_series_sx_games.csv" },
            { "nintendo_switch", "nintendo_switch_games.csv" },
            { "nintendo_wii_u", "nintendo_wii_u_games.csv" },
            { "nintendo_wii", "nintendo_wii_games.csv" },
            { "nintendo_gamecube", "nintendo_gamecube_games.csv" },
            { "nintendo_64", "nintendo_64_games.csv" },
            { "nintendo_3ds", "nintendo_3ds_games.csv" },
            { "nintendo_ds", "nintendo_ds_games.csv" },
            { "nintendo_gameboy", "nintendo_gameboy_games.csv" },
            { "nintendo_gameboy_color", "nintendo_gameboy_color_games.csv" },
            { "nintendo_gameboy_advance", "nintendo_gameboy_advance_games.csv" },
            { "sega_master_system", "sega_master_system_games.csv" },
            { "sega_genesis", "sega_genesis_games.csv" },
            { "sega_32x", "sega_32x_games.csv" },
            { "sega_cd", "sega_cd_games.csv" },
            { "sega_saturn", "sega_saturn_games.csv" },
            { "sega_dreamcast", "sega_dreamcast_games.csv" },
            { "sega_gamegear", "sega_gamegear_games.csv" },
            { "atari_2600", "atari_2600_games.csv" },
            { "atari_5200", "atari_5200_games.csv" },
            { "atari_7800", "atari_7800_games.csv" },
            { "atari_jaguar", "atari_jaguar_games.csv" },
            { "atari_lynx", "atari_lynx_games.csv" },
            { "snk_neogeo", "snk_neogeo_games.csv" },
            { "panasonic_3do", "panasonic_3do_games.csv" }
        };

            // Loop through each platform and call the import method
            foreach (var platform in platforms)
            {
                string systemName = platform.Key;
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", platform.Value);

                await _gameImportService.ImportGamesFromCsvToDatabase(systemName, filePath);
            }


            return RedirectToAction("Admin"); // Redirect to Admin page after the import
        }
    }
}
