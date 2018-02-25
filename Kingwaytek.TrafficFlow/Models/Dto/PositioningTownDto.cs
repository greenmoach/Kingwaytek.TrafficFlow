using Newtonsoft.Json;

namespace Kingwaytek.TrafficFlow
{
    public class PositioningTownDto
    {
        [JsonProperty("townname")]
        public string TownName { get; set; }

        [JsonProperty("ridtown")]
        public string Id { get; set; }

        [JsonProperty("xy")]
        public string LatLon { get; set; }
    }
}