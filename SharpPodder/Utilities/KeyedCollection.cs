using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpPodder.Utilities
{
    public class KeyedCollection<TKey, TItem> : System.Collections.ObjectModel.KeyedCollection<TKey, TItem>
    {
        private Func<TItem, TKey> getKey;

        public KeyedCollection(Func<TItem, TKey> getKey)
        {
            this.getKey = getKey;
        }

        public KeyedCollection(Func<TItem, TKey> getKey, IEnumerable<TItem> items)
            : this(getKey)
        {
            foreach (var item in items)
                Add(item);
        }

        protected override TKey GetKeyForItem(TItem item)
        {
            return getKey(item);
        }
		
		public TItem Take(TKey key)
		{
			var value = this[key];
			this.Remove(key);
			return value;
		}
    }
}