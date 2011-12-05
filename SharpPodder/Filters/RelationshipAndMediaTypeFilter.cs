using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using SharpPodder.FeedMerging;

namespace SharpPodder
{
    [JsonObject(MemberSerialization.OptIn)]
    public class RelationshipAndMediaTypeFilter : ISubscriptionFilter
    {
        [JsonProperty]
        private HashSet<Pair> AllowerPairs { get; set; }

        [JsonProperty]
        public ItemMergeStatus[] ApplyTo { get; set; }

        public RelationshipAndMediaTypeFilter()
        {
            AllowerPairs = new HashSet<RelationshipAndMediaTypeFilter.Pair>();
            ApplyTo = new ItemMergeStatus[] { ItemMergeStatus.NewItem, ItemMergeStatus.UpdatedItem };
        }

        public RelationshipAndMediaTypeFilter Add(string mediaType, string relationshipType)
        {
            AllowerPairs.Add(new Pair() { MediaType = mediaType, RelationshipType = relationshipType });
            return this;
        }

        public RelationshipAndMediaTypeFilter(string mediaType, string relationshipType)
            : this()
        {
            Add(mediaType, relationshipType);
        }
        
        private struct Pair
        {
            public string MediaType { get; set; }
            public string RelationshipType { get; set; }
        }

        public void Filter(MergeResult mergeResult)
        {
            var targetItems = mergeResult.AllItemsBelongsTo(ApplyTo);
            var targetLinks = targetItems.SelectMany(x => x.Links);
            var toDownloadLinks = targetLinks.Where(x => AllowerPairs.Contains(new Pair() { MediaType = x.MediaType, RelationshipType = x.RelationshipType }));
            foreach (var link in toDownloadLinks)
                link.Downloadable = true;
        }
    }
}