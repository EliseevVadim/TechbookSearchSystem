using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using System.Xml;
using StopWord;
using Techbook.Models;

namespace Techbook.Services;

public class RssSearcher(string contentPath)
{
    private readonly string[] _links = File.ReadAllLines(contentPath);
    private readonly Dictionary<string, int> _sectionsCoefficients = new()
    {
        {"Title", 5},
        {"Description", 2},
        {"Content", 1},
    };

    public SearchResultViewModel SearchArticles(string query)
    {
        List<ArticleViewModel> allArticles = LoadAllArticles(query);
        List<ArticleViewModel> matchingArticles = allArticles
            .Where(article => article.Rating > 0)
            .OrderByDescending(article => article.Rating)
            .ThenByDescending(article => article.PublishingDate)
            .ToList();
        return new SearchResultViewModel
        {
            Articles = matchingArticles,
            ResultsFound = matchingArticles.Count
        };
    }

    private List<ArticleViewModel> LoadAllArticles(string query)
    {
        query = CleanQuery(query);
        List<ArticleViewModel> articles = new List<ArticleViewModel>();
        foreach (string link in _links)
        {
            XmlReader xmlReader = XmlReader.Create(link);
            SyndicationFeed feed = SyndicationFeed.Load(xmlReader);
            foreach (SyndicationItem item in feed.Items)
            {
                articles.Add(new ArticleViewModel
                {
                    Title = item.Title.Text,
                    Description = item.Summary?.Text,
                    Link = item.Links[0].Uri.AbsoluteUri,
                    Content = item.Content?.ToString(),
                    PublishingDate = item.PublishDate.UtcDateTime,
                    ImagePath = GetImagePath(item),
                    Rating = CalculateRating(item, query)
                });
            }
            xmlReader.Close();
        }
        return articles;
    }

    private string GetImagePath(SyndicationItem item)
    {
        try
        {
            return item.Links
                .First(link => link.RelationshipType == "enclosure")
                .Uri
                .AbsoluteUri;
        }
        catch
        {
            return string.Empty;
        }
    }

    private string CleanQuery(string input)
    {
        string result = Regex.Replace(input, @"[^\w\s]", "");
        result = Regex.Replace(result, @"\s+", " ");
        return result.RemoveStopWords("ru")
            .RemoveStopWords("en");
    }

    private int CalculateRating(SyndicationItem item, string query)
    {
        string[] tags = query.Split(" ", StringSplitOptions.TrimEntries);
        string title = item.Title.Text;
        string? description = item.Summary?.Text;
        string? content = item.Content?.ToString();
        int rating = 0;
        foreach (string tag in tags)
        {
            rating += CountOccurrences(title, tag) * _sectionsCoefficients["Title"];
            rating += CountOccurrences(description, tag) * _sectionsCoefficients["Description"];
            rating += CountOccurrences(content, tag) * _sectionsCoefficients["Content"];
        }
        return rating;
    }
    
    private int CountOccurrences(string? text, string substring)
    {
        if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(substring))
            return 0;

        text = text.ToLower();
        substring = substring.ToLower();
        
        int count = 0;
        int index = 0;
        
        while ((index = text.IndexOf(substring, index, StringComparison.OrdinalIgnoreCase)) != -1)
        {
            index += substring.Length;
            count++;
        }
        return count;
    }
}