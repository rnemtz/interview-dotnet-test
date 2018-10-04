using System;
using System.Collections.Concurrent;

namespace EazeCrawler.Common.Structures
{
    public class EventQueue<T> : ConcurrentQueue<T>
    {
        public event EventHandler<T> ItemEnqueued;

        protected virtual void OnItemEnqueued(T item)
        {
            ItemEnqueued?.Invoke(this, item);
        }

        public new virtual void Enqueue(T item)
        {
            base.Enqueue(item);
            OnItemEnqueued(item);
        }
    }
}