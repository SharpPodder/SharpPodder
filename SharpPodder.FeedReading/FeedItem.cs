using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SharpPodder.FeedReading
{
    [DebuggerDisplay("Id={Id}, PublishDate={PublishDate}")]
    public class FeedItem
    {
        public string Id { get; set; }
        public DateTimeOffset PublishDate { get; set; }
        public IEnumerable<FeedItemLink> Links { get; set; }
        public DateTimeOffset? LastUpdatedTime { get; set; }
        public string Summary { get; set; }
        public string Title { get; set; }
        public HashSet<string> Categories { get; set; }
    }
}