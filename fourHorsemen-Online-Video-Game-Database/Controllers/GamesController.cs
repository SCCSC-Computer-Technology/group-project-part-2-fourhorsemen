using fourHorsemen_Online_Video_Game_Database.Services;     //for rawg api service
using Microsoft.AspNetCore.Mvc;                             //for mvc controller functionality
using Microsoft.EntityFrameworkCore;                        //for ef core functionality
using fourHorsemen_Online_Video_Game_Database.Data;         //for accessing the GameDBContext
using fourHorsemen_Online_Video_Game_Database.Models;       //for accessing the Game model
using System.Diagnostics;


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
        public async Task<IActionResult> GenerateCsv(int platformId, string systemName)
        {
            //build file path dynamically based on system name
            string fileName = $"{systemName}_games.csv";
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", fileName);

            //count how many games are already in the csv if file exists
            int initialCount = 0;
            if (System.IO.File.Exists(filePath))
            {
                initialCount = CountGamesInCsv(filePath);
            }

            //save new game names from the aoi to csv file
            await _apiService.SaveGameNamesToCsvAsync(filePath, platformId);

            //sort csv file alphabetically
            SortCsvAlphabetically(filePath);

            //count how many games are in csv file after update
            int finalCount = CountGamesInCsv(filePath);

            //calculate how many new games were added
            int gamesAdded = finalCount - initialCount;

            //prepare message to show how many games were added
            string message = gamesAdded > 0
                             ? $"{gamesAdded} more games added to the CSV for {systemName}."
                             : "No new games added.";

            //return success message with final number of games
            return Content($"{systemName} game names saved and sorted successfully to: {filePath}. {message}");
        }

        //helper method to count number of games in csv file
        private int CountGamesInCsv(string filePath)
        {
            if (!System.IO.File.Exists(filePath)) return 0; //if the file doesn't exist, return 0

            //read all lines in the csv file and ignore empty lines
            var lines = System.IO.File.ReadAllLines(filePath)
                                      .Where(line => !string.IsNullOrWhiteSpace(line)) //ignore empty lines
                                      .ToArray();

            //return number of lines (number of games)
            return lines.Length;
        }



        //helper method to sort csv file alphabetically by game titles
        private void SortCsvAlphabetically(string filePath)
        {
            if (!System.IO.File.Exists(filePath)) return;

            var lines = System.IO.File.ReadAllLines(filePath)
                                      .Where(line => !string.IsNullOrWhiteSpace(line)) //ignore empty lines
                                      .OrderBy(line => line.Trim()) //sort alphabetically
                                      .ToArray();

            System.IO.File.WriteAllLines(filePath, lines); //write sorted lines back to the file
        }


        //public async Task<IActionResult> GenerateNesCsv()
        //{
        //    //define file path to save NES games CSV
        //    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "nes_games.csv");

        //    //save NES game names to CSV file (using platform ID for NES)
        //    await _apiService.SaveGameNamesToCsvAsync(filePath, platformId: 49);

        //    //return success message
        //    return Content($"NES game names saved successfully to: {filePath}");
        //}

        ////creates a local csv of SNES games using the rawg api
        ////[Authorize(Roles = "Admin")] --> NEED TO MAKE ADMIN ROLES
        //public async Task<IActionResult> GenerateSnesCsv()
        //{
        //    //define file path to save SNES games CSV
        //    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "snes_games.csv");

        //    //save SNES game names to CSV file (using platform ID for SNES)
        //    await _apiService.SaveGameNamesToCsvAsync(filePath, platformId: 79);

        //    //return success message
        //    return Content($"SNES game names saved successfully to: {filePath}");
        //}

        ////creates a local csv of Sega Master System games using the rawg api
        ////[Authorize(Roles = "Admin")] --> NEED TO MAKE ADMIN ROLES
        //public async Task<IActionResult> GenerateSegaMasterSystemCsv()
        //{
        //    //define file path to save Sega Master System games CSV
        //    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "sega_master_system_games.csv");

        //    //save Sega Master System game names to CSV file (using platform ID for Sega Master System)
        //    await _apiService.SaveGameNamesToCsvAsync(filePath, platformId: 74);

        //    //return success message
        //    return Content($"Sega Master System game names saved successfully to: {filePath}");
        //}

        ////creates a local csv of Sega Genesis games using the rawg api
        ////[Authorize(Roles = "Admin")] --> NEED TO MAKE ADMIN ROLES
        //public async Task<IActionResult> GenerateSegaGenesisCsv()
        //{
        //    //define file path to save Sega Genesis games CSV
        //    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "sega_genesis_games.csv");

        //    //save Sega Genesis game names to CSV file (using platform ID for Sega Genesis)
        //    await _apiService.SaveGameNamesToCsvAsync(filePath, platformId: 167);

        //    //return success message
        //    return Content($"Sega Genesis game names saved successfully to: {filePath}");
        //}


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
        public IActionResult SystemGamesList(string system, string searchString, string startsWith)
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

            // Filter the list based on search string (case-insensitive)
            if (!string.IsNullOrEmpty(searchString))
            {
                games = games
                    .Where(g => g.Title.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Filter by first letter
            if (!string.IsNullOrEmpty(startsWith))
            {
                games = games
                    .Where(g => g.Title.StartsWith(startsWith, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            //format the system name for custom titles
            string formattedTitle = system.ToLower() switch
            {
                "nes" => "Nintendo (NES) / Famicom",
                "snes" => "Super Nintendo (SNES) / Super Famicom",
                "sega_master_system" => "Sega Master System / Sega Mark III",
                "sega_genesis" => "Sega Genesis / Mega Drive",
                //default case for any system that doesn't match the above
                _ => system.Replace('_', ' ') //replace underscores with spaces
                    .ToLower() //convert the system name to lowercase
                    .Split(' ') //split the name into words based on spaces
                    .Select(word => char.ToUpper(word[0]) + word.Substring(1)) //capitalize the first letter of each word
                    .Aggregate((current, next) => current + " " + next) //join the words back together with spaces, capitalized
            };


            // Set the dynamic title
            ViewBag.SystemTitle = $"{formattedTitle} Game List";
            ViewBag.System = system;               // keep track of which system we're on
            ViewBag.SearchString = searchString;   // for maintaining state in view
            ViewBag.StartsWith = startsWith;

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

        [HttpGet("/Games/GameDetails/{slug}")]
        public async Task<IActionResult> GameDetails(string slug)
        {
            var gameData = await _apiService.GetGameDetailsAsync(slug);

            if (gameData == null)
                return NotFound();

            var viewModel = new GameDetailsViewModel
            {
                Title = gameData.Value.GetProperty("name").GetString(),
                Developer = gameData.Value.TryGetProperty("developers", out var devs) && devs.GetArrayLength() > 0
                    ? devs[0].GetProperty("name").GetString()
                    : "Unknown",
                Publisher = gameData.Value.TryGetProperty("publishers", out var pubs) && pubs.GetArrayLength() > 0
                    ? pubs[0].GetProperty("name").GetString()
                    : "Unknown",
                Platform = string.Join(", ", gameData.Value.GetProperty("platforms").EnumerateArray()
                    .Select(p => p.GetProperty("platform").GetProperty("name").GetString())),
                ReleaseDate = gameData.Value.GetProperty("released").GetString() ?? "N/A",
                Players = "1+",
                Sales = "N/A",
                CoverImageUrl = gameData.Value.TryGetProperty("background_image", out var img)
                                ? img.GetString()
                                : "/images/placeholder.png" // fallback image
            };

            return View(viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
