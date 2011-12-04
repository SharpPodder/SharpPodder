using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpPodder.FeedReading
{
    public class FeedItemLink
    {
        public Uri Uri { get; set; }
        public string MediaType { get; set; }
        public string RelationshipType { get; set; }
        public long Length { get; set; }
        public string Title { get; set; }
    }
}