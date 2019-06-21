using Microsoft.AspNetCore.Mvc;

namespace ArkashaAudioBot.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return Ok();
        }
    }
}
