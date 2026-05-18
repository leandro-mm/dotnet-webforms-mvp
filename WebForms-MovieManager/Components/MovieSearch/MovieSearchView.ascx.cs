
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebForms_MovieManager.Components.Communication;
using WebForms_MovieManager.Models;
using WebForms_MovieManager.Repositories;
using WebForms_MovieManager.Services;


namespace WebForms_MovieManager.Components.MovieSearch
{
    public partial class MovieSearchView : UserControl, IMovieSearchView
    {
        #region MovieSearchView stuffs

        private MovieSearchPresenter _presenter;
        private bool _isSubscribed = false; // Add subscription tracking

        protected void Page_Load(object sender, EventArgs e)
        {
            InitializePresenter();

            // Subscribe to rating updates
            if (!_isSubscribed)
            {
                EventAggregatorProvider.Instance.Subscribe<RatingUpdatedEvent>(OnRatingUpdated);
                _isSubscribed = true;
            }

            if (!IsPostBack)
            {
                rptResults.ItemCommand += rptResults_ItemCommand;
                ComponentLoaded?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                // Ensure ItemCommand is wired on postback as well
                rptResults.ItemCommand += rptResults_ItemCommand;
            }
        }
        protected override void OnUnload(EventArgs e)
        {
            if (_isSubscribed)
            {
                EventAggregatorProvider.Instance.Unsubscribe<RatingUpdatedEvent>(OnRatingUpdated);
            }
            base.OnUnload(e);
        }

        private void OnRatingUpdated(RatingUpdatedEvent ratingEvent)
        {
            if (_presenter != null)
            {
                // Refresh the rating for the specific movie
                _presenter.RefreshMovieRating(ratingEvent.MovieId, ratingEvent.NewRating);
            }
        }

        protected void rptResults_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "SelectMovie")
            {
                int movieId = Convert.ToInt32(e.CommandArgument);

                // Get the full movie object from the data source
                var movies = DataSource as IEnumerable<Movie>;
                var selectedMovie = movies?.FirstOrDefault(m => m.Id == movieId);

                if (selectedMovie != null)
                {
                    OnMovieSelected(movieId, selectedMovie);
                }
            }
        }
        // Publish movie selected event
        private void OnMovieSelected(int movieId, Movie movie)
        {
            var selectedEvent = new MovieSelectedEvent
            {
                SourceComponentId = this.ComponentId,
                MovieId = movieId,
                Movie = movie
            };

            EventAggregatorProvider.Instance.Publish(selectedEvent);
        }
        private void InitializePresenter()
        {
            if (_presenter == null)
            {

                var repository = new MovieRepository();
                var logger = new ErrorLogger();
                _presenter = new MovieSearchPresenter(this, repository, logger);
                _presenter.Initialize();
            }
        }
        #endregion

        #region Events
        protected void btnSearch_Click(object sender, EventArgs e)
        {            
            SearchTriggered?.Invoke(this, EventArgs.Empty);
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {            
            SearchCleared?.Invoke(this, EventArgs.Empty);
        }

        protected void btnToggleAdvanced_Click(object sender, EventArgs e)
        {
            bool isVisible = divAdvancedSearch.Style["display"] == "block";
            SetAdvancedSearchVisible(!isVisible);
            btnToggleAdvanced.Text = isVisible ? "Advanced Search" : "Basic Search";
        }

        protected void ddlGenre_SelectedIndexChanged(object sender, EventArgs e)
        {            
            FilteredChange?.Invoke(this, EventArgs.Empty);
        }

        protected void ddlYear_SelectedIndexChanged(object sender, EventArgs e)
        {            
            FilteredChange?.Invoke(this, EventArgs.Empty);
        }

        protected void ddlRating_SelectedIndexChanged(object sender, EventArgs e)
        {            
            FilteredChange?.Invoke(this, EventArgs.Empty);
        }

        protected string HighlightSearchTerm(string text)
        {
            string searchTerm = ViewState["SearchTerm"] as string;

            if (string.IsNullOrEmpty(searchTerm))
                return text;

            return Regex.Replace(text, Regex.Escape(searchTerm),
                match => $"<span class='search-term-highlight'>{match.Value}</span>",
                RegexOptions.IgnoreCase);
        }

        #endregion


        #region IMovieSearchView stuffs

        public string SearchTerm 
        {
            get => txtSearchTerm.Text;
            set 
            { 
                txtSearchTerm.Text = value;
            }
        }
        public string SelectedGenre 
        {
            get => ddlGenre.SelectedValue;
            set
            {
                if (ddlGenre.Items.FindByValue(value) != null)
                {
                    ddlGenre.SelectedValue = value;
                }   
            }
        }
        public int? SelectedYear 
        {
            get => string.IsNullOrEmpty(ddlYear.SelectedValue) ? (int?)null :
               Convert.ToInt32(ddlYear.SelectedValue);

            set
            {
                ddlYear.SelectedValue = value?.ToString() ?? "";
            }
        }
        public double? MinimumRating 
        {
            get => string.IsNullOrEmpty(ddlRating.SelectedValue) ? (double?)null :
               Convert.ToDouble(ddlRating.SelectedValue);

            set 
            {
                ddlRating.SelectedValue = value?.ToString() ?? "";
            }
        }
        public IEnumerable<string> Genres 
        {
            get => (IEnumerable<string>)ViewState["Genres"] ?? new List<string>();

            set
            {
                ddlGenre.Items.Clear();
                ddlGenre.Items.Add(new ListItem("All Genres", ""));
                foreach (var genre in value)
                {
                    ddlGenre.Items.Add(new ListItem(genre, genre));
                }
            }
        }
        public IEnumerable<int> AvailableYears 
        {
            get => (IEnumerable<int>)ViewState["AvailableYears"] ?? new List<int>();

            set
            {
                var currentValue = ddlYear.SelectedValue;
                ddlYear.Items.Clear();
                ddlYear.Items.Add(new ListItem("All Years", ""));

                foreach (var year in value)
                {

                    ddlYear.Items.Add(new ListItem(year.ToString(), year.ToString()));
                }

                if (!string.IsNullOrEmpty(currentValue) &&
                    ddlYear.Items.FindByValue(currentValue) != null)
                {
                    ddlYear.SelectedValue = currentValue;
                }
            }
        }
        public IEnumerable<Movie> DataSource 
        {
            get => (IEnumerable<Movie>)rptResults.DataSource;
            set 
            {
                rptResults.DataSource = value;
            }
        }

        // Gets the control ID for HTML markup that is generated by ASP.NET.
        public string ComponentId => $"MovieSearch_{ClientID}";

        public event EventHandler SearchTriggered;
        public event EventHandler SearchCleared;
        public event EventHandler FilteredChange;
        public event EventHandler ComponentLoaded;
        public event EventHandler ComponentDatachanged;

        public void BindData()
        {
            rptResults.DataBind();
        }

        public void ClearData()
        {            
            rptResults.DataSource = null;
            rptResults.DataBind();
            ComponentDatachanged?.Invoke(this, EventArgs.Empty);
        }

        public void HighlightSearch(string term)
        {
            ViewState["SearchTerm"] = term;
        }

        public void SetAdvancedSearchVisible(bool visible)
        {
            divAdvancedSearch.Style["display"] = visible ? "block" : "none";
        }

        public void SetLoadingState(bool isLoading)
        {
            loadingIndicator.Visible = isLoading;
            btnSearch.Enabled = !isLoading;
            btnClear.Enabled = !isLoading;
        }

        public void ShowError(string message)
        {
            var script = $"alert('Error: {message.Replace("\\", "\\\\").Replace("'", "\\'")}');";
            ScriptManager.RegisterStartupScript(this, GetType(), "Error", script, true);
        }

        public void ShowMessage(string message)
        {
            var script = $"alert('{message.Replace("'", "\\'")});";
            ScriptManager.RegisterStartupScript(this, GetType(), "Message", script, true);
        }

        public void ShowSearchResult(int resultCount)
        {
            lblResultsCount.Text = $"Found {resultCount} movie{(resultCount != 1 ? "s" : "")}";
            lblNoResults.Visible = resultCount == 0;
        }

        #endregion
    }
}