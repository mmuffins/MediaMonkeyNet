using BaristaLabs.ChromeDevTools.Runtime.Runtime;
using MediaMonkeyNet;
using System;
using System.Threading.Tasks;

namespace Sample
{
    class Program
    {
        static async Task Main(string[] args)
        {
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
                        await mm.Player.RefreshAsync();

                        Console.WriteLine("Current Track:");
                        Console.WriteLine("Title:" + mm.CurrentTrack.Title);
                        Console.WriteLine("Artist:" + mm.CurrentTrack.Artist);
                        Console.WriteLine("Rating:" + mm.CurrentTrack.Rating);
                        Console.WriteLine("Position:" + TimeSpan.FromMilliseconds(mm.Player.TrackPosition));
                        Console.WriteLine("Progress:" + mm.Player.Progress.ToString("P0"));
                        Console.WriteLine("Player state:" + mm.Player.State);

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
