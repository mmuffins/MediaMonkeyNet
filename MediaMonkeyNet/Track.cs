﻿using BaristaLabs.ChromeDevTools.Runtime.Runtime;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MediaMonkeyNet
{
    /// <summary>Represents a song object.</summary>  
    public class Track
    {
        [JsonProperty]
        public string Actors { get; private set; }

        [JsonProperty]
        public string AlbumArtist { get; private set; }

        [JsonProperty]
        public int AlbumID { get; private set; }

        [JsonProperty]
        public string Album { get; private set; }

        [JsonProperty]
        public string Artist { get; private set; }

        [JsonProperty]
        public string Author { get; private set; }

        [JsonProperty]
        public int Bitrate { get; private set; }

        [JsonProperty]
        public int BPM { get; private set; }

        [JsonProperty]
        public string Conductor { get; private set; }

        [JsonProperty]
        public string Copyright { get; private set; }

        [JsonProperty]
        public string Custom1 { get; private set; }

        [JsonProperty]
        public string Custom2 { get; private set; }

        [JsonProperty]
        public string Custom3 { get; private set; }

        [JsonProperty]
        public string Custom4 { get; private set; }

        [JsonProperty]
        public string Custom5 { get; private set; }

        [JsonProperty]
        public string Custom6 { get; private set; }

        [JsonProperty]
        public string Custom7 { get; private set; }

        [JsonProperty]
        public string Custom8 { get; private set; }

        [JsonProperty]
        public string Custom9 { get; private set; }

        [JsonProperty]
        public string Custom10 { get; private set; }

        [JsonProperty]
        public string Date { get; private set; }

        ////[JsonProperty]
        ////public DateTime DateAdded { get; private set; }

        [JsonProperty]
        public int Day { get; private set; }

        [JsonProperty]
        public bool Deleted { get; private set; }

        [JsonProperty]
        public string Director { get; private set; }

        [JsonProperty]
        public string DiscNumber { get; private set; }

        [JsonProperty]
        public int DiscNumberInt { get; private set; }

        [JsonProperty]
        public string Encoder { get; private set; }

        [JsonProperty]
        public string EpisodeNumber { get; private set; }

        [JsonProperty]
        public double FileLength { get; private set; }

        ////[JsonProperty("fileModified_UTC", NullValueHandling = NullValueHandling.Ignore)]
        //public DateTime FileModified { get; private set; }

        [JsonProperty]
        public string FileName { get; private set; }

        [JsonProperty]
        public string FileType { get; private set; }

        [JsonProperty]
        public int Frequency { get; private set; }

        [JsonProperty]
        public string Genre { get; private set; }

        [JsonProperty("groupDesc")]
        public string Grouping { get; private set; }

        [JsonProperty]
        public int ID { get; private set; }

        [JsonProperty]
        public string InvolvedPeople { get; private set; }

        [JsonProperty]
        public bool IsntInDB { get; private set; }

        [JsonProperty]
        public string ISRC { get; private set; }

        [JsonProperty]
        public bool Isvideo { get; private set; }

        [JsonProperty]
        public bool IsPlaying { get; private set; }

        //[JsonProperty]
        //public DateTime LastPlayed { get; private set; }

        [JsonProperty]
        public double VolumeLeveling { get; private set; }

        [JsonProperty]
        public string Lyricist { get; private set; }

        [JsonProperty]
        public double MaxSample { get; private set; }

        [JsonProperty("idMedia")]
        public int MediaID { get; private set; }

        [JsonProperty]
        public string MediaLabel { get; private set; }

        [JsonProperty]
        public long MediaSN { get; private set; }

        [JsonProperty]
        public string MimeType { get; private set; }

        [JsonProperty]
        public int Month { get; private set; }

        [JsonProperty]
        public string Mood { get; private set; }

        [JsonProperty]
        public double NormalizeAlbum { get; private set; }

        [JsonProperty]
        public double NormalizeTrack { get; private set; }

        [JsonProperty]
        public string Composer { get; private set; }

        [JsonProperty]
        public string Occasion { get; private set; }

        [JsonProperty]
        public string ObjectType { get; private set; }

        [JsonProperty("origArtist")]
        public string OriginalArtist { get; private set; }

        [JsonProperty("origDate")]
        public int OriginalDate { get; private set; }

        [JsonProperty("origDay")]
        public int OriginalDay { get; private set; }

        [JsonProperty("origLyricist")]
        public string OriginalLyricist { get; private set; }

        [JsonProperty("origMonth")]
        public int OriginalMonth { get; private set; }

        [JsonProperty("origTitle")]
        public string OriginalTitle { get; private set; }

        [JsonProperty("origYear")]
        public int OriginalYear { get; private set; }

        [JsonProperty]
        public string ParentalRating { get; private set; }

        [JsonProperty]
        public string Path { get; private set; }

        [JsonProperty]
        public string PersistentID { get; private set; }

        [JsonProperty]
        public int PlayCounter { get; private set; }

        [JsonProperty]
        public int PlayListID { get; private set; }

        [JsonProperty]
        public int PlaylistOrder { get; private set; }

        [JsonProperty]
        public string Producer { get; private set; }

        [JsonProperty]
        public string Publisher { get; private set; }

        [JsonProperty]
        public string Quality { get; private set; }

        [JsonProperty]
        public int Rating { get; private set; }

        [JsonProperty]
        public string SeasonNumber { get; private set; }

        [JsonProperty]
        public int SkipCount { get; private set; }

        [JsonProperty]
        public int SongID { get; private set; }

        [JsonProperty]
        public int SongLength { get; private set; }

        [JsonProperty]
        public int StartTime { get; private set; }

        [JsonProperty]
        public string Stereo { get; private set; }

        [JsonProperty]
        public int StopTime { get; private set; }

        [JsonProperty]
        public string Summary { get; private set; }

        [JsonProperty]
        public string SyncId { get; private set; }

        [JsonProperty]
        public string Tempo { get; private set; }

        [JsonProperty]
        public int TemporaryOrder { get; private set; }

        [JsonProperty]
        public string Title { get; private set; }

        [JsonProperty]
        public string TrackNumber { get; private set; }

        [JsonProperty]
        public int TrackNumberInt { get; private set; }

        [JsonProperty]
        public int TrackType { get; private set; }

        [JsonProperty]
        public string TrackTypeStr { get; private set; }

        [JsonProperty]
        public string trackTypeStringId { get; private set; }

        [JsonProperty]
        public bool VBR { get; private set; }

        [JsonProperty]
        public string WebSource { get; private set; }

        [JsonProperty]
        public int Year { get; private set; }

        /// <summary>Initializes a new instance of the <see cref="Track"/> class.</summary>  
        public Track() { }

        /// <summary>Initializes a new instance of the <see cref="Track"/> class from a ChromeDevTools RemoteObject.</summary>  
        /// <param name="remoteObject">The RemoteObject instance to deserialize.</param>
        public Track(RemoteObject remoteObject)
        {
            if(remoteObject.Value is null) { return; }

            var serializerSettings = new JsonSerializerSettings
            {
                MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
                DateParseHandling = DateParseHandling.None,
                Converters = {
                        new IsoDateTimeConverter { DateTimeStyles = System.Globalization.DateTimeStyles.AssumeUniversal }
                },
            };

            var trackString = remoteObject.Value.ToString();

            // workaround for invalid json in mm alpha rev 2116
            var trackJson = trackString.Replace("tempString\":\"\"\"extendedTags", "tempString\":\"\",\"extendedTags");

            JsonConvert.PopulateObject(remoteObject.Value.ToString(), this, serializerSettings);
        }
    }
}
