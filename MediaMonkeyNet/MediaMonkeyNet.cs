
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MasterDevs.ChromeDevTools;
using MasterDevs.ChromeDevTools.Protocol.Chrome.Runtime;
using Newtonsoft.Json.Linq;

namespace MediaMonkeyNet
{
    public class MediaMonkeyNet
    {
        const string JsonPostfix = "/json";
        const string DefaultRemoteDebuggingUri = "http://localhost:9222";

        string RemoteDebuggingUri;
        string SessionWSEndpoint;
        private ChromeSession ws;

        public bool IsMuted
        {
            get
            {
                try
                {
                    var response = this.Evaluate("app.player.mute");
                    if (response.Exception != null)
                    {
                        return false;
                    }

                    return (bool)response.Value;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public bool IsPaused
        {
            get
            {
                try
                {
                    var response = this.Evaluate("app.player.paused");
                    if (response.Exception != null)
                    {
                        return false;
                    }

                    return (bool)response.Value;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public bool IsPlaying
        {
            get
            {
                try
                {
                    var response = this.Evaluate("app.player.isPlaying");
                    if (response.Exception != null)
                    {
                        return false;
                    }

                    return (bool)response.Value;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public bool IsRepeat
        {
            get
            {
                try
                {
                    var response = this.Evaluate("app.player.repeatPlaylist");
                    if (response.Exception != null)
                    {
                        return false;
                    }

                    return (bool)response.Value;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public bool IsShuffle
        {
            get
            {
                try
                {
                    var response = this.Evaluate("app.player.shufflePlaylist");
                    if (response.Exception != null)
                    {
                        return false;
                    }

                    return (bool)response.Value;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public int TrackLength
        {
            get
            {
                try
                {
                    var response = this.Evaluate("app.player.trackLengthMS");
                    if (response.Exception != null)
                    {
                        return 0;
                    }

                    return int.Parse(response.Value.ToString());
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        public int TrackPosition
        {
            get
            {
                try
                {
                    var response = this.Evaluate("app.player.trackPositionMS");
                    if (response.Exception != null)
                    {
                        return 0;
                    }

                    return int.Parse(response.Value.ToString());
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        public double Volume
        {
            get
            {
                try
                {
                    var response = this.Evaluate("app.player.volume");
                    if (response.Exception != null)
                    {
                        return 0;
                    }

                    return (double)response.Value;
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        public MediaMonkeyNet() : this(DefaultRemoteDebuggingUri, true) { }

        public MediaMonkeyNet(string remoteDebuggingUri) : this(remoteDebuggingUri, true) { }

        public MediaMonkeyNet(string remoteDebuggingUri, bool useDefaultSession)
        {
            this.RemoteDebuggingUri = remoteDebuggingUri;

            if (useDefaultSession)
            {
                List<RemoteSessionsResponse> sessions = this.GetAvailableSessions();

                // Use the first available session
                this.SetActiveSession(sessions.FirstOrDefault().webSocketDebuggerUrl);
            }
        }

        private TRes SendRequest<TRes>()
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(RemoteDebuggingUri + JsonPostfix);
            WebResponse resp = req.GetResponse();
            Stream respStream = resp.GetResponseStream();

            StreamReader sr = new StreamReader(respStream);
            string s = sr.ReadToEnd();
            resp.Dispose();
            return Deserialise<TRes>(s);
        }

        private T Deserialise<T>(string json)
        {
            T obj = Activator.CreateInstance<T>();
            using (System.IO.MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(json)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
                obj = (T)serializer.ReadObject(ms);
                return obj;
            }
        }

        private T Deserialise<T>(Stream json)
        {
            T obj = Activator.CreateInstance<T>();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            obj = (T)serializer.ReadObject(json);
            return obj;
        }


        public List<RemoteSessionsResponse> GetAvailableSessions()
        {
            /// <summary>
            /// Get a list of all available remote sessions for the current instance
            /// </summary>

            var res = this.SendRequest<List<RemoteSessionsResponse>>();
            return (from r in res
                    where r.devtoolsFrontendUrl != null
                    select r).ToList();
        }

        public void SetActiveSession(string sessionWSEndpoint)
        {
            /// <summary>
            /// Set the active remote session for the current instance
            /// </summary>
            // Sometimes binding to localhost might resolve wrong AddressFamily, force IPv4
            this.SessionWSEndpoint = sessionWSEndpoint.Replace("ws://localhost", "ws://127.0.0.1");
            var chromeSessionFactory = new ChromeSessionFactory();
            ws = chromeSessionFactory.Create(this.SessionWSEndpoint) as ChromeSession;
        }

        public bool HasActiveSession()
        {
            if (ws != null)
            {
                try
                {
                    ws.GetType();
                    return true;
                }
                catch (NullReferenceException)
                {
                    return false;
                }
            }
            return false;
        }

        public EvaluateResponse Evaluate(string command)
        {
            /// <summary>
            /// Generic method to send a command to mediamonkey
            /// </summary>

            if (!this.HasActiveSession())
            {
                // No active chrome session
                throw new NullReferenceException("No active session found");
            }

            try
            {
                EvaluateCommand cmd = CommandFactory.Create(command);

                // var result = ws.SendAsync(cmd).Result;
                // var cmdResponse = vs as CommandResponse<EvaluateCommandResponse>;
                // var cmdResponseResult = ab.Result;
                // var evalResponse = new EvaluateResponse(res);
                // var parsedObject = xx.Value as JObject;
                return new EvaluateResponse((ws.SendAsync(cmd).Result as CommandResponse<EvaluateCommandResponse>).Result);
            }
            catch (NullReferenceException)
            {
                //Session is not available anymore, update the local session accordingly
                this.ws = null;
                throw;
            }

        }

        public EvaluateResponse GetPlayingStatus()
        {
            /// <summary>
            /// Starts Playback
            /// </summary>
            return this.Evaluate("app.player.playAsync()");
        }

        public Track GetCurrentTrack()
        {

            /// <summary>
            /// Returns a track object for the current track
            /// </summary>

            EvaluateResponse currentTrack = this.Evaluate("app.player.getCurrentTrack()");

            if(currentTrack.Exception != null || currentTrack.Value == null)
            {
                return null;
            }
            else
            {
                // currentTrack.Value = new Track(currentTrack.Value as JObject);
                var result = currentTrack.Value as JObject;
                return new Track(result);
            }

        }

        public EvaluateResponse NextTrack()
        {
            /// <summary>
            /// Plays the next file in the current playlist
            /// </summary>
            return this.Evaluate("app.player.nextAsync()");
        }

        public EvaluateResponse PausePlayback()
        {
            /// <summary>
            /// Pauses Playback
            /// </summary>
            return this.Evaluate("app.player.pauseAsync()");
        }

        public EvaluateResponse PreviousTrack()
        {
            /// <summary>
            /// Plays the previous file in the current playlist
            /// </summary>
            return this.Evaluate("app.player.prevAsync()");
        }

        public EvaluateResponse SetMute(bool enabled)
        {
            /// <summary>
            /// Sets mute status
            /// </summary>
            return this.Evaluate("app.player.mute = " + enabled.ToString().ToLower());
        }

        public EvaluateResponse SetRating(int rating) {
            /// <summary>
            /// Sets Rating of the currently playing track
            /// </summary>
            /// <param name="rating">Rating of the track from 0 to 100</param>

            var currentTrack = this.GetCurrentTrack();

            return this.SetRating(rating, currentTrack.SongID);
        }

        public EvaluateResponse SetRating(int rating, Track track)
        {
            /// <summary>
            /// Set Rating of the specified track
            /// </summary>
            /// <param name="rating">Rating of the track from 0 to 100</param>
            /// <param name="track">Track object of the track</param>

            return this.SetRating(rating, track.SongID);
        }

        public EvaluateResponse SetRating(int rating, int ID)
        {
            /// <summary>
            /// Set Rating of the specified track
            /// </summary>
            /// <param name="rating">Rating of the track from 0 to 100</param>
            /// <param name="rating">SongID property of the track</param>

            string evalString = "app.getObject('track', { id:" + ID 
                + "}).then(function(track){ if (track) {track.rating =" 
                + rating + "; track.commitAsync();}});";

            var result = this.Evaluate(evalString);
            return result;
        }

        public EvaluateResponse SetRepeat(bool enabled)
        {
            /// <summary>
            /// Sets repeat status
            /// </summary>
            return this.Evaluate("app.player.repeatPlaylist = " + enabled.ToString().ToLower());
        }

        public EvaluateResponse SetShuffle(bool enabled)
        {
            /// <summary>
            /// Sets shuffle status
            /// </summary>
            return this.Evaluate("app.player.shufflePlaylist = " + enabled.ToString().ToLower());
        }

        public EvaluateResponse SetTrackPosition(int position)
        {
            /// <summary>
            /// Seek to the provided track time, in ms
            /// </summary>

            return this.Evaluate("app.player.seekMSAsync(" + position + ")");
        }

        public EvaluateResponse SetVolume(double volume)
        {
            /// <summary>
            /// Sets the current value between 0 and 1.
            /// </summary>

            // Values outside 0 and 1 are automatically converted to 0/1 by mediamonkey
            var nfi = new System.Globalization.NumberFormatInfo() {
                NumberDecimalSeparator = "."
            };

            return this.Evaluate("app.player.volume = " + volume.ToString(nfi));
        }

        public EvaluateResponse StartPlayback()
        {
            /// <summary>
            /// Starts Playback
            /// </summary>
            return this.Evaluate("app.player.playAsync()");
        }

        public EvaluateResponse StopPlayback()
        {
            /// <summary>
            /// Stops playback
            /// </summary>
            return this.Evaluate("app.player.stopAsync()");
        }

        public EvaluateResponse TogglePlayback()
        {
            /// <summary>
            /// Toggles play and pause status
            /// </summary>
            return this.Evaluate("app.player.playPauseAsync()");
        }

    }
}