using System.Threading.Tasks;
using ArkashaAudioBot.Services;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace ArkashaAudioBot.Controllers
{
    [Route("api/video")]
    public class VideoController : Controller
    {
        private readonly IVideoService _videoService;

        public VideoController(IVideoService videoService)
        {
            _videoService = videoService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Update update)
        {
            await _videoService.GetVideo(update);
            return Ok();
        }
    }
}
