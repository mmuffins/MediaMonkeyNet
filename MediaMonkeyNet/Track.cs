using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace MediaMonkeyNet
{
    public class Track
    {
        public string Actors { get; set; }

        public string AlbumArtistName { get; set; }

        public int AlbumID { get; private set; }

        public string AlbumName { get; set; }

        public string ArtistName { get; set; }

        public string Author { get; set; }

        public int Bitrate { get; set; }

        public int BPM { get; set; }

        public string Conductor { get; set; }

        public string Copyright { get; set; }

        public string Custom1 { get; set; }

        public string Custom2 { get; set; }

        public string Custom3 { get; set; }

        public string Custom4 { get; set; }

        public string Custom5 { get; set; }

        public string Date { get; set; }

        public DateTime DateAdded { get; set; }

        public int Day { get; set; }

        public string Director { get; set; }

        public string DiscNumber { get; set; }

        public string Encoder { get; set; }

        public string EpisodeNumber { get; set; }

        public double FileLength { get; set; }

        public DateTime FileModified { get; set; }

        public string FileName { get; private set; }

        public string FileType { get; private set; }

        public int Frequency { get; private set; }

        public string Genre { get; set; }

        public string Grouping { get; set; }

        public int ID { get; set; }

        public string InvolvedPeople { get; set; }

        public bool IsntInDB { get; set; }

        public string ISRC { get; set; }

        public bool Isvideo { get; private set; }

        public DateTime LastPlayed { get; set; }

        public double Leveling { get; set; }

        public string Lyricist { get; set; }

        public int MediaID { get; private set; }

        public string MediaLabel { get; private set; }

        public int Month { get; set; }

        public string Mood { get; set; }

        public string MusicComposer { get; set; }

        public string Occasion { get; set; }

        public string OriginalArtist { get; set; }

        public int OriginalDate { get; set; }

        public int OriginalDay { get; set; }

        public string OriginalLyricist { get; set; }

        public int OriginalMonth { get; set; }

        public string OriginalTitle { get; set; }

        public int OriginalYear { get; set; }

        public string ParentalRating { get; set; }

        public string Path { get; set; }

        public string PersistentID { get; private set; }

        public int PlayCounter { get; set; }

        public int PlayListID { get; private set; }

        public int PlaylistOrder { get; set; }

        public string Producer { get; set; }

        public string Publisher { get; set; }

        public string Quality { get; set; }

        public int Rating { get; set; }

        public string SeasonNumber { get; set; }

        public int SkipCount { get; set; }

        public int SongID { get; private set; }

        public int SongLength { get; set; }

        public int StartTime { get; set; }

        public int StopTime { get; set; }

        public string Summary { get; private set; }

        public string Tempo { get; set; }

        public string Title { get; set; }

        public string TrackOrder { get; set; }

        public int TrackType { get; set; }

        public string TrackTypeStr { get; private set; }

        public bool VBR { get; set; }

        public int Year { get; set; }


        public Track(JObject Jobject)
        {
            Actors = (string)Jobject.GetValue("actors");

            AlbumArtistName = (string)Jobject.GetValue("albumArtist");

            AlbumID = (int)Jobject.GetValue("idalbum");

            AlbumName = (string)Jobject.GetValue("album");

            ArtistName = (string)Jobject.GetValue("artist");

            Author = (string)Jobject.GetValue("author");

            Bitrate = (int)Jobject.GetValue("bitrate");

            BPM = (int)Jobject.GetValue("bpm");

            Conductor = (string)Jobject.GetValue("conductor");

            Copyright = (string)Jobject.GetValue("copyright");

            Custom1 = (string)Jobject.GetValue("custom1");

            Custom2 = (string)Jobject.GetValue("custom2");

            Custom3 = (string)Jobject.GetValue("custom3");

            Custom4 = (string)Jobject.GetValue("custom4");

            Custom5 = (string)Jobject.GetValue("custom5");

            Date = (string)Jobject.GetValue("date");


            var dateAdded_UTC = Jobject.GetValue("dateAdded_UTC");
            if (dateAdded_UTC.HasValues)
            {
                DateTime.TryParse((string)dateAdded_UTC, out DateTime parsedDateAdded);
                DateAdded = parsedDateAdded;
            }

            Day = (int)Jobject.GetValue("day");

            Director = (string)Jobject.GetValue("director");

            DiscNumber = (string)Jobject.GetValue("discNumber");

            Encoder = (string)Jobject.GetValue("encoder");

            EpisodeNumber = (string)Jobject.GetValue("episodeNumber");

            FileLength = (double)Jobject.GetValue("fileLength");

            var fileModified_UTC = Jobject.GetValue("fileModified_UTC");
            if (fileModified_UTC.HasValues)
            {
                DateTime.TryParse((string)fileModified_UTC, out DateTime parsedFileModified);
                FileModified = parsedFileModified;
            }

            FileName = (string)Jobject.GetValue("filename");

            FileType = (string)Jobject.GetValue("fileType");

            Frequency = (int)Jobject.GetValue("frequency");

            Genre = (string)Jobject.GetValue("genre");

            Grouping = (string)Jobject.GetValue("groupDesc");

            ID = (int)Jobject.GetValue("id");

            InvolvedPeople = (string)Jobject.GetValue("involvedPeople");

            IsntInDB = (bool)Jobject.GetValue("isntInDB");

            ISRC = (string)Jobject.GetValue("isrc");

            Isvideo = (bool)Jobject.GetValue("isVideo");

            var lastTimePlayed_UTC = Jobject.GetValue("lastTimePlayed_UTC");
            if (lastTimePlayed_UTC.HasValues)
            {
                DateTime.TryParse((string)lastTimePlayed_UTC, out DateTime parsedLastPlayed);
                LastPlayed = parsedLastPlayed;
            }

            Leveling = (double)Jobject.GetValue("volumeLeveling");

            Lyricist = (string)Jobject.GetValue("lyricist");

            MediaID = (int)Jobject.GetValue("idMedia");

            MediaLabel = (string)Jobject.GetValue("mediaLabel");

            Month = (int)Jobject.GetValue("month");

            Mood = (string)Jobject.GetValue("mood");

            MusicComposer = (string)Jobject.GetValue("composer");

            Occasion = (string)Jobject.GetValue("occasion");

            OriginalArtist = (string)Jobject.GetValue("origArtist");

            OriginalDate = (int)Jobject.GetValue("origDate");

            OriginalDay = (int)Jobject.GetValue("origDay");

            OriginalLyricist = (string)Jobject.GetValue("origLyricist");

            OriginalMonth = (int)Jobject.GetValue("origMonth");

            OriginalTitle = (string)Jobject.GetValue("origTitle");

            OriginalYear = (int)Jobject.GetValue("origYear");

            ParentalRating = (string)Jobject.GetValue("parentalRating");

            Path = (string)Jobject.GetValue("path");

            PersistentID = (string)Jobject.GetValue("persistentID");

            PlayCounter = (int)Jobject.GetValue("playCounter");

            PlayListID = (int)Jobject.GetValue("idPlaylistSong");

            PlaylistOrder = (int)Jobject.GetValue("playlistSongOrder");

            Producer = (string)Jobject.GetValue("producer");

            Publisher = (string)Jobject.GetValue("publisher");

            Quality = (string)Jobject.GetValue("quality");

            Rating = (int)Jobject.GetValue("rating");

            SeasonNumber = (string)Jobject.GetValue("seasonNumber");

            SkipCount = (int)Jobject.GetValue("skipCount");

            SongID = (int)Jobject.GetValue("idsong");

            SongLength = (int)Jobject.GetValue("songLength");

            StartTime = (int)Jobject.GetValue("startTime");

            StopTime = (int)Jobject.GetValue("stopTime");

            Summary = (string)Jobject.GetValue("summary");

            Tempo = (string)Jobject.GetValue("tempo");

            Title = (string)Jobject.GetValue("title");

            TrackOrder = (string)Jobject.GetValue("trackNumber");

            TrackType = (int)Jobject.GetValue("trackType");

            TrackTypeStr = (string)Jobject.GetValue("trackTypeStr");

            VBR = (bool)Jobject.GetValue("vbr");

            Year = (int)Jobject.GetValue("year");
        }

        public EvaluateResponse<object> SetRating(MediaMonkeyNet RemoteSession, int rating)
        {
            /// <summary>
            /// Sets Rating of the currently playing track
            /// </summary>
            /// <param name="RemoteSession">MediaMonkeyNet Remote Session</param>
            /// <param name="rating">Rating of the track between 0 and 100</param>

            return RemoteSession.SetRating(rating, SongID);
        }

    }
}

