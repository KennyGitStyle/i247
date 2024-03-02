using BusinessLogic.Service;
using Microsoft.AspNetCore.Mvc;


namespace Presentation.Controllers;

public class HomeController : Controller
{
    private readonly IRandomUserService _randomUserService;

    public HomeController(IRandomUserService randomUserService)
    {
        _randomUserService = randomUserService;
    }

     public async Task<IActionResult> Index()
    {
        var users = await _randomUserService.GetRandomUsersAsync();
        return View(users);
    }
    

    public IActionResult Privacy()
    {
        return View();
    }
}
