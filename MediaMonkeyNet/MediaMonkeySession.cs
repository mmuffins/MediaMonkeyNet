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
    /// <summary>
    /// A MediaMonkey remote session.</summary>  
    public class MediaMonkeySession : IDisposable
    {
        private const int defaultConnectionPort = 9222;
        private const string defaultConnectionAddress = "127.0.0.1";
        private const string mmWebsocketUrl = "file:///mainwindow.html";
        private const int mmSessionTimeout = 1000;

        private bool playerUpdateInProgress;
        private ChromeSession mmSession;


        public string EndpointAddress { get; private set; }
        public string RemoteDebuggingUri { get; }
        public Player Player { get; }

        /// <summary>Initializes a new instance of the MediaMonkeySession class.</summary>  
        public MediaMonkeySession() : this(defaultConnectionAddress, defaultConnectionPort) { }

        /// <summary>Initializes a new instance of the MediaMonkeySession class for a specific host.</summary>  
        /// <param name="ConnectionAddress">The IP address or hostname to connect to.</param>
        /// <param name="ConnectionPort">The port number to connect to.</param>
        public MediaMonkeySession(string ConnectionAddress, int ConnectionPort)
        {
            RemoteDebuggingUri = $"http://{ConnectionAddress}:{ConnectionPort.ToString()}";
            Player = new Player();
        }

        /// <summary>Opens a session to the chromium instance hosting MediaMonkey.</summary>  
        public async Task OpenSessionAsync()
        {
            using (var webClient = new HttpClient())
            {
                webClient.BaseAddress = new Uri(RemoteDebuggingUri);
                var remoteSessions = await webClient.GetStringAsync("/json");
                var webSockets =  JsonConvert.DeserializeObject<ICollection<ChromeSessionInfo>>(remoteSessions);
                EndpointAddress = (webSockets.First(s => s.Url == mmWebsocketUrl)).WebSocketDebuggerUrl.Replace("ws://localhost", "ws://127.0.0.1");
                mmSession = new ChromeSession(EndpointAddress);
                mmSession.CommandTimeout = mmSessionTimeout;
            }
        }


        /// <summary>
        /// Sends the specified command to the currently active mm session and returns the associated command response.
        /// </summary>
        private async Task<EvaluateCommandResponse> SendCommandAsync(string command)
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

            return await mmSession.SendCommand(cmd) as EvaluateCommandResponse;
        }

        /// <summary>
        /// Refreshes state information for the current mediamonkey session.
        /// </summary>
        public async Task RefreshPlayerAsync()
        {
            if (playerUpdateInProgress) { return; }

            playerUpdateInProgress = true;
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

            var result = (await SendCommandAsync(cmd)).Result;



            playerUpdateInProgress = false;
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
