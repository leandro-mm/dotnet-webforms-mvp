using System;

namespace WebForms_MovieManager.Components.Communication
{
    public class RatingUpdatedEvent : IComponentEvent
    {
        public DateTime OccurredAt { get; private set; }
        public string SourceComponentId { get; set; }
        public int MovieId { get; set; }
        public double NewRating { get; set; }

        public string SourceComponentld { get; set; }

        public RatingUpdatedEvent()
        {
            OccurredAt = DateTime.Now;
        }
    }
}