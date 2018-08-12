using BaristaLabs.ChromeDevTools.Runtime;
using BaristaLabs.ChromeDevTools.Runtime.Runtime;
using Newtonsoft.Json;
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
            CurrentTrack = new Track(this);
        }

        /// <summary>Opens a session to the chromium instance hosting MediaMonkey.</summary>  
        public async Task OpenSessionAsync()
        {
            using (var webClient = new HttpClient())
            {
                webClient.BaseAddress = new Uri(RemoteDebuggingUri);
                string remoteSessions = await webClient.GetStringAsync("/json").ConfigureAwait(false);
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
            RemoteObject track = (await SendCommandAsync("app.player.getCurrentTrack()").ConfigureAwait(false)).Result;

            CurrentTrack = new Track(track, this);
            currentTrackRefreshInProgress = false;
        }

        /// <summary>Sets Rating of the track with the provided ID.</summary>
        /// <param name="rating">Rating of the track between 0 and 100.</param>
        /// <param name="ID">SongID property of the track.</param>
        private Task SetRatingAsync(int rating, int ID)
        {
            return SendCommandAsync("app.getObject('track', { id:" + ID
                + "}).then(function(track){ if (track) {track.rating ="
                + rating + "; track.commitAsync();}});");
        }

        /// <summary>Sets Rating of the provided track.</summary>
        /// <param name="rating">Rating of the track between 0 and 100.</param>
        /// <param name="track">Track object of the track.</param>
        private Task SetRatingAsync(int rating, Track track)
        {
            return SetRatingAsync(rating, track.ID);
        }

        /// <summary>Subscribes to the provided event.</summary>
        /// <param name="listener">Name of the object which receives a notification when the event occurs.</param>
        /// <param name="eventType">Name of the event type to listen for.</param>
        /// <param name="callback">Action to execute once the event is fired.</param>
        public async Task Subscribe(string listener, string eventType, Action<ConsoleAPICalledEvent> callback)
        {
            await mmSession.Runtime.Enable(new EnableCommand()).ConfigureAwait(false);
            await SendCommandAsync($"app.listen({listener},'{eventType}',console.debug)").ConfigureAwait(false);

            mmSession.Runtime.SubscribeToConsoleAPICalledEvent(callback);
        }

        /// <summary>Unsubscribes to the provided event.</summary>
        /// <param name="listener">Name of the object which receives a notification when the event occurs.</param>
        /// <param name="eventType">Name of the event type to listen for.</param>
        public Task UnsubScribe(string listener, string eventType)
        {
            return SendCommandAsync($"app.unlisten({listener},'{eventType}',console.debug)");
        }

        /// <summary>Enables event based updates for the player state and currently playing track.</summary>
        public async Task EnableUpdates()
        {
            await mmSession.Runtime.Enable(new EnableCommand()).ConfigureAwait(false);

            // Disable previous listeners to prevent getting duplicate notifications
            await DisableUpdates().ConfigureAwait(false);

            await SendCommandAsync("var mmNetRepeatListen=e=>console.debug('repeat:'+e);" +
                "var mmNetShuffleListen=e=>console.debug('shuffle:'+e);" +
                "var mmNetStateListen=e=>{switch(e){" +
                "case 'trackChanged':console.debug('trackChanged:');break;" +
                "case 'volumeChanged':console.debug('volume:'+app.player.volume);break;" +
                "default:console.debug('state:'+e)}};" +
                "app.listen(app.player,'repeatchange',mmNetRepeatListen);" +
                "app.listen(app.player,'shufflechange',mmNetShuffleListen);" +
                "app.listen(app.player,'playbackState',mmNetStateListen);").ConfigureAwait(false);

            mmSession.Runtime.SubscribeToConsoleAPICalledEvent(OnPlayerStateChanged);
            Player.EnableUpdates(mmSession);
        }

        /// <summary>Disables event based updates for the player state and currently playing track.</seummary>
        public Task DisableUpdates()
        {
            //mmSession.Runtime.Disable(new Disable()).ConfigureAwait(false);
            mmSession.UnSubscribe<ConsoleAPICalledEvent>(OnPlayerStateChanged);
            Player.DisableUpdates(mmSession);

            return SendCommandAsync("if(typeof mmNetRepeatListen==='function'){app.unlisten(app.player,'repeatchange',mmNetRepeatListen)};" +
                "if(typeof mmNetShuffleListen==='function'){app.unlisten(app.player,'shufflechange',mmNetShuffleListen)};" +
                "if(typeof mmNetStateListen==='function'){app.unlisten(app.player,'playbackState',mmNetStateListen)};");
        }

        private void OnPlayerStateChanged(ConsoleAPICalledEvent e)
        {

            if (e.Type != "debug") return;

            string[] eventInfo = e.Args.FirstOrDefault().Value.ToString().Split(':');
            switch (eventInfo[0])
            {
                //case "state":
                //    Player.SetPlayerState(eventInfo[1]);
                //    break;

                case "trackChanged":
                    RefreshCurrentTrackAsync().GetAwaiter();
                    break;
            }
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
                    mmSession = null;
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
