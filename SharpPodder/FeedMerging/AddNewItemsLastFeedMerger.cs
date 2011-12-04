using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpPodder.FeedReading;
using SharpPodder.Utilities;

namespace SharpPodder.FeedMerging
{
    public class AddNewItemsLastFeedMerger : IFeedMerger
    {
        public MergeResult Merge(IEnumerable<SubscriptionItem> previousItems, IEnumerable<FeedItem> currentItems)
        {
            var result = new MergeResult();
            var previousAuxList = new KeyedCollection<string, SubscriptionItem>(x => x.Id, previousItems);
            var currentAuxList = new KeyedCollection<string, FeedItem>(x => x.Id, currentItems);
            
            var previousIds = previousAuxList.Select(x => x.Id).ToArray();
            foreach (var id in previousIds)
            {
                if (!currentAuxList.Contains(id))
                    ChooseRemovedItem(id, previousAuxList, result);
                else
                    ChooseMergeItem(id, previousAuxList, currentAuxList, result);
            }
            
            var newIds = currentAuxList.Select(x => x.Id).ToArray();
            foreach (var id in newIds)
                ChooseNewItem(id, currentAuxList, result);
            
            return result;
        }

        private void ChooseNewItem(string id, KeyedCollection<string, FeedItem> currentAuxList, MergeResult result)
        {
            result.AddItem(new SubscriptionItem(currentAuxList.Take(id)), ItemMergeStatus.NewItem);
        }

        private void ChooseMergeItem(string id, KeyedCollection<string, SubscriptionItem> previousAuxList, KeyedCollection<string, FeedItem> currentAuxList, MergeResult result)
        {
            var previous = previousAuxList.Take(id);
			var current = currentAuxList.Take(id);
            if (previous.LastUpdatedTime == current.LastUpdatedTime)
            {
                result.AddItem(previous, ItemMergeStatus.NoChangedItem);
            }
            else
            {
                previous.Update(current);
                result.AddItem(previous, ItemMergeStatus.UpdatedItem);
            }
        }

        private void ChooseRemovedItem(string id, KeyedCollection<string, SubscriptionItem> previousAuxList, MergeResult result)
        {
            result.AddItem(previousAuxList.Take(id), ItemMergeStatus.RemovedItem);
        }
    }
}