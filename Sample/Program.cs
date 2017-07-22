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
            var mediaMonkey = new MediaMonkeyNet.MediaMonkeyNet("http://localhost:9222");

            // Console.WriteLine(mediaMonkey.GetCurrentTrack().Title);



            // Ideally, only one process should use the same socket, otherwise debugging wouldn't work
            // anyway. Nevertheless, if there is an issue with socket binding run the following command
            // to identify the process that's blocking the port
            // Get-Process -Id ((Get-NetTCPConnection -LocalPort 9222 -State listen).owningprocess) | select Name, Description, Company, Path

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

            Console.WriteLine("Available debugging sessions");
            foreach (var s in sessions)
            {
                Console.WriteLine(s.url);
            }

            if (sessions.Count == 0)
            {
                Console.WriteLine("No debugging sessions are available");
                Console.ReadLine();
                return;
            }

            // Will use the first available session
            var endpointUrl = sessions.FirstOrDefault().webSocketDebuggerUrl;

            mediaMonkey.SetActiveSession(endpointUrl);

            var currentTrack = mediaMonkey.GetCurrentTrack();
            Console.WriteLine(currentTrack.Title);
            // Console.WriteLine(mediaMonkey.GetDefaultAction());

            // mediaMonkey.Play();

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
    }
}
