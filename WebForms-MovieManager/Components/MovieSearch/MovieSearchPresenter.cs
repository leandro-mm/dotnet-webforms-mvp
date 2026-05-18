using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using WebForms_MovieManager.Components.Base;
using WebForms_MovieManager.Models;
using WebForms_MovieManager.Repositories;
using WebForms_MovieManager.Services;

namespace WebForms_MovieManager.Components.MovieSearch
{
    public class MovieSearchPresenter
        : BaseComponentPresenter<IMovieSearchView, IEnumerable<Movie>>
    {
        private readonly IMovieRepository _repository;        

        public MovieSearchPresenter(
            IMovieSearchView view, 
            IMovieRepository repository, 
            IErrorLogger logger = null) 
            : base(view, logger)
        {
            _repository = repository;
            _logger = logger ?? new ErrorLogger();
            SubscribeToSearchEvents();
        }

        private void SubscribeToSearchEvents()
        {
            View.SearchTriggered += OnSearchTriggered;
            View.SearchCleared += OnSearchCleared;
            View.FilteredChange += OnFilterChanged;
        }

        #region Events
        private void OnFilterChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void OnSearchCleared(object sender, EventArgs e)
        {
            ClearSearch();
        }

        private void ClearSearch()
        {
            View.SearchTerm = string.Empty;
            View.SelectedGenre = string.Empty;
            View.SelectedYear = null;
            View.MinimumRating = null;
            LoadData();
        }

        private void OnSearchTriggered(object sender, EventArgs e)
        {
            LoadData();
        }
        #endregion region

        #region BaseComponentPresenter Methods
        protected override IEnumerable<Movie> OnLoadData()
        {
            return PerformSearch();
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            LoadSearchOptions();
        }
        #endregion

        #region MovieSearchPresenter Methods

        public void RefreshMovieRating(int movieId, double newRating)
        {
            // Get current movies from the view
            var movies = View.DataSource?.ToList();

            if (movies != null)
            {
                // Find and update the specific movie's rating
                var movie = movies.FirstOrDefault(m => m.Id == movieId);
                if (movie != null)
                {
                    movie.Rating = (int)newRating;

                    // Rebind to refresh the display
                    View.DataSource = movies;
                    View.BindData();
                }
            }
        }
        private void LoadSearchOptions()
        {
            try
            {
                var allMovies = _repository.GetAllMovies();

                View.Genres = allMovies
                                .Select(m => m.Genre)
                                .Distinct()
                                .OrderBy(g => g);

                View.AvailableYears = allMovies
                                    .Select(m => m.ReleaseYear)
                                    .Distinct()
                                    .OrderByDescending(y => y);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Failed to load search options");
                View.ShowError("Failed to load search ¥ilters");
            }
        }

        private IEnumerable<Movie> PerformSearch()
        {
            try
            {
                var allMovies = _repository.GetAllMovies();
                var query = allMovies.AsQueryable();

                if (!string.IsNullOrWhiteSpace(View.SearchTerm))
                {
                    string searchTermLower = View.SearchTerm.ToLower();
                    query = query.Where(m =>
                        m.MovieTitle.ToLower().Contains(searchTermLower) ||
                        m.Director.ToLower().Contains(searchTermLower)
                        );
                }

                if (!string.IsNullOrWhiteSpace(View.SelectedGenre))
                {
                    query = query.Where(m => m.Genre == View.SelectedGenre);
                }

                if (View.SelectedYear.HasValue)
                {
                    query = query.Where(m => m.ReleaseYear == View.SelectedYear.Value);
                }

                if (View.MinimumRating.HasValue)
                {
                    query = query.Where(m => m.Rating >= View.MinimumRating.Value);
                }

                var results = query.ToList();
                View.ShowSearchResult(results.Count);

                if (!string.IsNullOrWhiteSpace(View.SearchTerm))
                {
                    View.HighlightSearch(View.SearchTerm);
                }

                return results;

            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Error performing search");
                View.ShowError("Search operation failed");
                return new List<Movie>();
            }
        }
        #endregion
    }
}