using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kingwaytek.TrafficFlow.Repositories
{
    [MetadataType(typeof(PositioningMetadata))]
    public partial class Positioning
    {
        public InvestigationTypeEnum InvestigationTypeEnum
        {
            get { return (InvestigationTypeEnum)this.InvestigationType; }
            set { this.InvestigationType = (int)value; }
        }

        private class PositioningMetadata
        {
            [JsonIgnore]
            public virtual ICollection<Investigation> Investigation { get; set; }
        }
    }
}