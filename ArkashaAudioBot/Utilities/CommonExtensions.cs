using System.Linq;

namespace ArkashaAudioBot.Utilities
{
    public static class CommonExtensions
    {
        public static bool In<T>(this T value, params T[] list)
        {
            return list?.Contains(value) == true;
        }

        public static bool In<T>(this T? value, params T[] list)
            where T : struct
        {
            return value.HasValue && value.Value.In(list);
        }

        public static bool In<T>(this T value, params T?[] list)
            where T : struct
        {
            return list?.Contains(value) == true;
        }
    }
}
