using System;


namespace WebForms_MovieManager.Components.Communication
{
    public interface IComponentEvent
    {
        DateTime OccurredAt { get; }
        string SourceComponentld { get; }
    }
}
