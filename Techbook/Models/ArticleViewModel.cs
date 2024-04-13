namespace Techbook.Models;

public class ArticleViewModel
{
    public string Title { get; set; }
    public string? Description { get; set; }
    public string Link { get; set; }
    public DateTime PublishingDate { get; set; }
    public string? ImagePath { get; set; }
    public string? Content { get; set; }
    public int Rating { get; set; }
}