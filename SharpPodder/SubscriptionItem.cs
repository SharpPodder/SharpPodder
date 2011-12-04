using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Diagnostics;
using SharpPodder.FeedReading;
using SharpPodder.Utilities;

namespace SharpPodder
{
    [JsonObject(MemberSerialization.OptIn)]
    [DebuggerDisplay("Id={Id}, PublishDate={PublishDate}")]
    public class SubscriptionItem
    {
        public Subscription Subscription { get; internal set; }

        [JsonProperty]
        public string Id { get; set; }

        [JsonProperty]
        public DateTimeOffset PublishDate { get; set; }

        [JsonProperty]
        public DateTimeOffset? LastUpdatedTime { get; set; }

        [JsonProperty]
        public string Summary { get; set; }

        [JsonProperty]
        public string Title { get; set; }

        [JsonProperty]
        public HashSet<string> Categories { get; set; }

        [JsonProperty(Order = 1000)]
        public List<SubscriptionItemLink> Links { get; set; }

        public SubscriptionItem(Subscription subscription)
        {
            Subscription = subscription;
        }

        [JsonConstructor]
        private SubscriptionItem()
        {
        }

        public SubscriptionItem(FeedItem item)
        {
            Id = item.Id;
            PublishDate = item.PublishDate;
            LastUpdatedTime = item.LastUpdatedTime;
            Summary = item.Summary;
            Title = item.Title;
            Categories = item.Categories;
            Links = item.Links.Select(y => CreateLink(y)).ToList();
        }

        public void Update(FeedItem item)
        {
            LastUpdatedTime = item.LastUpdatedTime;
            Summary = item.Summary;
            Title = item.Title;
            Categories = item.Categories;
            var previousLinksAux = new KeyedCollection<Uri, SubscriptionItemLink>(x => x.Uri, Links);
            foreach (var currentLink in item.Links)
            {
                if (previousLinksAux.Contains(currentLink.Uri))
                {
                    var previousLink = previousLinksAux[currentLink.Uri];
                    if (previousLink.IsDownloaded && previousLink.Length != currentLink.Length)
                    {
                        previousLink.Length = currentLink.Length;
                        previousLink.MarkAsNotDownloaded();
                    }
                    previousLink.MediaType = currentLink.MediaType;
                    previousLink.RelationshipType = currentLink.RelationshipType;
                    previousLink.Title = currentLink.Title;
                    previousLinksAux.Remove(currentLink.Uri);
                }
                else
                {
                    var newLink = CreateLink(currentLink);
                    Links.Add(newLink);
                }
            }
            foreach (var previousLink in previousLinksAux)
                previousLink.Deleted = true;
        }

        private SubscriptionItemLink CreateLink(FeedItemLink currentLink)
        {
            var newLink = new SubscriptionItemLink(this)
            {
                Length = currentLink.Length,
                MediaType = currentLink.MediaType,
                RelationshipType = currentLink.RelationshipType,
                Title = currentLink.Title,
                Uri = currentLink.Uri
            };
            return newLink;
        }
    }
}