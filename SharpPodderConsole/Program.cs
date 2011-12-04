using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpPodder;
using SharpPodder.FeedReading;
using System.IO;

namespace SharpPodderConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //var newSubscription = SerializableSuscription.New("GorroDelMundo2.json", "Gorro2", new Uri("http://feeds2.feedburner.com/gorrodelmundo"));
            //newSubscription.FeedReader = new ServiceModelSyndicationFeedReader();
            //newSubscription.Save();

            var subscriptions = args.Any() 
                ? SerializableSuscription.OpenFromFolder(args)
                : SerializableSuscription.OpenFromFolder("Subscriptions").Union(SerializableSuscription.OpenFromUserFolder());

            foreach (var subscription in subscriptions)
            {
                Console.WriteLine();
                Console.WriteLine(subscription.Name);
                try
                {
                    subscription.RefreshItems();
                    var toDownload = subscription.Links.Where(x =>
                        x.Downloadable
                        && !x.IgnoreIt
                        && !x.IsDownloaded
                        && !x.HasFailed
                        && !x.Deleted);
                    foreach (var link in toDownload)
                    {
                        Console.WriteLine(link.Title);
                        Console.WriteLine(link.Uri);
                        Console.WriteLine();
                        link.Download();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Console.WriteLine();
                }
            }
        }
    }
}

