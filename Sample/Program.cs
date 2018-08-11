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
            //await TestPerformance();

            // Initialize the object with default uri htt://localhost:9222.
            using (MediaMonkeySession mm = new MediaMonkeySession())
            {
                try
                {
                    // Establish a session to the chromium instance running MediaMonkey.
                    await mm.OpenSessionAsync().ConfigureAwait(false);

                    // The player object contains properties related to the current status of the player.
                    await mm.Player.RefreshAsync().ConfigureAwait(false);
                    Console.WriteLine("Player volume: " + mm.Player.Volume);
                    Console.WriteLine("Shuffle is active: " + mm.Player.IsShuffle);
                    Console.WriteLine("Track Position in MS: " + mm.Player.TrackPosition);

                    // Update the currently playing track and display some information
                    await mm.RefreshCurrentTrackAsync().ConfigureAwait(false);
                    Console.WriteLine("Current Track:");
                    Console.WriteLine("Title:" + mm.CurrentTrack.Title);
                    Console.WriteLine("Artist:" + mm.CurrentTrack.Artist);
                    Console.WriteLine("Rating:" + mm.CurrentTrack.Rating);

                    // Start playback
                    //mm.Player.StartPlaybackAsync().GetAwaiter();

                    // Update the rating of the currently playing track
                    //mm.SetRatingAsync(80, mm.CurrentTrack).GetAwaiter();

                    // Using SendCommandAsync it's possible to execute generic javascript code
                    EvaluateCommandResponse currentSkin = await mm.SendCommandAsync("app.currentSkin();").ConfigureAwait(false);

                    if (currentSkin.Result != null)
                    {
                        Console.WriteLine("Current Skin: " + currentSkin.Result.Value.ToString());
                    }

                    // Subscribe to an event
                    var action = new Action<ConsoleAPICalledEvent>((ConsoleAPICalledEvent e) => {
                        Console.WriteLine(e.Type + " event fired.");
                    });

                    //await mm.Subscribe("app.player", "shufflechange", action).ConfigureAwait(false);

                    // Enable automatic updates of the currently playing track and player state
                    // Note that track position will not be automatically updated
                    await mm.EnableUpdates().ConfigureAwait(false);

                    while (true)
                    {
                        Console.WriteLine("Current Track:");
                        Console.WriteLine("Title:" + mm.CurrentTrack.Title);
                        Console.WriteLine("Artist:" + mm.CurrentTrack.Artist);
                        Console.WriteLine("Rating:" + mm.CurrentTrack.Rating);
                        Console.WriteLine("Is Playing:" + mm.Player.IsPlaying);
                        System.Threading.Thread.Sleep(4000);
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Unable to communicate with MediaMonkey");
                    return;
                }
            }

            Console.ReadLine();
        }

        static async Task TestPerformance()
        {
            var sw = new System.Diagnostics.Stopwatch();
            var asyncTime = new TimeSpan();
            var optimizedTime = new TimeSpan();


            // Initialize the object with default uri htt://localhost:9222
            using (MediaMonkeySession mm = new MediaMonkeySession())
            {

                await mm.OpenSessionAsync().ConfigureAwait(false);
                await mm.Player.RefreshAsync().ConfigureAwait(false);

                sw.Restart();
                for (int i = 0; i < 500; i++) await mm.Player.TogglePlaybackAsync();
                asyncTime = sw.Elapsed;

                sw.Restart();
                for (int i = 0; i < 500; i++) await mm.Player.TogglePlaybackAsync().ConfigureAwait(false);
                optimizedTime = sw.Elapsed;
            }

            Console.WriteLine("Async: " + asyncTime);
            Console.WriteLine("Optimized: " + optimizedTime);
            Console.ReadLine();
        }
    }
}
