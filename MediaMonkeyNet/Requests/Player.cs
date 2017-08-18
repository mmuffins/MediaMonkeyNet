using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MediaMonkeyNet
{
    public class Player
    {

        private MediaMonkeyNet _Session;

        public MediaMonkeyNet Session
        {
            get { return _Session; }
            set { _Session = value; }
        }

        [JsonProperty]
        public bool IsMuted;

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

        public double Progress {
            get {
                if(TrackLength == 0)
                {
                    return 0.0;
                }

                return ((double)TrackPosition /TrackLength);
            }
        }

        public Player() {}

        public Player(MediaMonkeyNet Session)
        {
            this.Session = Session;
        }

        private void CheckSession()
        {
            if (_Session == null)
            {
                throw new NullReferenceException("No remote session is defined.");
            }
        }

        async public Task Refresh()
        {
            /// <summary>
            /// Refreshes properties of the object with data from the remote session
            /// </summary>

            if (_Session == null)
            {
                throw new NullReferenceException("No remote session is defined.");
            }

            string cmd = "function PlayerStatus(){var dict={};"
                + "dict['IsMuted']=app.player.mute;"
                + "dict['IsPaused']=app.player.paused;"
                + "dict['isPlaying']=app.player.isPlaying;"
                + "dict['IsRepeat']=app.player.repeatPlaylist;"
                + "dict['IsShuffle']=app.player.shufflePlaylist;"
                + "dict['TrackLength']=app.player.trackLengthMS;"
                + "dict['TrackPosition']=app.player.trackPositionMS;"
                + "dict['Volume']=app.player.volume;"
                + "return JSON.stringify(dict)};PlayerStatus();";

            var remotePlayer = _Session.Evaluate<string>(cmd, true);

            if (remotePlayer.Exception != null || remotePlayer.Value == null)
            {
                return;
            }
            // JsonConvert.PopulateObjectAsync apparently is obsolete

            await Task.Factory.StartNew(() =>JsonConvert.PopulateObject(remotePlayer.Value, this));

        }


        public EvaluateResponse<object> NextTrack()
        {
            /// <summary>
            /// Plays the next file in the current playlist
            /// </summary>

            CheckSession();
            return _Session.Evaluate<object>("app.player.nextAsync()");
        }

        public EvaluateResponse<object> PausePlayback()
        {
            /// <summary>
            /// Pauses Playback
            /// </summary>

            CheckSession();

            return _Session.Evaluate<object>("app.player.pauseAsync()");
        }

        public EvaluateResponse<object> PreviousTrack()
        {
            /// <summary>
            /// Plays the previous file in the current playlist
            /// </summary>

            CheckSession();
            return _Session.Evaluate<object>("app.player.prevAsync()");
        }

        public EvaluateResponse<object> SetMute(bool enabled)
        {
            /// <summary>
            /// Sets mute status
            /// </summary>

            CheckSession();
            return _Session.Evaluate<object>("app.player.mute = " + enabled.ToString().ToLower());
        }

        public EvaluateResponse<object> SetProgress(double progress)
        {
            /// <summary>
            /// Sets the progress of the currently playing track as percentage between 0 and 1.
            /// </summary>
            /// 

            CheckSession();

            if (progress < 0)
            {
                progress = 0;
            }

            if (progress > 1)
            {
                progress = 1;
            }

            return SetTrackPosition((long)(TrackLength * progress));
        }

        public EvaluateResponse<object> SetRepeat(bool enabled)
        {
            /// <summary>
            /// Sets repeat status
            /// </summary>

            CheckSession();
            return _Session.Evaluate<object>("app.player.repeatPlaylist = " + enabled.ToString().ToLower());
        }

        public EvaluateResponse<object> SetShuffle(bool enabled)
        {
            /// <summary>
            /// Sets shuffle status
            /// </summary>

            CheckSession();
            return _Session.Evaluate<object>("app.player.shufflePlaylist = " + enabled.ToString().ToLower());
        }

        public EvaluateResponse<object> SetTrackPosition(long position)
        {
            /// <summary>
            /// Seek to the provided track time, in ms
            /// </summary>

            CheckSession();
            return _Session.Evaluate<object>("app.player.seekMSAsync(" + position + ")");
        }

        public EvaluateResponse<object> SetVolume(double volume)
        {
            /// <summary>
            /// Sets the current value between 0 and 1.
            /// </summary>

            CheckSession();

            // Values outside 0 and 1 are automatically converted to 0/1 by mediamonkey
            var nfi = new System.Globalization.NumberFormatInfo()
            {
                NumberDecimalSeparator = "."
            };

            return _Session.Evaluate<object>("app.player.volume = " + volume.ToString(nfi));
        }

        public EvaluateResponse<object> StartPlayback()
        {
            /// <summary>
            /// Starts Playback
            /// </summary>

            CheckSession();
            return _Session.Evaluate<object>("app.player.playAsync()");
        }

        public EvaluateResponse<object> StopPlayback()
        {
            /// <summary>
            /// Stops playback
            /// </summary>

            CheckSession();
            return _Session.Evaluate<object>("app.player.stopAsync()");
        }

        public EvaluateResponse<object> TogglePlayback()
        {
            /// <summary>
            /// Toggles play and pause status
            /// </summary>

            CheckSession();
            return _Session.Evaluate<object>("app.player.playPauseAsync()");
        }
    }
}
