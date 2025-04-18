using System.Text.Json;


namespace fourHorsemen_Online_Video_Game_Database.Services
{
    //this service handles all communication with the RAWG API
    public class RawgApiService
    {
        private readonly HttpClient _httpClient;

        //RAWG API key
        private const string ApiKey = "7fe3653cefd54638a6b60936f83c508b";

        //base URL for the RAWG API
        private const string ApiUrl = "https://api.rawg.io/api";

        //constructor that sets up the HttpClient used for making requests
        public RawgApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
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

    }
}

