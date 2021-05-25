using BaristaLabs.ChromeDevTools.Runtime.Runtime;
using MediaMonkeyNet;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Sample
{
    class Program
    {
        private static Process MMProcess;
        private static Process[] MMengineProcessAr;

        static async Task Main(string[] args)
        {
            // Initialize the object with default uri htt://localhost:9222.
            using (MediaMonkeySession mm = new MediaMonkeySession())
            {
                try
                {
                    // Establish a session to the chromium instance running MediaMonkey.

                    while (true)
                    {
                        try
                        {
                            if (IsMMRunning())
                            {
                                await mm.OpenSessionAsync();
                                await mm.WindowReady();
                                //await mm.RefreshCurrentTrackAsync();
                                //await mm.Player.RefreshAsync();
                            }

                            System.Threading.Thread.Sleep(5);
                        }
                        catch (Exception ex)
                        {

                            Console.WriteLine("Connection error: " + ex.Message);

                        }
                    }
                }
                catch (System.Net.Http.HttpRequestException ex)
                {
                    Console.WriteLine("Connection error: " + ex.Message);
                    Console.ReadLine();
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Unknown error: " + ex.Message);
                    Console.ReadLine();
                    return;
                }
            }
        }

        public static void GetMMProcess()
        {
            Process[] mmEngineProcessAr = Process.GetProcessesByName("MediaMonkeyEngine");
            if (mmEngineProcessAr.Length < 2) { return; }

            Process[] mmProcessAr = Process.GetProcessesByName("MediaMonkey");
            if (mmProcessAr.Length == 0) { return; }

            MMProcess = mmProcessAr[0];
            MMengineProcessAr = mmEngineProcessAr;
        }

        public static bool IsMMRunning()
        {
            // Search for processes if not yet found or if they exited
            if (MMProcess == null || MMengineProcessAr == null || MMProcess.HasExited == true || MMengineProcessAr.Any(proc => proc.HasExited == true))
            {
                MMProcess = null;
                MMengineProcessAr = null;
                GetMMProcess();
            }

            // Check if both the main process and the engine processes were found
            if (MMProcess == null || MMengineProcessAr == null || MMengineProcessAr.Length < 2) { return false; }

            // Immediately after start, MM needs a couple of moments to initialize.
            // Sending commands to MM before it is ready can crash the application.
            // There is currently no (known) way to actually check for a ready state, so we wait for a 
            // few moments after mm was started to give a green light
            return (DateTime.Now.Subtract(MMengineProcessAr[1].StartTime).TotalMilliseconds >= 0);
        }
    }
}
