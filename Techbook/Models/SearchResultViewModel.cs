namespace Techbook.Models;

public class SearchResultViewModel
{
    public int ResultsFound { get; set; }
    public required List<ArticleViewModel> Articles { get; set; }
}