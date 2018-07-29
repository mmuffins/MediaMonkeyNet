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

            // Initialize the object with default uri htt://localhost:9222
            using (MediaMonkeySession mm = new MediaMonkeySession())
            {

                await mm.OpenSessionAsync().ConfigureAwait(false);
                await mm.Player.RefreshAsync().ConfigureAwait(false);
            }
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
