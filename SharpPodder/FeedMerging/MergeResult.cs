using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpPodder.FeedMerging
{
    public class MergeResult : IEnumerable<SubscriptionItem>
    {
        private readonly List<SubscriptionItem> items;
        private readonly Dictionary<ItemMergeStatus, HashSet<SubscriptionItem>> itemsByStatus; 

        public MergeResult()
        {
            items = new List<SubscriptionItem>();
            itemsByStatus = new Dictionary<ItemMergeStatus, HashSet<SubscriptionItem>>();
            foreach (var status in AllStatusValues())
                itemsByStatus[status] = new HashSet<SubscriptionItem>();
        }

        public void AddItem(SubscriptionItem item, ItemMergeStatus mergeStatus)
        {
            items.Add(item);
            itemsByStatus[mergeStatus].Add(item);
        }

        public bool ItemBelongsTo(SubscriptionItem item, params ItemMergeStatus[] mergeStatus)
        {
            if (mergeStatus == null || !mergeStatus.Any())
                mergeStatus = AllStatusValues();
            return mergeStatus.Any(x => itemsByStatus[x].Contains(item));
        }

        public IEnumerable<SubscriptionItem> AllItemsBelongsTo(params ItemMergeStatus[] mergeStatus)
        {
            return items.Where(x => ItemBelongsTo(x, mergeStatus));
        }

        private static ItemMergeStatus[] AllStatusValues()
        {
            return Enum.GetValues(typeof(ItemMergeStatus)).Cast<ItemMergeStatus>().ToArray();
        }

        public bool ThereWereNoItems
        {
            get { return !itemsByStatus[ItemMergeStatus.NoChangedItem].Any() && !itemsByStatus[ItemMergeStatus.UpdatedItem].Any() && !itemsByStatus[ItemMergeStatus.RemovedItem].Any(); }
        }
        
        public IEnumerator<SubscriptionItem> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.IEnumerable)items).GetEnumerator();
        }
    }
}