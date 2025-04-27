using fourHorsemen_Online_Video_Game_Database.Data;
using fourHorsemen_Online_Video_Game_Database.Models;
using fourHorsemen_Online_Video_Game_Database.Services;
using Microsoft.EntityFrameworkCore;

public class GameImportService
{
    private readonly GameDBContext _context;
    private readonly RawgApiService _apiService;

    public GameImportService(GameDBContext context, RawgApiService apiService)
    {
        _context = context;
        _apiService = apiService;
    }

    public async Task ImportGamesFromCsvToDatabase(string systemName, string filePath)
    {
        // Read all game titles from the CSV file
        var gameTitles = System.IO.File.ReadAllLines(filePath)
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .ToList();

        foreach (var title in gameTitles)
        {
            // Check if the game already exists in the database
            var existingGame = await _context.Games.FirstOrDefaultAsync(g => g.Title == title);

            if (existingGame == null) // If the game doesn't exist, fetch additional data
            {
                // Fetch game details from the RAWG API using the title
                var gameDetails = await _apiService.GetGameDetailsAsync(title);

                if (gameDetails != null)
                {
                    string developer = "Unknown";
                    var developersArray = gameDetails.Value.GetProperty("developers");
                    if (developersArray.GetArrayLength() > 0)
                    {
                        developer = developersArray[0].GetProperty("name").GetString();
                    }

                    string publisher = "Unknown";
                    var publishersArray = gameDetails.Value.GetProperty("publishers");
                    if (publishersArray.GetArrayLength() > 0)
                    {
                        publisher = publishersArray[0].GetProperty("name").GetString();
                    }

                    var newGame = new Game
                    {
                        Title = title,
                        Slug = gameDetails.Value.GetProperty("slug").GetString(),
                        Developer = developer,
                        Publisher = publisher,
                        ReleaseDate = DateTime.TryParse(gameDetails.Value.GetProperty("released").GetString(), out var releaseDate) ? releaseDate : DateTime.MinValue,
                        NumberOfPlayers = 1,  
                        Sales = 0            
                    };

                    _context.Games.Add(newGame);
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}
