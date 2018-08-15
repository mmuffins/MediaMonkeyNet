using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediaMonkeyNet
{
    /// <summary>Represents an album art object.</summary>  
    public class Cover
    {
        [JsonProperty]
        public int CoverStorage { get; private set; }

        [JsonProperty("coverTypeDesc")]
        public string CoverType { get; private set; }

        [JsonProperty]
        public string Description { get; private set; }

        [JsonProperty("_persistentID")]
        public string PersistentID { get; private set; }

        [JsonProperty("picturePath")]
        public string FilePath { get; private set; }

        [JsonProperty("pictureType")]
        public string FileType { get; private set; }
    }
}
