using CodeHollow.FeedReader;
using fourHorsemen_Online_Video_Game_Database.Models;
using System.Text.RegularExpressions;

public class GameNewsService
{
    private readonly List<string> _rssFeeds = new List<string>
    {
        "https://www.pcgamer.com/rss/",
        "https://www.gamespot.com/feeds/news/",
        "https://kotaku.com/rss"
    };

    public async Task<List<FeedItemWithImage>> GetNewsAsync()
    {
        var allItems = new List<FeedItemWithImage>();

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
                        allItems.Add(new FeedItemWithImage
                        {
                            Title = item.Title,
                            Link = item.Link,
                            PublishingDate = item.PublishingDate,
                            Description = item.Description,
                            ImageUrl = imageUrl
                        });
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
            .Take(15)
            .ToList();
    }

    private string? ExtractImageUrl(FeedItem item)
    {
        // Try parsing <img src="..."> from description
        var match = Regex.Match(item.Description ?? "", "<img.+?src=[\"'](.+?)[\"']", RegexOptions.IgnoreCase);
        if (match.Success)
        {
            return match.Groups[1].Value;
        }

        // Fallback: sometimes in SpecificItems
        if (item.SpecificItem?.Element != null)
        {
            var mediaContent = item.SpecificItem.Element.Descendants()
                .FirstOrDefault(e => e.Name.LocalName == "content" && e.Attributes().Any(a => a.Name.LocalName == "url"));
            return mediaContent?.Attribute("url")?.Value;
        }

        return null;
    }
}
