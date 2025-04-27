using CodeHollow.FeedReader;
using fourHorsemen_Online_Video_Game_Database.Models;
using System.Text.RegularExpressions;

public class GameNewsService
{
    private readonly List<string> _rssFeeds = new List<string>
    {
        "https://www.pcgamer.com/rss/",
        "https://www.gamespot.com/feeds/news/",
        "https://www.nintendolife.com/feeds/latest",
        "https://www.ign.com/rss",
        "https://www.purexbox.com/feeds/latest",
        "https://www.gameinformer.com/rss"
    };

    public async Task<List<FeedItemWithImage>> GetNewsAsync()
    {
        var allItems = new List<FeedItemWithImage>();
        var seenTitlesAndImages = new HashSet<(string Title, string ImageUrl)>(); // Track seen combinations

        foreach (var url in _rssFeeds)
        {
            try
            {
                using var httpClient = new HttpClient();
                var response = await httpClient.GetAsync(url);
                var contentType = response.Content.Headers.ContentType?.MediaType;

                if (contentType == "application/rss+xml" ||
                    contentType == "text/xml" ||
                    contentType == "application/xml")
                {
                    var content = await response.Content.ReadAsByteArrayAsync();
                    var feed = FeedReader.ReadFromByteArray(content);

                    foreach (var item in feed.Items)
                    {
                        var imageUrl = ExtractImageUrl(item);
                        var title = item.Title;

                        if (!string.IsNullOrEmpty(imageUrl) && !seenTitlesAndImages.Contains((title, imageUrl)))
                        {
                            allItems.Add(new FeedItemWithImage
                            {
                                Title = title,
                                Link = item.Link,
                                PublishingDate = item.PublishingDate,
                                Description = item.Description,
                                ImageUrl = imageUrl
                            });

                            // Track the combination of Title and Image URL to avoid duplicates
                            seenTitlesAndImages.Add((title, imageUrl));
                        }
                    }
                }
            }
            catch
            {
                // silent fail per feed
            }
        }

        return allItems
            .OrderByDescending(i => i.PublishingDate ?? DateTime.MinValue)
            .Take(100)  //displays 100 news items, 20 per page
            .ToList();
    }

    private string? ExtractImageUrl(FeedItem item)
    {
        // First, try parsing <img src="..."> from the description using regex
        var match = Regex.Match(item.Description ?? "", "<img.+?src=[\"'](.+?)[\"']", RegexOptions.IgnoreCase);
        if (match.Success)
        {
            return match.Groups[1].Value;
        }

        // Second, check if the item has media content in the 'specificItem' element
        if (item.SpecificItem?.Element != null)
        {
            var mediaContent = item.SpecificItem.Element.Descendants()
                .FirstOrDefault(e => e.Name.LocalName == "content" && e.Attributes().Any(a => a.Name.LocalName == "url"));
            if (mediaContent != null)
            {
                return mediaContent?.Attribute("url")?.Value;
            }
        }

        // Return a default image URL if no image found
        return "https://example.com/default-image.jpg"; // Replace with an actual default image
    }
}
