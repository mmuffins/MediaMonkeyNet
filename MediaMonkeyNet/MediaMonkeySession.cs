using BaristaLabs.ChromeDevTools.Runtime;
using BaristaLabs.ChromeDevTools.Runtime.Runtime;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MediaMonkeyNet
{
    /// <summary>Represents a MediaMonkey remote session.</summary>  
    public class MediaMonkeySession : IDisposable
    {
        private const int defaultConnectionPort = 9222;
        private const string defaultConnectionAddress = "127.0.0.1";
        private const string mmWebsocketUrl = "file:///mainwindow.html";
        private const int mmSessionTimeout = 1000;
        private bool currentTrackRefreshInProgress;

        private ChromeSession mmSession;

        /// <summary>Gets the chromium websocket address of the current MediaMonkey instance.</summary>  
        public string EndpointAddress { get; private set; }

        /// <summary>Gets the Uri used for remote debugging of the current MediaMonkey instance.</summary>  
        public string RemoteDebuggingUri { get; }

        /// <summary>Gets player object for the current MediaMonkey instance.</summary>  
        public Player Player { get; }

        /// <summary>Gets the currently playing track.</summary>  
        public Track CurrentTrack { get; private set; }

        /// <summary>Initializes a new instance of the <see cref="MediaMonkeySession"/> class.</summary>  
        public MediaMonkeySession() : this(defaultConnectionAddress, defaultConnectionPort) { }

        /// <summary>Initializes a new instance of the <see cref="MediaMonkeySession"/> class for a specific host.</summary>  
        /// <param name="ConnectionAddress">The IP address or hostname to connect to.</param>
        /// <param name="ConnectionPort">The port number to connect to.</param>
        public MediaMonkeySession(string ConnectionAddress, int ConnectionPort)
        {
            RemoteDebuggingUri = $"http://{ConnectionAddress}:{ConnectionPort.ToString()}";
            Player = new Player(this);
            CurrentTrack = new Track();
        }

        /// <summary>Opens a session to the chromium instance hosting MediaMonkey.</summary>  
        public async Task OpenSessionAsync()
        {
            using (var webClient = new HttpClient())
            {
                webClient.BaseAddress = new Uri(RemoteDebuggingUri);
                var remoteSessions = await webClient.GetStringAsync("/json").ConfigureAwait(false);
                var webSockets =  JsonConvert.DeserializeObject<ICollection<ChromeSessionInfo>>(remoteSessions);
                EndpointAddress = (webSockets.First(s => s.Url == mmWebsocketUrl)).WebSocketDebuggerUrl.Replace("ws://localhost", "ws://127.0.0.1");
                mmSession = new ChromeSession(EndpointAddress);
                mmSession.CommandTimeout = mmSessionTimeout;
            }
        }

        /// <summary>
        /// Sends the specified command to MediaNMonkey and returns the associated command response.</summary>
        /// <param name="command">The command expression to send to MediaMonkey.</param>
        public async Task<EvaluateCommandResponse> SendCommandAsync(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                throw new ArgumentNullException(nameof(command));
            }

            if (mmSession is null)
            {
                throw new NullReferenceException("No active MediaMonkey session found.");
            }

            var cmd = new EvaluateCommand()
            {
                ObjectGroup = "console",
                IncludeCommandLineAPI = true,
                ReturnByValue = true,
                AwaitPromise = true,
                Silent = false,
                Expression = command
            };
            
            return await mmSession.SendCommand(cmd).ConfigureAwait(false) as EvaluateCommandResponse;
        }

        /// <summary>
        /// Refreshes the currently playing track.</summary>
        public async Task RefreshCurrentTrackAsync()
        {
            // Prevent concurrent calls
            if (currentTrackRefreshInProgress) { return; }

            currentTrackRefreshInProgress = true;
            var track = (await SendCommandAsync("app.player.getCurrentTrack()").ConfigureAwait(false)).Result;
            if(track.Value != null)
            {
                var serializerSettings = new JsonSerializerSettings
                {
                    MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
                    DateParseHandling = DateParseHandling.None,
                    Converters = {
                        new IsoDateTimeConverter { DateTimeStyles = System.Globalization.DateTimeStyles.AssumeUniversal }
                    },
                };

                // workaround for invalid json in mm alpha rev 2116
                var trackString = track.Value.ToString().Replace("tempString\":\"\"\"extendedTags", "tempString\":\"\",\"extendedTags");
                trackString = track.Value.ToString();

                CurrentTrack = JsonConvert.DeserializeObject<Track>(track.Value.ToString(), serializerSettings);
                currentTrackRefreshInProgress = false;
            }
        }

        /// <summary>Sets Rating of the track with the provided ID.</summary>
        /// <param name="rating">Rating of the track between 0 and 100.</param>
        /// <param name="ID">SongID property of the track.</param>
        public Task SetRating(int rating, int ID)
        {
            return SendCommandAsync("app.getObject('track', { id:" + ID
                + "}).then(function(track){ if (track) {track.rating ="
                + rating + "; track.commitAsync();}});");
        }

        /// <summary>Sets Rating of the provided track.</summary>
        /// <param name="rating">Rating of the track between 0 and 100.</param>
        /// <param name="track">Track object of the track.</param>
        public Task SetRating(int rating, Track track)
        {
            return SetRating(rating, track.SongID);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                //if (disposing)
                //{
                //    // dispose managed state (managed objects).
                //}

                if(mmSession != null)
                {
                    mmSession.Dispose();
                }
                disposedValue = true;
            }
        }

        ~MediaMonkeySession()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}
