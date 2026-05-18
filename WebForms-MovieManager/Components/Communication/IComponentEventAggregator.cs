using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebForms_MovieManager.Components.Communication
{
    public interface IComponentEventAggregator
    {
        void Publish<TEvent>(TEvent eventData) where TEvent : IComponentEvent;
        void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : IComponentEvent;
        void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : IComponentEvent;

    }
}
