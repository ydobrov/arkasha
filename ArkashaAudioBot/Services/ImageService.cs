using System;
using System.IO;
using System.Net;
using Microsoft.Extensions.Logging;
using YoutubeExplode.Models;

namespace ArkashaAudioBot.Services
{
    public class ImageService
    {
        private readonly ILogger<AudioService> _logger;

        public ImageService(ILogger<AudioService> logger)
        {
            _logger = logger;
        }

        public Stream GetValidImage(ThumbnailSet imageSet)
        {
            return GetImageStream(imageSet.HighResUrl) ??
                   GetImageStream(imageSet.StandardResUrl) ??
                   GetImageStream(imageSet.MaxResUrl) ??
                   GetImageStream(imageSet.MediumResUrl) ??
                   GetDefaultImage();
        }

        private static Stream GetDefaultImage()
        {
            var file = new FileStream("Content/default-img.png", FileMode.Open);
            return file;
        }

        private Stream GetImageStream(string url)
        {
            if (string.IsNullOrEmpty(url))
                return null;

            try
            {
                var uriBuilder = new UriBuilder(url);
                var request = (HttpWebRequest)WebRequest.Create(uriBuilder.Uri);
                var response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.NotFound)
                    _logger.LogInformation($"Broken for url {url}. 404 Not Found.");
                else
                    return response.GetResponseStream();
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Broken for url {url}. Other error: {ex.Message}.");
            }

            return null;
        }
    }
}
