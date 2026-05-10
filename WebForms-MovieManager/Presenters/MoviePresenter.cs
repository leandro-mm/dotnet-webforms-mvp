using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebForms_MovieManager.Models;
using WebForms_MovieManager.Repositories;
using WebForms_MovieManager.Validators;
using WebForms_MovieManager.Views;

namespace WebForms_MovieManager.Presenters
{
    public class MoviePresenter
    {
        private readonly IMovieRepository _repository;
        private readonly IMovieView _view;

        public MoviePresenter(IMovieView view, IMovieRepository repository)
        {
            _view = view;
            _repository = repository;

            //subscribe to events
            _view.AddMovieEvent += OnAddMovie;
            _view.UpdateMovieEvent += OnUpdateMovie;
            _view.DeleteMovieEvent += OnDeleteMovie;
            _view.EditMovieEvent += OnEditMovie;
            _view.LoadMoviesEvent += OnLoadMovies;
            _view.ClearFormEvent += OnClearForm;
        }

        private void OnClearForm(object sender, EventArgs e)
        {
            _view.ClearForm();
            _view.SetFormToEditMode(false);
            _view.SuccessMessage = "Form Cleared";
        }

        private void OnLoadMovies(object sender, EventArgs e)
        {
            LoadAllMovies();
        }

        private void OnEditMovie(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_view.MovieId))
            {
                _view.ShowValidationErrors("Please select a movie to edit");
                return;
            }
            
            int movieId = Convert.ToInt32(_view.MovieId);
            var movie = _repository.GetMovieById(movieId);

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
            }
            else
            {
                _view.ErrorMessage = "Movie not found";
            }

        }

        private void OnDeleteMovie(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_view.MovieId))
            {
                _view.ShowValidationErrors("No movie selected for deletion");
                return;
            }

            int movieId = Convert.ToInt32(_view.MovieId);

            if (_repository.MovieExists(movieId)) 
            { 
                _repository.DeleteMovieById(movieId);
                LoadAllMovies();
                _view.ClearForm();
                _view.SuccessMessage = "Movie deleted successfully!";
                _view.SetFormToEditMode(false);
            }
            else
            {
                _view.ErrorMessage = "Movie not found";
            }
        }

        private void OnUpdateMovie(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_view.MovieId))
            {
                _view.ShowValidationErrors("No movie selected for update");
                return;
            }

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
                return;
            }

            //update in the repository
            _repository.UpdateMovie(movie);

            //Refresh the grid
            LoadAllMovies();

            //clear form and show success
            _view.ClearForm();
            _view.SuccessMessage = "Movie added successfully!";

            //reset form mode
            _view.SetFormToEditMode(false);
        }

        private void OnAddMovie(object sender, EventArgs e)
        {
            //create movie from view data
            var movie = new Movie 
            { 
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
        }

        private void LoadAllMovies()
        {
            var movies = _repository.GetAllMovies();
            _view.MoviesDataSource = movies;
            _view.BindMoviesGrid();
        }
    }
}