using Microsoft.Ajax.Utilities;
using System;
using WebForms_MovieManager.Components.Base;
using WebForms_MovieManager.Repositories;
using WebForms_MovieManager.Services;

namespace WebForms_MovieManager.Components.RatingControl
{
    public class RatingControlPresenter : BaseComponentPresenter<IRatingControlView,RatingData>
    {
        private readonly IMovieRepository _repository;
        private readonly IErrorLogger _logger;

        public RatingControlPresenter(IRatingControlView view, IMovieRepository movieRepository, IErrorLogger errorLogger=null)
            :base(view,errorLogger)
        {
            _repository = movieRepository;
            _logger = errorLogger;
            SubscribeToRatingEvents();
        }

        private void SubscribeToRatingEvents()
        {
            View.RatingChanged += OnRatingChanged;
            View.RatingSaved += OnRatingSaved;
        }

        private void OnRatingSaved(object sender, EventArgs e)
        {
            try
            {
                var movie = _repository.GetMovieById(View.MovieId);
                if (movie != null)
                {
                    var newAverage = (movie.Rating + View.CurrentRate) / 2;
                    movie.Rating = (int)Math.Round(newAverage, 1);
                    _repository.UpdateMovie(movie);
                    View.ShowMessage("Rating saved successfully!");
                    LoadData(); // Refresh dispLay
                }
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, $"Error saving rating For movie {View.MovieId}");
                View.ShowError("Failed to save rating");
            }
        }

        private void OnRatingChanged(object sender, RatingChangedEventArgs e)
        {
            try
            {
                _logger.LogInformation($"Rating changed for movie " +
                    $"{e.MovieId}: {e.OldRating} -> {e.NewRating}");

                View.DisplayRating(e.NewRating);

                // In reaL app, you might auto-save or just store temporariLy
                View.CurrentRate = e.NewRating;
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, $"Error handling rating change For movie {View.MovieId}");
                View.ShowError("Failed to update rating");
            }
        }

        protected override RatingData OnLoadData()
        {
            return LoadRatingData();
        }

        private RatingData LoadRatingData()
        {
            try
            {
                var movie = _repository.GetMovieById(View.MovieId);
                if (movie == null)
                    return null;

                // SimuLate rating data (in reaL app, this wouLd come from a rating depository
                var ratingData = new RatingData
                {
                    MovieId = View.MovieId,
                    AverageRating = movie.Rating,
                    TotalVotes = new Random().Next(1, 100), // SimuLated votes
                    UserRating = View.CurrentRate,
                    HasUserRated = View.CurrentRate > 0
                };

                View.ShowRatingSumary(ratingData.TotalVotes, ratingData.AverageRating);
                return ratingData;

            }
            catch (Exception ex)
            {

                _logger.LogError(ex, $"Error loading rating data For movie {View.MovieId}");
                View.ShowError("Failed to load rating data");
                return null;
            }
        }
    }
}