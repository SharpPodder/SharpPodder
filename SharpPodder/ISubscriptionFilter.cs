using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpPodder.FeedMerging;

namespace SharpPodder
{
    public interface ISubscriptionFilter
    {
        void Filter(MergeResult MergeResult);
    }
}