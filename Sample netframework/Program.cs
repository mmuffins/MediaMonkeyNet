using BaristaLabs.ChromeDevTools.Runtime.Runtime;
using MediaMonkeyNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample_netframework
{
    class Program
    {
        static async Task Main(string[] args)
        {
            /* Steps to reproduce:
             * - Make sure that MM5 is running before starting the application
             * - Start the application 
             * - Play a track by double clicking it in the main MM window
             * - Verify that details for the currently playing track are written to the console
             * - Change the track by clicking the Next File button in MM
             * - The next execution of RefreshCurrentTrackAsync will trigger an exception
             */

            // Initialize the object with default uri htt://localhost:9222.
            using (MediaMonkeySession mm = new MediaMonkeySession())
            {
                try
                {
                    // Establish a session to the chromium instance running MediaMonkey.
                    await mm.OpenSessionAsync();

                    while (true)
                    {
                        // Refresh data for the currently playing track
                        await mm.RefreshCurrentTrackAsync();

                        Console.WriteLine("Current Track:");
                        Console.WriteLine("Title:" + mm.CurrentTrack.Title);
                        Console.WriteLine("Artist:" + mm.CurrentTrack.Artist);
                        Console.WriteLine("Rating:" + mm.CurrentTrack.Rating);
                        System.Threading.Thread.Sleep(2000);
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
    }
}
