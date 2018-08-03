using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MediaMonkeyNet
{
    /// <summary>Represents the player state of MediaMonkey.</summary>  
    public class Player
    {
        private bool playerRefreshInProgress;

        /// <summary>Gets a value indicating whether the player is muted.</summary>  
        [JsonProperty]
        public bool IsMuted { get; private set; }

        /// <summary>Gets a value indicating whether the player is paused.</summary>  
        [JsonProperty]
        public bool IsPaused { get; private set; }

        /// <summary>Gets a value indicating whether the player is playing.</summary>  
        [JsonProperty]
        public bool IsPlaying { get; private set; }

        /// <summary>Gets a value indicating whether the player is set to repeat.</summary>  
        [JsonProperty]
        public bool IsRepeat { get; private set; }

        /// <summary>Gets a value indicating whether the player set to shuffle.</summary>  
        [JsonProperty]
        public bool IsShuffle { get; private set; }

        /// <summary>Gets the <see cref="MediaMonkeySession"/> instance hosting the player.</summary>  
        public MediaMonkeySession Session { get; }

        /// <summary>Gets the length of the currently playing track in milliseconds.</summary>  
        [JsonProperty]
        public long TrackLength { get; private set; }

        /// <summary>Gets the playback position of the currently playing track in milliseconds.</summary>  
        [JsonProperty]
        public long TrackPosition { get; private set; }

        /// <summary>Gets the player volume from 0 to 1.</summary>  
        [JsonProperty]
        public double Volume { get; private set; }

        /// <summary>Gets the playback position of the currently playing track in percent.</summary>  
        public double Progress
        {
            get => TrackLength == 0 ? 0.0 : (double)TrackPosition / TrackLength;
        }

        /// <summary>Initializes a new instance of the <see cref="Player"/> class.</summary>  
        /// <param name="session">The <see cref="MediaMonkeySession"/> instance hosting the player.</param>
        public Player(MediaMonkeySession session)
        {
            Session = session;
        }

        /// <summary>Refreshes state information of the player.</summary>
        public async Task RefreshAsync()
        {
            // Prevent concurrent calls
            if (playerRefreshInProgress) { return; }

            playerRefreshInProgress = true;
            string cmd = "var playerState={'IsMuted':app.player.mute," +
                "'IsPaused':app.player.paused," +
                "'IsPlaying':app.player.isPlaying," +
                "'IsRepeat':app.player.repeatPlaylist," +
                "'IsShuffle':app.player.shufflePlaylist," +
                "'TrackLength':app.player.trackLengthMS," +
                "'TrackPosition':app.player.trackPositionMS," +
                "'Volume':app.player.volume};" +
                "playerState";

            var mmState = (await Session.SendCommandAsync(cmd).ConfigureAwait(false)).Result;
            if(mmState.Value != null)
            {
                JsonConvert.PopulateObject(mmState.Value.ToString(), this);
            }
            playerRefreshInProgress = false;
        }

        /// <summary>Plays the next file in the current playlist.</summary>
        public Task NextTrackAsync()
        {
            return Session.SendCommandAsync("app.player.nextAsync()");
        }

        /// <summary>Pauses Playback.</summary>
        public Task PausePlaybackAsync()
        {
            return Session.SendCommandAsync("app.player.pauseAsync()");
        }

        /// <summary>Plays the previous file in the current playlist.</summary>
        public Task PreviousTrackAsync()
        {
            return Session.SendCommandAsync("app.player.prevAsync()");
        }

        /// <summary>Enables or disables mute.</summary>
        public Task SetMuteAsync(bool enabled)
        {
            return Session.SendCommandAsync("app.player.mute = " + enabled.ToString().ToLower());
        }

        /// <summary>Sets the progress of the currently playing track.</summary>
        /// <param name="progress">Progress of as decimal value between 0 and 1.</param>
        public Task SetProgressAsync(double progress)
        {
            if (progress < 0) { progress = 0; }
            if (progress > 1) { progress = 1; }

            return SetTrackPositionAsync((long)(TrackLength * progress));
        }

        /// <summary>Enables or disables repeat mode.</summary>
        public Task SetRepeatAsync(bool enabled)
        {
            return Session.SendCommandAsync("app.player.repeatPlaylist = " + enabled.ToString().ToLower());
        }

        /// <summary>Enables or disables shuffle.</summary>
        public Task SetShuffleAsync(bool enabled)
        {
            return Session.SendCommandAsync("app.player.shufflePlaylist = " + enabled.ToString().ToLower());
        }

        /// <summary>Updates the state properties according the passed string.</summary>
        public void SetPlayerState(string state)
        {
            switch (state)
            {
                case "play":
                    IsPlaying = true;
                    IsPaused = false;
                    break;

                case "unpause":
                    IsPlaying = true;
                    IsPaused = true;
                    break;

                case "stop":
                    IsPlaying = false;
                    IsPaused = false;
                    break;

                default:
                    Console.WriteLine("Unknown:" + state);
                    break;
            }
        }

        /// <summary>Seek to the provided track position.</summary>
        /// <param name="position">Position of the track in milliseconds.</param>
        public Task SetTrackPositionAsync(long position)
        {
            return Session.SendCommandAsync("app.player.seekMSAsync(" + position + ")");
        }

        /// <summary>Sets the player volume.</summary>
        /// <param name="progress">Player volume as decimal value between 0 and 1.</param>
        public Task SetVolumeAsync(double volume)
        {
            // Values outside 0 and 1 are automatically converted to 0/1 by mediamonkey
            var nfi = new System.Globalization.NumberFormatInfo()
            {
                NumberDecimalSeparator = "."
            };

            return Session.SendCommandAsync("app.player.volume = " + volume.ToString(nfi));
        }

        /// <summary>Starts playback.</summary>
        public Task StartPlaybackAsync()
        {
            return Session.SendCommandAsync("app.player.playAsync()");
        }

        /// <summary>Stops playback.</summary>
        public Task StopPlaybackAsync()
        {
            return Session.SendCommandAsync("app.player.stopAsync()");
        }

        /// <summary>Toggles play and pause status.</summary>
        public Task TogglePlaybackAsync()
        {
            return Session.SendCommandAsync("app.player.playPauseAsync()");
        }

    }
}
