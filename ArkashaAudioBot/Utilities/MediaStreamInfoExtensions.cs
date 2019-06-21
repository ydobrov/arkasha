using System.Collections.Generic;
using System.Linq;
using YoutubeExplode.Models.MediaStreams;
using static YoutubeExplode.Models.MediaStreams.VideoQuality;

namespace ArkashaAudioBot.Utilities
{
    public static class MediaStreamInfoExtensions
    {
        public static IEnumerable<AudioStreamInfo> WhereMp4(this IReadOnlyList<AudioStreamInfo> audios)
        {
            return audios.Where(p => p.Container == Container.Mp4);
        }

        public static IEnumerable<MuxedStreamInfo> WhereVideoQualityIsRational(this IReadOnlyList<MuxedStreamInfo> videos)
        {
            return videos.Where(p =>
                p.VideoQuality.In(Low144, Low240, Medium360, Medium480, High720, High1080, High1440));
        }

        public static IEnumerable<T> WhereSizeUnder50MBytes<T>(this IEnumerable<T> medias) where T : MediaStreamInfo
        {
            var fiftyMBytesLimit = 50 * 1024 * 1024;
            return medias.Where(p => p.Size < fiftyMBytesLimit);
        }
    }
}
