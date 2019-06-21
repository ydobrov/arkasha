using System;
using System.Linq;
using System.Threading.Tasks;
using ArkashaAudioBot.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using YoutubeExplode;
using YoutubeExplode.Models.MediaStreams;
using static ArkashaAudioBot.Utilities.ValidationMessages;

namespace ArkashaAudioBot.Services
{
    public class AudioService : IAudioService
    {
        public TelegramBotClient TelegramClient { get; }
        private readonly ILogger<AudioService> _logger;
        private readonly YoutubeClient _youtubeClient;

        private long ChatId { get; set; }

        public AudioService(YoutubeClient youtubeClient,
            ILogger<AudioService> logger,
            IOptions<BotConfiguration> config)
        {
            _youtubeClient = youtubeClient;
            _logger = logger;
            TelegramClient = new TelegramBotClient(config.Value.AudioBotToken);
        }

        public async Task GetAudio(Update update)
        {
            ChatId = update.Message.Chat.Id;
            var messageText = update.Message.Text;

            _logger.LogInformation($"Received a text message in chat {ChatId} with message {messageText}");

            if (messageText == "/start")
            {
                await SendTextMessage(StartCommandReplyMessage);
                return;
            }
            
            YoutubeClient.TryParseVideoId(messageText, out var videoId);

            if (videoId == null)
            {
                await SendTextMessage(InvalidLinkMessage);
                return;
            }

            var video = await _youtubeClient.GetVideoAsync(videoId);

            MediaStreamInfoSet streamInfoSet;

            try
            {
                streamInfoSet = await _youtubeClient.GetVideoMediaStreamInfosAsync(videoId);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Error for message {messageText} with GetVideoMediaStreamInfosAsync: {ex}");
                await SendTextMessage(SomeErrorMessage);
                return;
            }

            var streamInfo = streamInfoSet.Audio
                .WhereMp4()
                .WhereSizeUnder50MBytes()
                // TODO: To test and review sources how does WithHighestBitrate work
                .WithHighestBitrate();

            if (streamInfo == null)
            {
                var failMessage = streamInfoSet.Audio.WhereMp4().Any() ? FileSizeExceedLimitMessage : Mp4DoesNotExistsMessage;
                await SendTextMessage(failMessage);
                _logger.LogInformation($"Stream info for {messageText} was empty and error for user is {failMessage}");
                return;
            }

            await SendAudio(streamInfo, video);
        }

        private async Task SendAudio(AudioStreamInfo streamInfo, YoutubeExplode.Models.Video video)
        {
            using (var audioStream = await _youtubeClient.GetMediaStreamAsync(streamInfo))
                await TelegramClient.SendAudioAsync(ChatId, audioStream, video.Title, ParseMode.Default,
                    (int)video.Duration.TotalSeconds, video.Author, title: video.Title);
        }

        private async Task SendTextMessage(string message) => await TelegramClient.SendTextMessageAsync(ChatId, message);
    }
}
