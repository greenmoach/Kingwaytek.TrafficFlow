using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Kingwaytek.TrafficFlow
{
    public class PositioningDto
    {
        [JsonProperty("ID")]
        public int Id { get; set; }

        [JsonProperty("YCOR")]
        public double Latitude { get; set; }

        [JsonProperty("XCOR")]
        public double Longitude { get; set; }

        [JsonProperty("CITYNAME_1")]
        public string CityName { get; set; }

        [JsonProperty("TOWNNAME_1")]
        public string TownName { get; set; }

        [JsonProperty("RD_NAME_1")]
        public string Road1 { get; set; }

        [JsonProperty("RD_NAME_2")]
        public string Road2 { get; set; }
    }
}