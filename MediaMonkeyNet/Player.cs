using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediaMonkeyNet
{
    public class Player
    {
        public bool IsMuted { get; set; }

        [JsonProperty]
        public bool IsPaused;

        [JsonProperty]
        public bool IsPlaying;

        [JsonProperty]
        public bool IsRepeat;

        [JsonProperty]
        public bool IsShuffle;

        [JsonProperty]
        public long TrackLength;

        [JsonProperty]
        public long TrackPosition;

        [JsonProperty]
        public double Volume;

        public double Progress
        {
            get
            {
                if (TrackLength == 0)
                {
                    return 0.0;
                }

                return ((double)TrackPosition / TrackLength);
            }
        }
    }
}
