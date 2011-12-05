using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpPodder.FeedMerging
{
    public enum ItemMergeStatus
    {
        NoChangedItem,
        RemovedItem,
        UpdatedItem,
        NewItem
    }
}