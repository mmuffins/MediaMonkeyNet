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
    public class MediaMonkeyNet
    {
        const string JsonPostfix = "/json";
        const string DefaultRemoteDebuggingUri = "http://localhost:9222";

        string RemoteDebuggingUri;
        string SessionWSEndpoint;
        private ChromeSession ws;

        public MediaMonkeyNet() : this(DefaultRemoteDebuggingUri, true) { }

        public MediaMonkeyNet(string remoteDebuggingUri) : this(remoteDebuggingUri, true) { }

        public MediaMonkeyNet(string remoteDebuggingUri, bool useDefaultSession)
        {
            this.RemoteDebuggingUri = remoteDebuggingUri;

            if (useDefaultSession)
            {
                List<RemoteSessionsResponse> sessions;

                try
                {
                    sessions = this.GetAvailableSessions();
                    if (sessions.Count == 0)
                    {
                        // Console.WriteLine("No debugging sessions are available");
                        // Console.ReadLine();
                        return;
                    }

                    // Will use the first available session
                    var endpointUrl = sessions.FirstOrDefault().webSocketDebuggerUrl;

                    this.SetActiveSession(endpointUrl);
                }
                catch (Exception)
                {
                    // Console.WriteLine("Could not get available sessions");
                    // Console.ReadLine();
                    return;
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

        public List<RemoteSessionsResponse> GetAvailableSessions()
        {
            var res = this.SendRequest<List<RemoteSessionsResponse>>();
            return (from r in res
                    where r.devtoolsFrontendUrl != null
                    select r).ToList();
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

        public void SetActiveSession(string sessionWSEndpoint)
        {
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
            // Generic method to send a command to mediamonkey

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

        public Track GetCurrentTrack()
        {
            // Returns a track object for the current track
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

        public EvaluateResponse Play()
        {
            /// <summary>
            /// Starts Playback
            /// </summary>
            var result = this.Evaluate("app.player.playAsync()");
            return result;
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
    }
}