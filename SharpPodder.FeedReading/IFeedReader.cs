using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpPodder.FeedReading
{
    public interface IFeedReader
    {
        Feed ReadItems(Uri uri); 
    }
}