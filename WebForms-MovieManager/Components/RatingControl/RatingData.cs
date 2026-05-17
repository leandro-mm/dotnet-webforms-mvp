

namespace WebForms_MovieManager.Components.RatingControl
{
    public class RatingData
    {
        public int MovieId { get; set; }
        public double AverageRating { get; set; }
        public int TotalVotes { get; set; }
        public double UserRating { get; set; }
        public bool HasUserRated { get; set; }
        public string RatingDisplay => $"{AverageRating:F1} * ({TotalVotes} votes)";
    }
}