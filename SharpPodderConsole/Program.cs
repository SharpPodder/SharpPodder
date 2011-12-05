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
		static readonly TimeSpan blockFor = TimeSpan.FromMinutes(30);
        static void Main(string[] args)
        {
            //var newSubscription = SerializableSuscription.New("GorroDelMundo2.json", "Gorro2", new Uri("http://feeds2.feedburner.com/gorrodelmundo"));
            //newSubscription.FeedReader = new ServiceModelSyndicationFeedReader();
            //newSubscription.Save();

            var subscriptions = args.Any() 
                ? SerializableSuscription.OpenFromFolder(blockFor, args)
                : SerializableSuscription.OpenFromFolder(blockFor, "Subscriptions").Union(SerializableSuscription.OpenFromUserFolder(blockFor));

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
                        && !x.Deleted)
						.ToArray();
                    foreach (var link in toDownload)
                    {
						Console.WriteLine("{0} - {1}", link.SubscriptionItem.Title, link.Title);
                        Console.WriteLine(link.Uri);
                        link.Download();
						Console.WriteLine(link.DownloadedFileUri);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Console.WriteLine();
                }
				finally 
				{
					subscription.Dispose();
				}
            }
        }
    }
}

