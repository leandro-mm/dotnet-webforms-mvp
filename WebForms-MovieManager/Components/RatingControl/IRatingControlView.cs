using System;

using WebForms_MovieManager.Components.Base;

namespace WebForms_MovieManager.Components.RatingControl
{
    public interface IRatingControlView :IComponentView<RatingData>
    {
        int MovieId { get; set; }
        double CurrentRate { get; set; }
        bool IsReadOnly { get; set; }

        event EventHandler<RatingChangedEventArgs> RatingChanged;
        event EventHandler RatingSaved;
        void DisplayRating(double rating);
        void ShowRatingSumary(int totalVotes, double averageRating);
    }
}
