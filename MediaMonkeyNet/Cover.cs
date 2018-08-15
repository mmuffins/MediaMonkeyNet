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

        [JsonProperty]
        public int CoverType { get; private set; }

        //public string CoverTypeDesc { get; private set; }

        //public bool Deleted { get; private set; }

        [JsonProperty]
        public string Description { get; private set; }

        [JsonProperty("_persistentID")]
        public string PersistentID { get; private set; }

        [JsonProperty]
        public string PicturePath { get; private set; }

        [JsonProperty]
        public string PictureType { get; private set; }
    }
}
