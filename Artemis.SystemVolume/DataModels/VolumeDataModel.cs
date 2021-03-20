using Artemis.Core;
using Artemis.Core.DataModelExpansions;
using System.Collections.Generic;

namespace Artemis.SystemVolume.DataModels
{
    public class VolumeDataModel : DataModel
    {
        public int volume { get; set; }
        public bool muted { get; set; }
        public string DefaultDeviceName { get; set; }

        public DataModelEvent volumeChanged { get; set; } = new DataModelEvent();
    }
}