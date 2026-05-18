using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebForms_MovieManager.Components.Communication
{
    public class ComponentEventAggregator : IComponentEventAggregator
    {
        private readonly ConcurrentDictionary<Type, List<object>> _handlers = new ConcurrentDictionary<Type, List<object>>();

        public void Publish<TEvent>(TEvent eventData) where TEvent : IComponentEvent
        {
            if (_handlers.TryGetValue(typeof(TEvent), out var handlers))
            {
                foreach (var handler in handlers)
                {
                    ((Action<TEvent>)handler)?.Invoke(eventData);
                }
            }
        }

        public void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : IComponentEvent
        {
            var handlers = _handlers.GetOrAdd(typeof(TEvent), _ => new List<object>());
            lock (handlers)
            {
                handlers.Add(handler);
            }
        }

        public void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : IComponentEvent
        {
            if (_handlers.TryGetValue(typeof(TEvent), out var handlers))
            {
                lock (handlers)
                {
                    handlers.Remove(handler);
                }
            }
        }
    }
}