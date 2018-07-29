using MediaMonkeyNet;
using System;
using System.Threading.Tasks;

namespace Sample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Initialize the object with default uri htt://localhost:9222
            using (MediaMonkeySession mm = new MediaMonkeySession())
            {
                // Open the session
                await mm.OpenSessionAsync();

                await mm.RefreshPlayerAsync();
            }
        }
    }
}
