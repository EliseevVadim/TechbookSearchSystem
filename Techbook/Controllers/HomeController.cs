using Microsoft.AspNetCore.Mvc;
using Techbook.Models;
using Techbook.Services;

namespace Techbook.Controllers;

public class HomeController : Controller
{
    private readonly IWebHostEnvironment _webHostEnvironment;

    public HomeController(IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Search([FromQuery]string query)
    {
        RssSearcher searcher = new RssSearcher($"{_webHostEnvironment.WebRootPath}/data/links.txt");
        SearchResultViewModel result = searcher.SearchArticles(query);
        return View("Search", result);
    }
}