using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using SharpPodder.FeedMerging;

namespace SharpPodder
{
    [JsonObject(MemberSerialization.OptIn)]
    public class StandardLimitOnNewSubscriptions : ISubscriptionFilter
    {
        [JsonProperty]
        public int MaxInitialDownloads { get; set; }

        [JsonProperty]
        public TimeSpan MaxOldInitialDownloads { get; set; }

        public StandardLimitOnNewSubscriptions()
        {
            MaxInitialDownloads = 5;
            MaxOldInitialDownloads = TimeSpan.FromDays(366);
        }

        public void Filter(MergeResult mergeResult)
        {
            var newestToOlder = mergeResult.Reverse().ToArray();
            if (mergeResult.ThereWereNoItems)
            {
                var ignore = false;
                var count = 0;
                foreach (var item in newestToOlder)
                {
                    ignore = ignore || count == MaxInitialDownloads || (DateTimeOffset.Now - item.PublishDate > MaxOldInitialDownloads);
                    if (ignore)
                        foreach (var link in item.Links.Where(x => x.Downloadable))
                            link.IgnoreIt = true;
                    else if (item.Links.Any(x => x.Downloadable && !x.IgnoreIt && !x.Deleted))
                        count++;
                }
            }
        }
    }
}
