using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using SharpPodder.FeedMerging;

namespace SharpPodder
{
    [JsonObject(MemberSerialization.OptIn)]
    public class StandardPodcastContentFilter : ISubscriptionFilter
    {
        private readonly RelationshipAndMediaTypeFilter BaseFilter = new RelationshipAndMediaTypeFilter("audio/mpeg", "enclosure").Add("audio/mp3", "enclosure");
        
        [JsonProperty]
        public ItemMergeStatus[] ApplyTo
        {
            get { return BaseFilter.ApplyTo; }
            set { BaseFilter.ApplyTo = value; }
        }

        public void Filter(MergeResult mergeResult)
        {
            BaseFilter.Filter(mergeResult);
        }
    }
}