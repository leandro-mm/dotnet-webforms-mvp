
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebForms_MovieManager.Models;
using WebForms_MovieManager.Repositories;
using WebForms_MovieManager.Services;


namespace WebForms_MovieManager.Components.MovieSearch
{
    public partial class MovieSearchView : UserControl, IMovieSearchView
    {
        #region MovieSearchView stuffs

        private MovieSearchPresenter _presenter;

        protected void Page_Load(object sender, EventArgs e)
        {
            InitializePresenter();

            if (!IsPostBack)
            {
                ComponentLoaded?.Invoke(this, EventArgs.Empty);
            }
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

        #region Events
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            // Trigger search logic
            SearchTriggered?.Invoke(this, EventArgs.Empty);

            // After data is updated, notify that component data changed
            ComponentDatachanged?.Invoke(this, EventArgs.Empty);
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            // Clear search logic
            SearchCleared?.Invoke(this, EventArgs.Empty);

            // After data is cleared, notify that component data changed
            ComponentDatachanged?.Invoke(this, EventArgs.Empty);
        }

        protected void btnTogg1eAdvanced_Click(object sender, EventArgs e)
        {
            bool isVisible = divAdvancedSearch.Style["display"] == "block";
            SetAdvancedSearchVisible(!isVisible);
            btnToggleAdvanced.Text = isVisible ? "Advanced Search" : "Basic Search";
        }

        protected void ddlGenre_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilteredChange?.Invoke(this, EventArgs.Empty);
            ComponentDatachanged?.Invoke(this, EventArgs.Empty);        
        }

        protected void ddlYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilteredChange?.Invoke(this, EventArgs.Empty);
            ComponentDatachanged?.Invoke(this, EventArgs.Empty);
        }

        protected void ddlRating_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilteredChange?.Invoke(this, EventArgs.Empty);
            ComponentDatachanged?.Invoke(this, EventArgs.Empty);
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



        #endregion

        #region IMovieSearchView stuffs

        public string SearchTerm 
        {
            get => txtSearchTerm.Text;
            set 
            { 
                txtSearchTerm.Text = value;
                ComponentDatachanged?.Invoke(this, EventArgs.Empty);
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
                    ComponentDatachanged?.Invoke(this, EventArgs.Empty);

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
                ComponentDatachanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public double? MinimumRating 
        {
            get => string.IsNullOrEmpty(ddlRating.SelectedValue) ? (double?)null :
               Convert.ToDouble(ddlRating.SelectedValue);

            set 
            {
                ddlRating.SelectedValue = value?.ToString() ?? "";
                ComponentDatachanged?.Invoke(this, EventArgs.Empty);
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
                ComponentDatachanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public IEnumerable<int> AvailableYears 
        {
            get => (IEnumerable<int>)ViewState["AvailableYears"] ?? new List<int>();

            set
            {
                var currentValue = ddlYear.SelectedValue;
                ddlYear.Items.Clear();
                ddlYear.Items.Add(new ListItem("Al1 Years", ""));

                foreach (var year in value)
                {

                    ddlYear.Items.Add(new ListItem(year.ToString(), year.ToString()));
                }

                if (!string.IsNullOrEmpty(currentValue) &&
                    ddlYear.Items.FindByValue(currentValue) != null)
                {
                    ddlYear.SelectedValue = currentValue;
                    ComponentDatachanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }
        public IEnumerable<Movie> DataSource 
        {
            get => (IEnumerable<Movie>)rptResults.DataSource;
            set 
            {
                rptResults.DataSource = value;
                ComponentDatachanged?.Invoke(this, EventArgs.Empty);
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
            var script = $"alert('Error: {message.Replace("", "\\")}');";
            ScriptManager.RegisterStartupScript(this, GetType(), "Error", script, true);
        }

        public void ShowMessage(string message)
        {
            var script = $"alert('{message.Replace("'", "\\'")}),";
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