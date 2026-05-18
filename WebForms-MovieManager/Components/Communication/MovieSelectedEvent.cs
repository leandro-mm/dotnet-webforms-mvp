using System;
using WebForms_MovieManager.Models;

namespace WebForms_MovieManager.Components.Communication
{
    public class MovieSelectedEvent : IComponentEvent
    {
        public DateTime OccurredAt { get; private set; }
        public string SourceComponentId { get; set; }
        public int MovieId { get; set; }
        public Movie Movie { get; set; }

        public string SourceComponentld { get; set; }

        public MovieSelectedEvent()
        {
            OccurredAt = DateTime.Now;
        }
    }
}