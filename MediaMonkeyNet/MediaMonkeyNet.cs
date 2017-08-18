﻿
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
    public class MediaMonkeyNet : IDisposable
    {
        const string JsonPostfix = "/json";
        const string DefaultRemoteDebuggingUri = "http://localhost:9222";

        string RemoteDebuggingUri;
        string SessionWSEndpoint;
        private ChromeSession ws;
        bool disposed = false;

        //public bool IsMuted
        //{
        //    get
        //    {
        //        try
        //        {
        //            var response = this.Evaluate<bool>("app.player.mute");
        //            if (response.Exception != null)
        //            {
        //                return false;
        //            }

        //            return response.Value;
        //        }
        //        catch (Exception)
        //        {
        //            throw;
        //        }
        //    }
        //}

        //public bool IsPaused
        //{
        //    get
        //    {
        //        try
        //        {
        //            var response = this.Evaluate<bool>("app.player.paused");
        //            if (response.Exception != null)
        //            {
        //                return false;
        //            }

        //            return response.Value;
        //        }
        //        catch (Exception)
        //        {
        //            throw;
        //        }
        //    }
        //}

        //public bool IsPlaying
        //{
        //    get
        //    {
        //        try
        //        {
        //            var response = this.Evaluate<bool>("app.player.isPlaying");
        //            if (response.Exception != null)
        //            {
        //                return false;
        //            }

        //            return response.Value;
        //        }
        //        catch (Exception)
        //        {
        //            throw;
        //        }
        //    }
        //}

        //public bool IsRepeat
        //{
        //    get
        //    {
        //        try
        //        {
        //            var response = this.Evaluate<bool>("app.player.repeatPlaylist");
        //            if (response.Exception != null)
        //            {
        //                return false;
        //            }

        //            return response.Value;
        //        }
        //        catch (Exception)
        //        {
        //            throw;
        //        }
        //    }
        //}

        //public bool IsShuffle
        //{
        //    get
        //    {
        //        try
        //        {
        //            var response = this.Evaluate<bool>("app.player.shufflePlaylist");
        //            if (response.Exception != null)
        //            {
        //                return false;
        //            }

        //            return response.Value;
        //        }
        //        catch (Exception)
        //        {
        //            throw;
        //        }
        //    }
        //}

        //public long TrackLength
        //{
        //    get
        //    {
        //        try
        //        {
        //            var response = this.Evaluate<long>("app.player.trackLengthMS");
        //            if (response.Exception != null)
        //            {
        //                return 0;
        //            }

        //            return response.Value;
        //        }
        //        catch (Exception)
        //        {
        //            throw;
        //        }
        //    }
        //}

        //public long TrackPosition
        //{
        //    get
        //    {
        //        try
        //        {
        //            var response = this.Evaluate<long>("app.player.trackPositionMS");
        //            if (response.Exception != null)
        //            {
        //                return 0;
        //            }

        //            return response.Value;
        //        }
        //        catch (Exception)
        //        {
        //            throw;
        //        }
        //    }
        //}

        //public double Volume
        //{
        //    get
        //    {
        //        try
        //        {
        //            var response = this.Evaluate<object>("app.player.volume");
        //            if (response.Exception != null)
        //            {
        //                return 0;
        //            }

        //            return double.Parse(response.Value.ToString());
        //        }
        //        catch (Exception)
        //        {
        //            throw;
        //        }
        //    }
        //}

        public MediaMonkeyNet() : this(DefaultRemoteDebuggingUri, true) { }

        public MediaMonkeyNet(string remoteDebuggingUri) : this(remoteDebuggingUri, true) { }

        public MediaMonkeyNet(string remoteDebuggingUri, bool useDefaultSession)
        {
            this.RemoteDebuggingUri = remoteDebuggingUri;

            if (useDefaultSession)
            {
                List<RemoteSessionsResponse> sessions = this.GetAvailableSessions();

                // Use the first available session
                if (sessions.Count > 0)
                {
                    string url = sessions.FirstOrDefault().webSocketDebuggerUrl;

                    if (!String.IsNullOrWhiteSpace(url))
                    {
                        this.SetActiveSession(url);
                    }
                }
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
            }

            if (ws != null)
            {
                ws.Dispose();
                ws = null;
            }

            disposed = true;
        }

        ~MediaMonkeyNet()
        {
            Dispose(false);
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
                    Dispose();
                    return false;
                }
            }

            return false;
        }

        public EvaluateResponse<T> Evaluate<T>(string command, bool returnByValue = true)
        {
            /// <summary>
            /// Generic method to send a command to mediamonkey
            /// </summary>

            if (!this.HasActiveSession())
            {
                    // No active chrome session
                    throw new NullReferenceException("No active session found");
            }

            var cmd = new EvaluateCommand
            {
                ObjectGroup = "console",
                IncludeCommandLineAPI = true,
                AwaitPromise = true,
                Silent = false,
                ReturnByValue = returnByValue,
                Expression = command
            };

            try
            {
                var evalResult = ws.SendAsync(cmd).Result as CommandResponse<EvaluateCommandResponse>;

                return new EvaluateResponse<T>(evalResult.Result);
            }
            catch (NullReferenceException)
            {
                //Session is not available anymore, update the local session accordingly
                this.ws = null;
                throw;
            }

        }

        private object EvaluateAsync()
        {
            // needs support for async/await which was
            // introduced in chromium 55
            // mm currently is based on chromium 53

            if (!this.HasActiveSession())
            {
                // No active chrome session
                throw new NullReferenceException("No active session found");
            }

            try
            {

                // Run the initial command
                var cmd = new EvaluateCommand
                {
                    ObjectGroup = "console",
                    IncludeCommandLineAPI = true,
                    ReturnByValue = true,
                    AwaitPromise = true,
                    Silent = false
                };

                // works perfectly, but can cause performance issues
                //cmd.Expression = "function AllSongs(){var list = app.db.getTracklist('SELECT * FROM Songs', -1);while(list.isLoaded =! true){ /* wait until resolved */ }return list;};ReturnPromise();";
                cmd.Expression = "new Promise(function(resolve) { resolve('done') })";
                var cmdResponse = (ws.SendAsync(cmd).Result as CommandResponse<EvaluateCommandResponse>).Result.Result;

                // We don't return the value of the promise since
                // we can't know when it will be finished
                // instead, get a reference to the object
                // and send a new AwaitPromiseCommand
                // which tells chrome to await the result of the object

                var awaitcmd = new AwaitPromiseCommand
                {
                    ReturnByValue = true,
                    PromiseObjectId = cmdResponse.ObjectId
                };

                var awaitPromiseResponse = ws.SendAsync(awaitcmd).Result;
                return awaitPromiseResponse;
            }
            catch (NullReferenceException)
            {
                //Session is not available anymore, update the local session accordingly
                this.ws = null;
                throw;
            }

        }

        public EvaluateResponse<IEnumerable<EvaluateObjectProperty<T>>> GetObject<T>(string remoteObjectId)
        {
            /// <summary>
            /// Get properties of the provided remote object
            /// </summary>

            if (!this.HasActiveSession())
            {
                // No active chrome session
                throw new NullReferenceException("No active session found");
            }

            try
            {

                // Run the initial command
                var cmd = new GetPropertiesCommand
                {
                    ObjectId = remoteObjectId,
                    OwnProperties = true,
                    AccessorPropertiesOnly = false
                };

                //var getPropResult = ;
                // var xx = new EvaluateResponse<IEnumerable<EvaluateObjectProperty>>(getPropResult);
                // var ab = xx.Value.Where(x => x.Name == "isLoaded");

                // return new EvaluateResponse<object>((ws.SendAsync(cmd).Result as CommandResponse<EvaluateCommandResponse>).Result);

                return new EvaluateResponse<IEnumerable<EvaluateObjectProperty<T>>>((ws.SendAsync(cmd).Result as CommandResponse<GetPropertiesCommandResponse>).Result);
            }
            catch (NullReferenceException)
            {
                //Session is not available anymore, update the local session accordingly
                this.ws = null;
                throw;
            }

        }

        public EvaluateResponse<object> GetPlayingStatus()
        {
            /// <summary>
            /// Starts Playback
            /// </summary>
            return this.Evaluate<object>("app.player.playAsync()");
        }

        public List<Cover> GetCoverList(bool forceWait = false)
        {

            /// <summary>
            /// Returns a list of all covers for the current track
            /// </summary>

            // Make sure that the covers are loaded before requesting them by calling loadCoverListAsync()
            var loadPromise = this.Evaluate<object>("var loadPromise = app.player.getCurrentTrack().getCoverList();loadPromise.whenLoaded();loadPromise;", false);

            if (loadPromise.Exception != null || loadPromise.ObjectId == null)
            {
                return null;
            }

            string loadpromiseID = loadPromise.ObjectId;
            var promiseObj = this.GetObject<object>(loadPromise.ObjectId);

            // wait until isLoaded is true
            if (forceWait)
            {
                for (int i = 0; i < 10; i++)
                {
                    // this kind of loop is more hack than anything else,
                    // but until async functions are supported by mediamonkey
                    // this is the best option I have

                    var promiseLoaded = this.GetObject<object>(loadpromiseID);
                    if (!bool.Parse(promiseLoaded.Value.Where(x => x.Name == "isLoaded").FirstOrDefault().Value.ToString()))
                    {
                        i = 20;
                    }
                }
            }

            // Promise is resolved, load the actual covers
            var remoteEvaluation = this.Evaluate<object>("var list=[];var covers = app.player.getCurrentTrack().getCoverList();covers.whenLoaded();covers.forEach(function(itm){list.push(itm)});list;", false);
            var remoteIDs = this.GetObject<object>(remoteEvaluation.ObjectId);
            var idList = remoteIDs.Value.Where(x => x.Description == "Object");

            var coverList = new List<Cover>();

            foreach (var item in idList)
            {
                var cover = new Cover(this.GetObject<object>(item.ObjectId));
                coverList.Add(cover);
            }


            return coverList;
        }

        public Track GetCurrentTrack()
        {

            /// <summary>
            /// Returns a track object for the current track
            /// </summary>

            EvaluateResponse<JObject> currentTrack = this.Evaluate<JObject>("app.player.getCurrentTrack()");

            if (currentTrack.Exception != null || currentTrack.Value == null)
            {
                return null;
            }
            else
            {
                // currentTrack.Value = new Track(currentTrack.Value as JObject);
                var result = currentTrack.Value;
                return new Track(result);
            }
        }

        public EvaluateResponse<object> SetRating(int rating, Track track)
        {
            /// <summary>
            /// Set Rating of the specified track
            /// </summary>
            /// <param name="rating">Rating of the track between 0 and 100</param>
            /// <param name="track">Track object of the track</param>

            return this.SetRating(rating, track.SongID);
        }

        public EvaluateResponse<object> SetRating(int rating, int ID)
        {
            /// <summary>
            /// Set Rating of the specified track
            /// </summary>
            /// <param name="rating">Rating of the track between 0 and 100</param>
            /// <param name="rating">SongID property of the track</param>

            string evalString = "app.getObject('track', { id:" + ID
                + "}).then(function(track){ if (track) {track.rating ="
                + rating + "; track.commitAsync();}});";

            return this.Evaluate<object>(evalString); ;
        }
    }
}