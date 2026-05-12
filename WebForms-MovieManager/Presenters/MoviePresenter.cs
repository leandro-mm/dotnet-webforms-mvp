using System;
using WebForms_MovieManager.Models;
using WebForms_MovieManager.Repositories;
using WebForms_MovieManager.Services;
using WebForms_MovieManager.Validators;
using WebForms_MovieManager.Views;

namespace WebForms_MovieManager.Presenters
{
    public class MoviePresenter
    {
        private readonly IMovieRepository _repository;
        private readonly IMovieView _view;
        private readonly IErrorLogger _logger;

        public MoviePresenter(IMovieView view, IMovieRepository repository)
            :this (view, repository, new ErrorLogger()){}

        public MoviePresenter(IMovieView view, IMovieRepository repository, IErrorLogger errorLogger)
        {
            _view = view;
            _repository = repository;
            _logger = errorLogger;

            EventsSubscribe();
        }

        private void EventsSubscribe()
        {
            _view.AddMovieEvent += OnAddMovie;
            _view.UpdateMovieEvent += OnUpdateMovie;
            _view.DeleteMovieEvent += OnDeleteMovie;
            _view.EditMovieEvent += OnLoadMovieInfo;
            _view.LoadMoviesEvent += OnLoadMovies;
            _view.ClearFormEvent += OnClearForm;
        }

        #region Event Handlers
        private void OnClearForm(object sender, EventArgs e)
        {
            try
            {
                _logger.LogInformation("Clearing movie form");

                _view.ClearForm();
                _view.SetFormToEditMode(false);
                _view.SuccessMessage = "Form Cleared";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Failed to clear movie form");
                throw;
            }
            
        }

        private void OnLoadMovies(object sender, EventArgs e)
        {
            _logger.LogInformation("Loading all movies.");
            LoadAllMovies();
        }

        private void OnLoadMovieInfo(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(_view.MovieId))
                {
                    _view.ShowValidationErrors("Please select a movie to edit");                    
                    return;
                }

                int movieId = Convert.ToInt32(_view.MovieId);
                var movie = _repository.GetMovieById(movieId);
                
                _logger.LogInformation($"Loading movie {_view.MovieId} for edit");

                if (movie != null)
                {
                    //populate the view with movie data
                    _view.MovieTitle = movie.MovieTitle;
                    _view.Director = movie.Director;
                    _view.ReleaseYear = movie.ReleaseYear.ToString();
                    _view.Genre = movie.Genre;
                    _view.Rating = movie.Rating.ToString();

                    _view.SetFormToEditMode(true);
                    _view.SuccessMessage = "Edit mode - Update the movie and click update";

                    _logger.LogInformation($"Movie {_view.MovieId} loaded for edit ");
                }
                else
                {
                    _view.ErrorMessage = "Movie not found";
                    _logger.LogWarning($"Movie {_view.MovieId} loaded for edit ");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to load movie {_view.MovieId} for edit");
                throw;
            }

            

        }

        private void OnDeleteMovie(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(_view.MovieId))
                {
                    _view.ShowValidationErrors("No movie selected for deletion");
                    return;
                }

                int movieId = Convert.ToInt32(_view.MovieId);
                _logger.LogWarning($"deleting movie with ID {_view.MovieId}");

                if (_repository.MovieExists(movieId))
                {
                    _repository.DeleteMovieById(movieId);
                    LoadAllMovies();
                    _view.ClearForm();
                    _view.SuccessMessage = "Movie deleted successfully!";
                    _view.SetFormToEditMode(false);

                    _logger.LogInformation($"Movie {_view.MovieId} deleted successfully");
                }
                else
                {
                    _view.ErrorMessage = "Movie not found";
                    _logger.LogWarning($"Attempted to delete non-existent movi {_view.MovieId}.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,$"Failed to delete movie {_view.MovieId}");
                throw;
            }
            
        }

        private void OnUpdateMovie(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(_view.MovieId))
                {
                    _view.ShowValidationErrors("No movie selected for update");
                    return;
                }

                _logger.LogInformation($"Updating movie ID: {_view.MovieId}");

                var movie = new Movie
                {
                    Id = Convert.ToInt32(_view.MovieId),
                    MovieTitle = _view.MovieTitle,
                    Director = _view.Director,
                    ReleaseYear = Convert.ToInt32(_view.ReleaseYear),
                    Genre = _view.Genre,
                    Rating = Convert.ToInt32(_view.Rating)
                };

                //validate movie
                var validationErrors = MovieValidator.ValidateMovie(movie);
                if (validationErrors.Count > 0)
                {
                    _view.ShowValidationErrors(string.Join("<br/>", validationErrors));
                    _logger.LogWarning($"Error while updating movie with ID {_view.MovieId}: " +
                        $"{Environment.NewLine} \t {string.Join(",", validationErrors)}");
                    return;
                }

                //update in the repository
                _repository.UpdateMovie(movie);

                //Refresh the grid
                LoadAllMovies();

                //clear form and show success
                _view.ClearForm();
                _view.SuccessMessage = "Movie was update successfully!";

                //reset form mode
                _view.SetFormToEditMode(false);

                _logger.LogInformation($"Movie {_view.MovieTitle} has been updated successfully.");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to update movie {_view.MovieId}");
                throw;
            }

           

           

           

           
        }

        private void OnAddMovie(object sender, EventArgs e)
        {
            try
            {
                _logger.LogInformation($"Adding new movie: {_view.MovieTitle}");

                int year;
                if (!int.TryParse(_view.ReleaseYear, out year))
                {
                    _view.ShowValidationErrors("Release year is required");
                    _logger.LogError($"Unable to handle {_view.ReleaseYear} as integer",ErrorSeverity.Error);
                    return;
                }
                //create movie from view data
                var movie = new Movie
                {
                    MovieTitle = _view.MovieTitle,
                    Director = _view.Director,
                    ReleaseYear = year,
                    Genre = _view.Genre,
                    Rating = Convert.ToInt32(_view.Rating)
                };

                //validate movie
                var validationErrors = MovieValidator.ValidateMovie(movie);
                if (validationErrors.Count > 0)
                {
                    _view.ShowValidationErrors(string.Join("<br/>", validationErrors));
                    _logger.LogWarning($"Validation failed for movie {Environment.NewLine} \t {string.Join(",", validationErrors)}");
                    return;
                }

                //add to repository
                _repository.AddMovie(movie);

                //refresh the grid
                LoadAllMovies();

                //clear form and show success
                _view.ClearForm();
                _view.SuccessMessage = "Movie added successfully!";

                //reset form mode
                _view.SetFormToEditMode(false);

                _logger.LogInformation($"Movie added successfully: {movie.MovieTitle} with ID {movie.Id}");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to add movie {_view.MovieTitle}");
                throw;
            }
            
        }

        private void LoadAllMovies()
        {
            try
            {
                var movies = _repository.GetAllMovies();
                _view.MoviesDataSource = movies;
                _view.BindMoviesGrid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                throw;
            }
           
        }
        #endregion
        
        #region Log
        private EventHandler SafeEventHandler(Action handler)
        {
            return (sender, e) =>
            {
                try
                {
                    handler();
                }
                catch (Exception ex)
                {

                    HandlePresenterError(ex);
                }
            };
        }

        private void HandlePresenterError(Exception ex)
        {
            _logger.LogError(ex);

            var errorMessage = CustomMessage.FriendlyMessagePresenter(ex);
            _view.ErrorMessage= errorMessage;
        }
        #endregion
    }
}