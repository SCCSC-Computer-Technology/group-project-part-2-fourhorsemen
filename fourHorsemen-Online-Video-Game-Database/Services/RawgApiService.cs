using fourHorsemen_Online_Video_Game_Database.Data;
using fourHorsemen_Online_Video_Game_Database.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;


namespace fourHorsemen_Online_Video_Game_Database.Services
{
    //this service handles all communication with the RAWG API
    public class RawgApiService
    {
        private readonly HttpClient _httpClient;
        private readonly GameDBContext _context;
        //RAWG API key
        private const string ApiKey = "7fe3653cefd54638a6b60936f83c508b";

        //base URL for the RAWG API
        private const string ApiUrl = "https://api.rawg.io/api";

        //constructor that sets up the HttpClient used for making requests
        public RawgApiService(HttpClient httpClient, GameDBContext context)
        {
            _httpClient = httpClient;
            _context = context;
        }


        //this method fetches game names from the API based on the provided platform ID and saves them to a CSV file
        public async Task SaveGameNamesToCsvAsync(string filePath, int platformId)
        {
            var allNames = new List<string>();
            int page = 1;
            bool hasMore = true;

            while (hasMore)
            {
                //build the request URL for the selected platform (platform ID passed in)
                string url = $"{ApiUrl}/games?platforms={platformId}&page={page}&page_size=40&key={ApiKey}";

                //send the request
                var response = await _httpClient.GetAsync(url);

                //stop if the request failed
                if (!response.IsSuccessStatusCode)
                    break;

                //read the response and parse it as JSON
                var content = await response.Content.ReadAsStringAsync();
                var json = JsonDocument.Parse(content);
                var results = json.RootElement.GetProperty("results");

                //extract the "name" from each game in the results
                foreach (var game in results.EnumerateArray())
                {
                    string name = game.GetProperty("name").GetString();
                    allNames.Add(name);
                }

                //check if there is a next page
                hasMore = json.RootElement.TryGetProperty("next", out var next)
                          && !string.IsNullOrEmpty(next.GetString());

                //move to the next page
                page++;
            }

            //save all collected names to a CSV file (one name per line)
            await File.WriteAllLinesAsync(filePath, allNames);

            //log to the console
            Console.WriteLine($"Saved {allNames.Count} game names to: {filePath}");
        }

        public async Task<JsonElement?> GetGameDetailsAsync(string slugOrTitle)
        {
            // First try slug directly
            string slugUrl = $"{ApiUrl}/games/{slugOrTitle}?key={ApiKey}";
            var slugResponse = await _httpClient.GetAsync(slugUrl);

            if (slugResponse.IsSuccessStatusCode)
            {
                var content = await slugResponse.Content.ReadAsStringAsync();
                var json = JsonDocument.Parse(content);
                var root = json.RootElement;

                // Handle redirect
                if (root.TryGetProperty("redirect", out var redirect) && redirect.GetBoolean()
                    && root.TryGetProperty("slug", out var newSlug))
                {
                    return await GetGameDetailsAsync(newSlug.GetString());
                }

                return root;
            }

            // If slug failed, try search
            string searchUrl = $"{ApiUrl}/games?search={Uri.EscapeDataString(slugOrTitle)}&key={ApiKey}";
            var searchResponse = await _httpClient.GetAsync(searchUrl);

            if (!searchResponse.IsSuccessStatusCode)
                return null;

            var searchContent = await searchResponse.Content.ReadAsStringAsync();
            var searchJson = JsonDocument.Parse(searchContent);
            var results = searchJson.RootElement.GetProperty("results");

            if (results.GetArrayLength() == 0)
                return null;

            var firstResult = results[0];
            var correctedSlug = firstResult.GetProperty("slug").GetString();

            // Now fetch full details using corrected slug
            return await GetGameDetailsAsync(correctedSlug);
        }

        public async Task SaveGameDetailsToDatabaseAsync(int platformId)
        {
            int page = 1;
            bool hasMore = true;

            while (hasMore)
            {
                string url = $"{ApiUrl}/games?platforms={platformId}&page={page}&page_size=40&key={ApiKey}";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    break;

                var content = await response.Content.ReadAsStringAsync();
                var json = JsonDocument.Parse(content);
                var results = json.RootElement.GetProperty("results");

                foreach (var game in results.EnumerateArray())
                {
                    string gameTitle = "Unknown";
                    if (game.TryGetProperty("name", out var nameProperty))
                    {
                        gameTitle = nameProperty.GetString();
                    }

                    string gameSlug = "Unknown";
                    if (game.TryGetProperty("slug", out var slugProperty))
                    {
                        gameSlug = slugProperty.GetString();
                    }

                    // Fetch full game details
                    var gameDetails = await GetGameDetailsAsync(gameSlug);
                    if (gameDetails != null)
                    {
                        var gameData = gameDetails.Value;

                        string developer = "Unknown";
                        var developersArray = gameData.GetProperty("developers");
                        if (developersArray.GetArrayLength() > 0)
                        {
                            var firstDeveloper = developersArray[0];
                            developer = firstDeveloper.GetProperty("name").GetString();
                        }

                        string publisher = "Unknown";
                        var publishersArray = gameData.GetProperty("publishers");
                        if (publishersArray.GetArrayLength() > 0)
                        {
                            var firstPublisher = publishersArray[0];
                            publisher = firstPublisher.GetProperty("name").GetString();
                        }

                        var gameEntity = new Game
                        {
                            Title = gameTitle,
                            Slug = gameSlug,
                            Developer = developer,
                            Publisher = publisher,
                            ReleaseDate = DateTime.Parse(gameData.GetProperty("released").GetString() ?? DateTime.MinValue.ToString()),
                            NumberOfPlayers = 1, // Placeholder, you can parse from the API if available
                            Sales = 0 // Placeholder
                        };

                        // Save to the database
                        _context.Games.Add(gameEntity);
                        await _context.SaveChangesAsync();
                    }
                }

                hasMore = json.RootElement.TryGetProperty("next", out var next)
                          && !string.IsNullOrEmpty(next.GetString());
                page++;
            }

            Console.WriteLine("Game details saved to database.");
        }

    }
}

