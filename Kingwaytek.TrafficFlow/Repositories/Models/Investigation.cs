using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kingwaytek.TrafficFlow.Repositories
{
    [MetadataType(typeof(InvestigationMetadata))]
    public partial class Investigation : IEditColumn, IDeleted
    {
        public InvestigationTypeEnum InvestigationTypeEnum
        {
            get { return (InvestigationTypeEnum)this.InvestigationType; }
            set { this.InvestigationType = (int)value; }
        }

        private class InvestigationMetadata
        {
            [JsonIgnore]
            public virtual ICollection<InvestigationData> InvestigationData { get; set; }
        }
    }
}