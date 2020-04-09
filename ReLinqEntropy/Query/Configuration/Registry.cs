using System;
using System.Collections.Generic;
using System.Linq;

namespace ReLinqEntropy.Query.Configuration
{
    public abstract class Registry<TRegistry, TKey, TItem>
        where TRegistry : Registry<TRegistry, TKey, TItem>, new()
    {
        internal readonly Dictionary<TKey, TItem> _items = new Dictionary<TKey, TItem>();

        public static TRegistry CreateDefault()
        {
            var defaultItemTypes = from t in typeof(TRegistry).Assembly.GetTypes()
                                   where typeof(TItem).IsAssignableFrom(t) && !t.IsAbstract
                                   select t;

            var registry = new TRegistry();
            registry.RegisterForTypes(defaultItemTypes);
            return registry;

        }

        public virtual void Register(TKey key, TItem item)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            _items.Add(key, item);
        }

        public void Register(IEnumerable<TKey> keys, TItem item)
        {
            if (keys == null)
            {
                throw new ArgumentNullException(nameof(keys));
            }
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            foreach (var key in keys)
            {
                _items.Add(key, item);
            }
        }

        public abstract TItem GetItem(TKey key);

        public virtual bool IsRegistered(TKey key)
        {
            return _items.ContainsKey(key);
        }

        protected internal virtual TItem GetItemExact(TKey key)
        {
            _items.TryGetValue(key, out TItem item);
            return item;
        }

        protected internal abstract void RegisterForTypes(IEnumerable<Type> itemTypes);
    }
}
