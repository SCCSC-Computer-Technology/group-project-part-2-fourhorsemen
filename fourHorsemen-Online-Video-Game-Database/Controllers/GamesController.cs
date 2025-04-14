using fourHorsemen_Online_Video_Game_Database.Services;     //for rawg api service
using Microsoft.AspNetCore.Mvc;                             //for mvc controller functionality
using Microsoft.EntityFrameworkCore;                        //for ef core functionality
using fourHorsemen_Online_Video_Game_Database.Data;         //for accessing the GameDBContext
using fourHorsemen_Online_Video_Game_Database.Models;       //for accessing the Game model


namespace fourHorsemen_Online_Video_Game_Database.Controllers
{
    //controller to display the game data from rawg and the local csv files
    public class GamesController : Controller
    {
        //service to fetch game data from rawg api
        private readonly RawgApiService _apiService;

        //cosnstructor that injects the rawg api service
        public GamesController(RawgApiService apiService)
        {
            _apiService = apiService;
        }


        // ================================
        // CSV GENERATION ACTIONS (ADMIN)
        // ================================

        //creates a local csv of NES games using the rawg api
        //[Authorize(Roles = "Admin")] --> NEED TO MAKE ADMIN ROLES
        public async Task<IActionResult> GenerateNesCsv()
        {
            //define file path to save NES games CSV
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "nes_games.csv");

            //save NES game names to CSV file (using platform ID for NES)
            await _apiService.SaveGameNamesToCsvAsync(filePath, platformId: 49);

            //return success message
            return Content($"NES game names saved successfully to: {filePath}");
        }

        //creates a local csv of SNES games using the rawg api
        //[Authorize(Roles = "Admin")] --> NEED TO MAKE ADMIN ROLES
        public async Task<IActionResult> GenerateSnesCsv()
        {
            //define file path to save SNES games CSV
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "snes_games.csv");

            //save SNES game names to CSV file (using platform ID for SNES)
            await _apiService.SaveGameNamesToCsvAsync(filePath, platformId: 79);

            //return success message
            return Content($"SNES game names saved successfully to: {filePath}");
        }

        //creates a local csv of Sega Master System games using the rawg api
        //[Authorize(Roles = "Admin")] --> NEED TO MAKE ADMIN ROLES
        public async Task<IActionResult> GenerateSegaMasterSystemCsv()
        {
            //define file path to save Sega Master System games CSV
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "sega_master_system_games.csv");

            //save Sega Master System game names to CSV file (using platform ID for Sega Master System)
            await _apiService.SaveGameNamesToCsvAsync(filePath, platformId: 74);

            //return success message
            return Content($"Sega Master System game names saved successfully to: {filePath}");
        }

        //creates a local csv of Sega Genesis games using the rawg api
        //[Authorize(Roles = "Admin")] --> NEED TO MAKE ADMIN ROLES
        public async Task<IActionResult> GenerateSegaGenesisCsv()
        {
            //define file path to save Sega Genesis games CSV
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "sega_genesis_games.csv");

            //save Sega Genesis game names to CSV file (using platform ID for Sega Genesis)
            await _apiService.SaveGameNamesToCsvAsync(filePath, platformId: 167);

            //return success message
            return Content($"Sega Genesis game names saved successfully to: {filePath}");
        }


        // ================================
        // GAME LIST VIEWS
        // ================================

        //dictionary to match each game system to its local csv file
        private static readonly Dictionary<string, string> _csvFiles = new()
        {
            ["nes"] = "nes_games.csv",
            ["snes"] = "snes_games.csv",
            ["sega_master_system"] = "sega_master_system_games.csv",
            ["sega_genesis"] = "sega_genesis_games.csv"
        };

        //shows a list of games for the selected system
        public IActionResult SystemGamesList(string system)
        {
            //return 400 if no system was provided
            if (string.IsNullOrEmpty(system))
                return BadRequest("System name is required.");

            //try to find the matching csv file name based on the system
            if (!_csvFiles.TryGetValue(system.ToLower(), out var filename))
                return BadRequest("Unsupported system specified.");

            //load the game titles from the corresponding csv file
            var games = LoadGamesFromCsv(filename);

            //return 404 if no games were loaded (file may be missing or empty)

            if (games.Count == 0)
            {
                return NotFound("No games found or CSV file missing.");
            }

            //format the system name for custom titles
            string formattedTitle = system.ToLower() switch
            {
                "nes" => "Nintendo (NES) - Famicom",
                "snes" => "Super Nintendo (SNES) - Super Famicom",
                "sega_master_system" => "Sega Master System - Sega Mark III",
                "sega_genesis" => "Sega Genesis - Mega Drive",
                //default case for any system that doesn't match the above
                _ => system.Replace('_', ' ') //replace underscores with spaces
                    .ToLower() //convert the system name to lowercase
                    .Split(' ') //split the name into words based on spaces
                    .Select(word => char.ToUpper(word[0]) + word.Substring(1)) //capitalize the first letter of each word
                    .Aggregate((current, next) => current + " " + next) //join the words back together with spaces, capitalized
            };


            // Set the dynamic title
            ViewBag.SystemTitle = $"{formattedTitle} Games";

            //return the list of games to the view
            return View(games);
        }

        //loads a CSV file from wwwroot and returns a list of Game objects
        private List<Game> LoadGamesFromCsv(string filename)
        {
            //get the full path to the csv file
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filename);

            //if the file doesn't exist, return an empty list
            if (!System.IO.File.Exists(filePath)) return new List<Game>();

            //read the lines from the file, clean them up, and convert them to a list of Game objects
            return System.IO.File.ReadAllLines(filePath)
                                 .Where(line => !string.IsNullOrWhiteSpace(line))
                                 .Select(name => new Game { Title = name.Trim() })
                                 .ToList();
        }


        // ================================
        // ADDITIONAL GAME DETAILS ACTIONS
        // ================================

        //// Displays detailed game info for one game by title
        //public async Task<IActionResult> Details(string title)
        //{
        //    if (string.IsNullOrEmpty(title))
        //    {
        //        return NotFound(); // Return 404 if no title was passed
        //    }

        //    // Use the API to get game details based on the title
        //    var gameDetailsJson = await _apiService.GetGameDetailsAsync(title);

        //    // Pass the JSON data to the view using ViewBag for now
        //    ViewBag.GameJson = gameDetailsJson;

        //    return View();
        //}
    }
}
