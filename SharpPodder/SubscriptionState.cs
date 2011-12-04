using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using SharpPodder.FileDownloaders;
using SharpPodder.FeedReading;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;
using SharpPodder.FeedMerging;

namespace SharpPodder
{
    [JsonObject(MemberSerialization.OptIn)]
    public class SubscriptionState
    {
        [JsonProperty]
        public DateTimeOffset? ItemsRefreshedAt { get; private set; }

        private List<SubscriptionItem> items = new List<SubscriptionItem>();
        
        [JsonProperty(ObjectCreationHandling=ObjectCreationHandling.Replace)]
        public List<SubscriptionItem> Items 
        {
            get { return items; }
            private set 
            {
                foreach (var item in value)
                {
                    item.Subscription = subscription;
                    foreach (var link in item.Links)
                        link.SubscriptionItem = item;
                }
                items = value;
            }
        }

        private Subscription subscription;

        public SubscriptionState(Subscription subscription)
        {
            this.subscription = subscription;
        }

        public void OverrideItems(IEnumerable<SubscriptionItem> items)
        {
            Items = items.ToList();
            ItemsRefreshedAt = DateTimeOffset.Now;
        }
    }
}