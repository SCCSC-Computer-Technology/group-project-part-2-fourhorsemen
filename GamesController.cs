using fourHorsemen_Online_Video_Game_Database.Services;     //for rawg api service
using Microsoft.AspNetCore.Mvc;                             //for mvc controller functionality
using Microsoft.EntityFrameworkCore;                        //for ef core functionality
using fourHorsemen_Online_Video_Game_Database.Data;         //for accessing the GameDBContext
using fourHorsemen_Online_Video_Game_Database.Models;       //for accessing the Game model
using System.Diagnostics;
using System.Text.Json;


namespace Microsoft.AspNetCore.Authorization
{

}

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


        // ================================
        // GAME LIST VIEWS
        // ================================

        //dictionary to match each game system to its local csv file
        private static readonly Dictionary<string, string> _csvFiles = new()
        {
            ["sony_ps5"] = "sony_ps5_games.csv",
            ["sony_ps4"] = "sony_ps4_games.csv",
            ["sony_ps3"] = "sony_ps3_games.csv",
            ["sony_ps2"] = "sony_ps2_games.csv",
            ["sony_ps1"] = "sony_ps1_games.csv",
            ["sony_vita"] = "sony_vita_games.csv",
            ["sony_psp"] = "sony_psp_games.csv",
            ["ms_xbox_sx"] = "microsoft_xbox_series_sx_games.csv",
            ["ms_xbox_one"] = "microsoft_xbox_one_games.csv",
            ["ms_xbox_360"] = "microsoft_xbox_360_games.csv",
            ["ms_xbox"] = "microsoft_xbox_games.csv",
            ["nintendo_switch"] = "nintendo_switch_games.csv",
            ["nintendo_wii_u"] = "nintendo_wii_u_games.csv",
            ["nintendo_wii"] = "nintendo_wii_games.csv",
            ["nintendo_gamecube"] = "nintendo_gamecube_games.csv",
            ["nintendo_64"] = "nintendo_64_games.csv",
            ["snes"] = "snes_games.csv",
            ["nes"] = "nes_games.csv",
            ["nintendo_3ds"] = "nintendo_3ds_games.csv",
            ["nintendo_ds"] = "nintendo_ds_games.csv",
            ["nintendo_gba"] = "nintendo_gameboy_advance_games.csv",
            ["nintendo_gbc"] = "nintendo_gameboy_color_games.csv",
            ["nintendo_gb"] = "nintendo_gameboy_games.csv",
            ["sega_dreamcast"] = "sega_dreamcast_games.csv",
            ["sega_saturn"] = "sega_saturn_games.csv",
            ["sega_32x"] = "sega_32x_games.csv",
            ["sega_cd"] = "sega_cd_games.csv",
            ["sega_genesis"] = "sega_genesis_games.csv",
            ["sega_master_system"] = "sega_master_system_games.csv",
            ["sega_gamegear"] = "sega_gamegear_games.csv",
            ["atari_jaguar"] = "atari_jaguar_games.csv",
            ["atari_7800"] = "atari_7800_games.csv",
            ["atari_5200"] = "atari_5200_games.csv",
            ["atari_2600"] = "atari_2600_games.csv",
            ["atari_lynx"] = "atari_lynx_games.csv",
            ["panasonic_3do"] = "panasonic_3do_games.csv",
            ["snk_neogeo"] = "snk_neogeo_games.csv"
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
                "sony_ps5" => "Sony PlayStation 5",
                "sony_ps4" => "Sony PlayStation 4",
                "sony_ps3" => "Sony PlayStation 3",
                "sony_ps2" => "Sony PlayStation 2",
                "sony_ps1" => "Sony PlayStation",
                "sony_psp" => "Sony Playstation Portable (PSP)",
                "ms_xbox_sx" => "Microsoft XBox Series S/X",
                "ms_xbox_one" => "Microsoft XBox One",
                "ms_xbox_360" => "Microsoft XBox 360",
                "ms_xbox" => "Microsoft XBox",
                "nintendo_gamecube" => "Nintendo GameCube",
                "snes" => "Super Nintendo (SNES) / Super Famicom",
                "nes" => "Nintendo (NES) / Famicom",
                "nintendo_3ds" => "Nintendo 3DS",
                "nintendo_ds" => "Nintendo DS",
                "nintendo_gba" => "Nintendo GameBoy Advance",
                "nintendo_gbc" => "Nintendo GameBoy Color",
                "nintendo_gb" => "Nintendo GameBoy",
                "sega_32x" => "Sega 32X",
                "sega_cd" => "Sega CD",
                "sega_genesis" => "Sega Genesis / Mega Drive",
                "sega_master_system" => "Sega Master System / Sega Mark III",
                "sega_gamegear" => "Sega GameGear",
                "panasonic_3do" => "Panasonic 3D0",
                "snk_neogeo" => "SNK Neo Geo",
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

            var game = gameData.Value;

            var viewModel = new GameDetailsViewModel
            {
                Title = game.TryGetProperty("name", out var name) ? name.GetString() : "Unknown",
                Developer = game.TryGetProperty("developers", out var devs) && devs.GetArrayLength() > 0
        ? devs[0].GetProperty("name").GetString()
        : "Unknown",
                Publisher = game.TryGetProperty("publishers", out var pubs) && pubs.GetArrayLength() > 0
        ? pubs[0].GetProperty("name").GetString()
        : "Unknown",
                Platform = game.TryGetProperty("platforms", out var platforms) && platforms.ValueKind == JsonValueKind.Array
        ? string.Join(", ", platforms.EnumerateArray()
            .Select(p => p.TryGetProperty("platform", out var plat) && plat.TryGetProperty("name", out var platName)
                ? platName.GetString()
                : "Unknown"))
        : "Unknown",
                ReleaseDate = game.TryGetProperty("released", out var released)
        ? released.GetString() ?? "N/A"
        : "N/A",
                Players = "1+",
                Sales = "N/A",
                CoverImageUrl = game.TryGetProperty("background_image", out var img)
        ? img.GetString() ?? "/images/placeholder.png"
        : "/images/placeholder.png",
                Description = game.TryGetProperty("description_raw", out var desc)
        ? desc.GetString() ?? "No description available."
        : "No description available."
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
