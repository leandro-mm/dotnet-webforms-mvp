using System;


namespace WebForms_MovieManager.Components.RatingControl
{
    public class RatingChangedEventArgs :EventArgs
    {
        public int MovieId { get; set; }
        public double NewRating { get; set; }
        public double OldRating { get; set; }

        public RatingChangedEventArgs(int movieI, double newRating, double oldRating)
        {
            MovieId = movieI;
            NewRating = newRating;
            OldRating = oldRating;

        }
    }
}