using Microsoft.AspNetCore.Mvc;
using fourHorsemen_Online_Video_Game_Database.Services;

namespace fourHorsemen_Online_Video_Game_Database.Controllers
{
    public class FunFactsController : Controller
    {
        public IActionResult Index()
        {
            var facts = FunFactsService.GetAllFacts();
            return View(facts);
        }
    }
}