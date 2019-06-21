using Telegram.Bot;

namespace SetWebhook
{
    public class Program
    {
        public static readonly string NgrokUrl = "https://fc9c0bdf.ngrok.io";

        private static void Main()
        {
            var audioBotClient = new TelegramBotClient("token");
            audioBotClient.DeleteWebhookAsync().Wait();

            var videoBotClient = new TelegramBotClient("token");
            videoBotClient.DeleteWebhookAsync().Wait();

            var videoBotClientSecond = new TelegramBotClient("token");
            videoBotClient.DeleteWebhookAsync().Wait();

            const bool useNgrok = true;

            var actualUrl = useNgrok ? NgrokUrl : "UrlForDeploy";

            audioBotClient.SetWebhookAsync($"{actualUrl}/api/audio").Wait();

            videoBotClient.SetWebhookAsync($"{actualUrl}/api/video").Wait();
            videoBotClientSecond.SetWebhookAsync("").Wait();
        }
    }
}
