using System.Threading.Tasks;
using ArkashaAudioBot.Services;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace ArkashaAudioBot.Controllers
{
    [Route("api/audio")]
    public class AudioController : Controller
    {
        private readonly IAudioService _audioService;

        public AudioController(IAudioService audioService)
        {
            _audioService = audioService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Update update)
        {
            await _audioService.GetAudio(update);
            return Ok();
        }
    }
}
