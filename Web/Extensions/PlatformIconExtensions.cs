using Web.Enums;

namespace Web.Extensions
{
    public static class PlatformIconExtensions
    {
        public static string GetIconUrl(this PlatformIcon platformIcon)
        {
            return platformIcon switch
            {
                PlatformIcon.Apple => "https://store.akamai.steamstatic.com/public/images/v6/icon_platform_mac.png",
                PlatformIcon.Windows => "https://store.akamai.steamstatic.com/public/images/v6/icon_platform_win.png?v=3",
                PlatformIcon.Linux => "https://store.akamai.steamstatic.com/public/images/v6/icon_platform_linux.png",
                _ => string.Empty
            };
        }
    }
}
