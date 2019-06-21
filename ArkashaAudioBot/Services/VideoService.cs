using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using ArkashaAudioBot.Utilities;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using YoutubeExplode;
using YoutubeExplode.Models.MediaStreams;
using static ArkashaAudioBot.Utilities.ValidationMessages;

namespace ArkashaAudioBot.Services
{
    public class VideoService : IVideoService
    {
        public TelegramBotClient TelegramClient { get; }
        private readonly ILogger<AudioService> _logger;
        private readonly YoutubeClient _youtubeClient;

        private long ChatId { get; set; }

        public VideoService(YoutubeClient youtubeClient,
            ILogger<AudioService> logger,
            IOptions<BotConfiguration> config)
        {
            _youtubeClient = youtubeClient;
            _logger = logger;
            TelegramClient = new TelegramBotClient(config.Value.VideoBotToken);
        }

        public async Task GetVideo(Update update)
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
                _logger.LogInformation("Invalid link");
                return;
            }

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

            var streamInfo = streamInfoSet.Muxed
                .WhereVideoQualityIsRational()
                .WhereSizeUnder50MBytes()
                .WithHighestVideoQuality();

            if (streamInfo == null)
            {
                await SendTextMessage(FileSizeExceedLimitMessage);
                _logger.LogInformation("Stream  info was empty, file size exceed limit");
                return;
            }

            var ext = streamInfo.Container.GetFileExtension();

            var videoFileName = $"downloaded_video_{Guid.NewGuid().ToString()}.{ext}";

            try
            {
                await _youtubeClient.DownloadMediaStreamAsync(streamInfo, videoFileName);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Error when DownloadMediaStreamAsync: {ex} InnerException: {ex.InnerException}");
                throw;
            }

            _logger.LogInformation($"Saved video {videoFileName}");

            var video = await _youtubeClient.GetVideoAsync(videoId);

            _logger.LogInformation($"Video title: {video.Title}");

            using (var fStream = System.IO.File.OpenRead(videoFileName))
                await TelegramClient.SendVideoAsync(ChatId, fStream, (int)video.Duration.TotalSeconds, streamInfo.Resolution.Width, streamInfo.Resolution.Height, video.Title);

            System.IO.File.Delete(videoFileName);
        }

        private async Task SendTextMessage(string message) => await TelegramClient.SendTextMessageAsync(ChatId, message);
    }
}
