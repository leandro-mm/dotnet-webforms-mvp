using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebForms_MovieManager.Models;
using WebForms_MovieManager.Presenters;
using WebForms_MovieManager.Repositories;
using WebForms_MovieManager.Views;

namespace WebForms_MovieManager.WebForms
{
    public partial class MovieManagement : Page, IMovieView
    {
        private MoviePresenter _presenter;

        #region Properties
        public string MovieId
        {
            get { return hdnMovieId.Value; }
            set { hdnMovieId.Value = value; }
        }

        public string MovieTitle
        {
            get { return txtTitle.Text; }
            set { txtTitle.Text = value; }
        }

        public string Director
        {
            get { return txtDirector.Text; }
            set { txtDirector.Text = value; }
        }
        public string ReleaseYear
        {
            get { return txtReleaseYear.Text; }
            set { txtReleaseYear.Text = value; }
        }
        public string Genre
        {
            get { return ddlGenre.SelectedValue; }
            set
            {
                if (ddlGenre.Items.FindByValue(value) != null)
                    ddlGenre.SelectedValue = value;
            }
        }
        public string Rating
        {
            get { return txtRating.Text; }
            set { txtRating.Text = value; }
        }
        public IEnumerable<Movie> MoviesDataSource
        {
            get { return (IEnumerable<Movie>)gvMovies.DataSource; }
            set { gvMovies.DataSource = value; }
        }
        public string ErrorMessage
        {
            get
            {
                // Return the current error message text
                return lblMessage.Visible && lblMessage.CssClass == "message-error" ?
                    lblMessage.Text : string.Empty;
            }
            set
            {
                lblMessage.Text = value;
                lblMessage.CssClass = "message-error";
                lblMessage.Visible = !string.IsNullOrEmpty(value);
            }
        }
        public string SuccessMessage
        {
            get
            {
                // Return the current success message text
                return lblMessage.Visible ? lblMessage.Text : string.Empty;
            }
            set
            {
                lblMessage.Text = value;
                lblMessage.CssClass = "message-success";
                lblMessage.Visible = !string.IsNullOrEmpty(value);
            }
        }
        #endregion

        #region Events
        public event EventHandler AddMovieEvent;
        public event EventHandler UpdateMovieEvent;
        public event EventHandler DeleteMovieEvent;
        public event EventHandler EditMovieEvent;
        public event EventHandler LoadMoviesEvent;
        public event EventHandler ClearFormEvent;
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                _presenter = new MoviePresenter(this, new MovieRepository());
                LoadMoviesEvent?.Invoke(this, EventArgs.Empty);
            }

            //initialize presenter on every postback
            if(_presenter == null)
            {
                _presenter = new MoviePresenter(this, new MovieRepository());
            }
        }

        protected void gvMovies_SelectedIndexChanged(object sender, EventArgs e)
        {
            //fires when a row’s "Select" button is clicked.
            GridView grid = (GridView)sender;            
            int selectedIndex = grid.SelectedIndex;

            if (selectedIndex >= 0) 
            { 
                MovieId = gvMovies.DataKeys[selectedIndex].Value.ToString();
                EditMovieEvent?.Invoke(this, EventArgs.Empty);
            }
        }

        #region IMovieView Methods
        
        //
        public void BindMoviesGrid()
        {
            gvMovies.DataBind();
        }

        public void ClearForm()
        {
            MovieId = "";
            MovieTitle = "";
            Director = "";
            ReleaseYear = "";
            Genre = "";            
            Rating = "";
            hdnMovieId.Value = "";
            txtTitle.Text = "";
        }

        public void SetFormToEditMode(bool isEditMode)
        {
            btnAdd.Visible = !isEditMode;
            btnUpdate.Visible = isEditMode;
        }

        public void ShowValidationErrors(string errors)
        {
            //using the built-in validation summary
            ErrorMessage= errors;
        }
        #endregion

        #region Button Clicks

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                AddMovieEvent?.Invoke(this, EventArgs.Empty);
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                UpdateMovieEvent?.Invoke(this, EventArgs.Empty);
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            DeleteMovieEvent?.Invoke(this, EventArgs.Empty);
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearFormEvent?.Invoke(this, EventArgs.Empty);
        }

        protected void btnLoad_Click(object sender, EventArgs e)
        {
            LoadMoviesEvent?.Invoke(this, EventArgs.Empty);
        }
        
        #endregion
    }
}