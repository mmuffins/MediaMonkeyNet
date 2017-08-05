using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaMonkeyNet;


namespace Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            // Initialize the remote session for MediaMonkey
            var mediaMonkey = new MediaMonkeyNet.MediaMonkeyNet("http://localhost:9222", false);

            // It's also possible to just use the default constructor without any parameters, which will
            // init the remote session with http://localhost:9222 and the default session
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

                // var asyncString = await mediaMonkey.EvalAsync("app.db.getTracklist('SELECT * FROM Songs', -1)");
                MyAsync(mediaMonkey);

                var response = mediaMonkey.Play();
                if (response.WasThrown)
                {
                    Console.WriteLine("An error occurred while issuing the play command:");
                    Console.WriteLine(response.Exception);
                }

                Track currentTrack = mediaMonkey.GetCurrentTrack();
                if (currentTrack == null)
                {
                    Console.WriteLine("No Track is currently playing");
                }
                else
                {
                    Console.WriteLine("Current Track:");
                    Console.Write("Title:" + currentTrack.Title);
                    Console.Write("Artist:" + currentTrack.ArtistName);
                }

                // We can issue generic commands using the Evaluate function
                // var allSongs = mediaMonkey.Evaluate("var list = uitools.getTracklist(); return list.asJSON");
                var allSongs = mediaMonkey.Evaluate("uitools.getTracklist()");


                var x4 = mediaMonkey.Evaluate("app.db.getTracklist('SELECT * FROM Songs', -1)");

                MyAsync(mediaMonkey);
                MyAsync(mediaMonkey);
                MyAsync(mediaMonkey);

            }
            catch (Exception)
            {
                Console.WriteLine("Unable to communicate with MediaMonkey");
                return;
            }

            return;

            var nowPlaying = mediaMonkey.GetCurrentTrack();
            Console.WriteLine(nowPlaying.Title);
            // Console.WriteLine(mediaMonkey.GetDefaultAction());




            int loopCount = 1;


            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Interval = 200;

            //aTimer.Enabled = true;

            Console.WriteLine("Press \'q\' to quit the sample.");

            //while (Console.Read() != 'q')


            while (Console.Read() != 'q')
            {
                Console.WriteLine(loopCount);
                loopCount++;
                Console.ReadLine();
                mediaMonkey.HasActiveSession();
                var abc = mediaMonkey.GetCurrentTrack();

                // abc.Title = "ee";
                // abc.Artist = "ee";

                // Console.WriteLine(mediaMonkey.GetCurrentTrack().Title);
                // Console.WriteLine(mediaMonkey.GetCurrentTrack().Title);
                // Console.WriteLine(mediaMonkey.GetCurrentTrack().Title);
                // Console.WriteLine(mediaMonkey.GetCurrentTrack().Title);
                // Console.WriteLine(mediaMonkey.GetCurrentTrack().Title);
                Console.WriteLine("=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=");
            }


            // Track currentTrack = mediaMonkey.GetCurrentTrack();
            // Console.WriteLine(currentTrack.Title);


            // mediaMonkey.RunCommand("app.player.stopAsync()");
            // var x2 = mediaMonkey.Evaluate("app.player.getCurrentTrack()");
            // var x1 = mediaMonkey.Evaluate("app.player.getXXSXk()");
            // var x3 = mediaMonkey.Evaluate("app.db.getTracklist('SELECT * FROM Songs', -1)");
            // var x4 = mediaMonkey.Evaluate("app.db.getTracklist('SELECT * FROM SXXongs', -1)");

            Console.ReadLine();
        }


        async static void MyAsync(MediaMonkeyNet.MediaMonkeyNet mm)
        {
            var asyncString = await mm.EvalAsync("app.db.getTracklist('SELECT * FROM Songs', -1)");

            Console.Write(asyncString);

        }
    }
}
