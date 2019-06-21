using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace ArkashaAudioBot.Services
{
    public interface IAudioService
    {
        Task GetAudio(Update update);
    }
}
