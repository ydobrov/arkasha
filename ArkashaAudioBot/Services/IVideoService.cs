using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace ArkashaAudioBot.Services
{
    public interface IVideoService
    {
        Task GetVideo(Update update);
    }
}
