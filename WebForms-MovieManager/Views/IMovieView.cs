using System;
using System.Collections.Generic;
using WebForms_MovieManager.Models;

namespace WebForms_MovieManager.Views
{
    public interface IMovieView
    {
        //properties for movie form fields
        string MovieId { get; set; }
        string MovieTitle { get; set; }
        string Director { get; set; }
        string ReleaseYear { get; set; }
        string Genre { get; set; }
        string Rating { get; set; }

        //GridView binding property
        IEnumerable<Movie> MoviesDataSource { get; set; }

        //UI State
        string ErrorMessage { get; set; }
        string SuccessMessage { get; set; }

        //Events
        event EventHandler AddMovieEvent;
        event EventHandler UpdateMovieEvent;
        event EventHandler DeleteMovieEvent;
        event EventHandler EditMovieEvent;
        event EventHandler LoadMoviesEvent;
        event EventHandler ClearFormEvent;

        //Methods
        void BindMoviesGrid();
        void ClearForm();
        void SetFormToEditMode(bool isEditMode);
        void ShowValidationErrors(string errors);

    }
}