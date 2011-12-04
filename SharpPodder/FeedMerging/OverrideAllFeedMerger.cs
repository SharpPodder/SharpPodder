using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpPodder.FeedReading;

namespace SharpPodder.FeedMerging
{
    public class OverrideAllFeedMerger : IFeedMerger
    {
        protected Subscription Subscription { get; private set; }

        public OverrideAllFeedMerger(Subscription subscription)
        {
            Subscription = subscription;
        }

        public MergeResult Merge(IEnumerable<SubscriptionItem> oldItems, IEnumerable<FeedItem> newItems)
        {
            var items = newItems.Select(x =>
            {
                var item = new SubscriptionItem(Subscription)
                {
                    Id = x.Id,
                    PublishDate = x.PublishDate,
                    LastUpdatedTime = x.LastUpdatedTime,
                    Summary = x.Summary,
                    Title = x.Title,
                    Categories = x.Categories
                };
                item.Links = x.Links.Select(y => new SubscriptionItemLink(item)
                {
                    Length = y.Length,
                    MediaType = y.MediaType,
                    RelationshipType = y.RelationshipType,
                    Title = y.Title,
                    Uri = y.Uri
                }).ToList();
                return item;
            });
            var result = new MergeResult();
            foreach (var item in items)
                result.AddItem(item, ItemMergeStatus.NewItem);
            return result;
        }
    }
}