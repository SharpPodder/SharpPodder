using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpPodder.FeedReading
{
    public class Feed
    {
        public IEnumerable<FeedItem> Items { get; set; }
    }
}