﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaMonkeyNet;
using System.Timers;

namespace Sample
{
    class Program
    {

        static void Main(string[] args)
        {
            // Initialize the remote session for MediaMonkey
            //testPerf();
            //Console.ReadLine();
            //return;

            using (var mediaMonkey = new MediaMonkeyNet.MediaMonkeyNet("http://localhost:9222", false))
            {
                // It's also possible to just use the default constructor without any parameters, which will
                // init the remote session with http://localhost:9222 and use the default session
                // Ideally, only one process should use the same socket, otherwise debugging wouldn't work
                // anyway. Nevertheless, if there is an issue with socket binding run the following command
                // to identify the process that's blocking the port try
                // Get-Process -Id ((Get-NetTCPConnection -LocalPort 9222 -State listen).owningprocess) | select Name, Description, Company, Path


                // Enumerate and select one of the available sessions

                List<RemoteSessionsResponse> sessions;

                try
                {
                    sessions = mediaMonkey.GetAvailableSessions();
                }
                catch (Exception)
                {
                    Console.WriteLine("Could not get available sessions");
                    Console.ReadLine();
                    return;
                }

                if (sessions.Count == 0)
                {
                    Console.WriteLine("No debugging sessions are available");
                    Console.ReadLine();
                    return;
                }

                Console.WriteLine("Available debugging sessions:");
                foreach (var s in sessions)
                {
                    Console.WriteLine(s.url);
                }

                // Use the first available session
                var endpointUrl = sessions.FirstOrDefault().webSocketDebuggerUrl;
                mediaMonkey.SetActiveSession(endpointUrl);

                // Last check if selecting a session was successful
                if (!mediaMonkey.HasActiveSession())
                {
                    Console.WriteLine("Could not get debugging session");
                    Console.ReadLine();
                    return;
                }

                // The connection to MM should be established now, 
                // so we can start issuing commands

                try
                {
                    // The application hosting our session could
                    // be closed any time so we are issuing
                    // the commands in a try block

                    // The player object contains properties related to the
                    // current status of the player


                    var player = new MediaMonkeyNet.Player(mediaMonkey);
                    player.Refresh().Wait();

                    Console.WriteLine("Player volume: " + player.Volume);
                    Console.WriteLine("Shuffle is active: " + player.IsShuffle);
                    Console.WriteLine("Track Position in MS: " + player.TrackPosition);

                    // GetCurrentTrack returns properties of the currently playing track
                    Track currentTrack = mediaMonkey.GetCurrentTrack();
                    if (currentTrack == null)
                    {
                        Console.WriteLine("No Track is currently playing");
                    }
                    else
                    {
                        Console.WriteLine("Current Track:");
                        Console.WriteLine("Title:" + currentTrack.Title);
                        Console.WriteLine("Artist:" + currentTrack.ArtistName);
                        Console.WriteLine("Rating:" + currentTrack.Rating);

                        // Start playback if a song was selected
                        player.StartPlayback();

                        var response = mediaMonkey.SetRating(80,currentTrack);
                        if (response.Exception != null)
                        {
                            // All commands return an object containing an exception property
                            // which can be checked to verify if an action was successful
                            Console.WriteLine("An error occurred while attempting to update the rating of the currently playing track:");
                            Console.WriteLine(response.Exception);
                        }
                    }

                    // Generic javascript code can be executed by using the evaluate method
                    var isRepeat = mediaMonkey.Evaluate<bool>("app.player.repeatPlaylist");
                    if (isRepeat.Exception != null)
                    {
                        Console.WriteLine("Repeat is active: " + isRepeat);
                    }

                    // The example above only works if mediamonkey runs the function
                    // synchronously. If it runs async, we need to handle the returned promise

                    // Get ID of the remote object
                    var allSongs = mediaMonkey.Evaluate<object>("function AllSongs(){var list = app.db.getTracklist('SELECT * FROM Songs LIMIT 3', -1);list.whenLoaded();return list;};AllSongs();", false);

                    if(allSongs.Exception != null)
                    {
                        Console.WriteLine("Error while executing query:");
                        Console.WriteLine(allSongs.Exception);
                    }
                    else
                    {
                        // Get properties of the remote object
                        var remoteObj = mediaMonkey.GetObject<object>(allSongs.ObjectId);

                        // GetObject returns an IEnumerable containing all properties of the remote object
                        bool isLoaded = bool.Parse(remoteObj.Value.Where(x => x.Name == "isLoaded").FirstOrDefault().Value.ToString());

                        if (isLoaded)
                        {
                            string tracks = remoteObj.Value.Where(x => x.Name == "asJSON").FirstOrDefault().Value.ToString();
                            Console.WriteLine("Songlist: " + tracks);
                        }
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Unable to communicate with MediaMonkey");
                    return;
                }

                Console.ReadLine();
            }
        }
    }
}
