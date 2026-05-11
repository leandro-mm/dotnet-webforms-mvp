using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebForms_MovieManager.Models;
using WebForms_MovieManager.Views;

namespace MovieManager.Tests.Mocks
{
    internal class MockView : IMovieView
    {
        #region Properties
        public string MovieId { get; set; }
        public string MovieTitle { get; set; }
        public string Director { get; set; }
        public string ReleaseYear { get; set; }
        public string Genre { get; set; }
        public string Rating { get; set; }
        public IEnumerable<Movie> MoviesDataSource { get; set; }
        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        #endregion

        #region Events
        public event EventHandler AddMovieEvent;
        public event EventHandler UpdateMovieEvent;
        public event EventHandler DeleteMovieEvent;
        public event EventHandler EditMovieEvent;
        public event EventHandler LoadMoviesEvent;
        public event EventHandler ClearFormEvent;
        #endregion

        #region Tracking Properties
        //tracking properties
        public bool IsFormCleared { get; private set; }
        public bool IsFormEditMode { get; set; }
        public bool GridWasBound { get; private set; }
        public List<string> ValidationErrors{ get; private set; } = new List<string>();
        #endregion

        #region IMovieView Implementation
        public void BindMoviesGrid()
        {
            GridWasBound = true;
        }

        public void ClearForm()
        {
            IsFormCleared = true;
            MovieId = string.Empty;
            MovieTitle = string.Empty;
            Director = string.Empty;
            ReleaseYear = string.Empty;
            Genre = string.Empty;
            Rating = string.Empty;
        }

        public void SetFormToEditMode(bool isEditMode)
        {
           IsFormEditMode = isEditMode;
        }

        public void ShowValidationErrors(string errors)
        {
            ValidationErrors.Clear();
            if (!string.IsNullOrEmpty(errors))
            {
                ValidationErrors.AddRange(errors.Split(new [] { "<br/>" },
                    StringSplitOptions.None));

            }
            ErrorMessage = errors;
        }
        #endregion

        #region Simulate UI actions
        public void RaiseAddMovieEvent()
        {
            AddMovieEvent?.Invoke(this, EventArgs.Empty);
        }

        public void RaiseUpdateMovieEvent()
        {
            UpdateMovieEvent?.Invoke(this, EventArgs.Empty);
        }

        public void RaiseDeleteMovieEvent()
        {
            DeleteMovieEvent?.Invoke(this, EventArgs.Empty);
        }

        public void RaiseEditMovieEvent()
        {
            EditMovieEvent?.Invoke(this, EventArgs.Empty);
        }
        public void RaiseLoadMovieEvent()
        {
            LoadMoviesEvent?.Invoke(this, EventArgs.Empty);
        }
        public void RaiseClearFormMovieEvent()
        {
            ClearFormEvent?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Helpers
        public void ResetTracking()
        {
            IsFormCleared = false;
            IsFormEditMode= false;
            ValidationErrors.Clear();
            ErrorMessage= string.Empty;
            SuccessMessage= string.Empty;   
        }
        #endregion

    }
}
