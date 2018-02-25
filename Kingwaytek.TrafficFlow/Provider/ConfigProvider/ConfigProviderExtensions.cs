namespace Kingwaytek.TrafficFlow
{
    public static class ConfigProviderExtensions
    {
        public static bool IsProduction(this IConfigProvider config)
        {
            return config.Get("IsProduction", false);
        }

        public static string VersionNumber(this IConfigProvider config)
        {
            return config.Get("VersionNumber", "1.0.0");
        }
    }
}