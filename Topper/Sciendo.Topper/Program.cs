using System;
using Sciendo.Topper.Source;
using Sciendo.Last.Fm;
using Sciendo.Topper.Store;

namespace Sciendo.Topper
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args == null || args.Length < 1)
            {
                Console.WriteLine("Give the output File.");
                Console.ReadKey();
                return;
            }
            var apiKey = "67b6145c521d4ca0e31ef35c3032d320";
            var lastFmTopArtistSourcer = new LastFmTopArtistSourcer(new ContentProvider<RootObject>( new UrlProvider(apiKey),new LastFmProvider()));
            var topItems = lastFmTopArtistSourcer.GetTopItems("scentmaster");
            foreach (var topItem in topItems)
            {
                Console.WriteLine("{0} has {1} Hits as per:{2}.", topItem.Name, topItem.Hits, topItem.Date);

            }
            JsonStore jsonStore = new JsonStore();
            jsonStore.Save(args[0], topItems);
            Console.ReadKey();
        }
    }
}
