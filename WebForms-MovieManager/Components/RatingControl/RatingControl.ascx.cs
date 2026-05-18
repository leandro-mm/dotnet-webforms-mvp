
using Microsoft.Ajax.Utilities;
using System;

using System.Web.UI;
using WebForms_MovieManager.Components.Communication;
using WebForms_MovieManager.Repositories;
using WebForms_MovieManager.Services;

namespace WebForms_MovieManager.Components.RatingControl
{
    public partial class RatingControl : UserControl, IRatingControlView
    {
        private RatingControlPresenter _presenter;
        
        public int MovieId 
        {
            get => ViewState["MovieId"] != null ? (int)ViewState["MovieId"] : 0;
            set => ViewState["MovieId"] = value;
        }
        public double CurrentRate 
        {
            get => string.IsNullOrEmpty(hdnRating.Value) ? 0 : Convert.ToDouble(hdnRating.Value);
            set
            {
                hdnRating.Value = value.ToString();
                DisplayRating(value);
            }
        }
        public bool IsReadOnly 
        {
            get => ViewState["IsReadonly"] != null && (bool)ViewState["IsReadonly"];
            set
            {
                ViewState["IsReadonly"] = value;
                btnSaveRating.Visible = !value;
                btnClearRating.Visible = value;
            }
        }
        public RatingData DataSource { get;set; }

        public string ComponentId => $"RatingControl_{MovieId}_{ClientID}";

        public event EventHandler<RatingChangedEventArgs> RatingChanged;
        public event EventHandler RatingSaved;
        public event EventHandler ComponentLoaded;
        public event EventHandler ComponentDatachanged;

        public void BindData()
        {
            if (DataSource != null)
            {
                DisplayRating(DataSource.AverageRating);
                ShowRatingSummary(DataSource.TotalVotes, DataSource.AverageRating);
            }
        }

        private void ShowRatingSummary(int totalVotes, double averageRating)
        {
            lblVotes.Text = $"({totalVotes} votes)";
            DisplayRating(averageRating);
        }

        public void ClearData()
        {
            CurrentRate = 0;
            lblRatingDisplay.Text = "";
            lblVotes.Text = "";
        }

        public void DisplayRating(double rating)
        {
            lblRatingDisplay.Text = $"{rating:F1} *";
            // JavaScript WiLL handLe star dispLay
            var script = $"document.querySelector('.rating-control[data-movie-id=\"{MovieId}\"] .stars')?._setRating?.({rating});";
            ScriptManager.RegisterStartupScript(this, GetType(), "SetRating", script, true);
        }

        public void SetLoadingState(bool isLoading)
        {
            loadingOverlay.Visible = isLoading;
        }

        public void ShowError(string message)
        {
            var script = $"alert('Error: {message.Replace("'", "\\'")}');";
            ScriptManager.RegisterStartupScript(this, GetType(), "RatingError", script, true);
        }

        public void ShowMessage(string message)
        {
            var script = $"alert({message.Replace("'", "\\'")});";
            ScriptManager.RegisterStartupScript(this, GetType(), "RatingMessage", script, true);
        }

        public void ShowRatingSumary(int totalVotes, double averageRating)
        {
            lblVotes.Text = $"({totalVotes} votes)";
            DisplayRating(averageRating);
        }

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
            if (_presenter == null) { 
                var repository = new MovieRepository();
                var logger = new ErrorLogger();
                _presenter = new RatingControlPresenter(this, repository, logger);
                _presenter.Initialize();
            }
        }

        protected void btnSaveRating_Click(object sender, EventArgs e)
        {
            RatingSaved?.Invoke(this, EventArgs.Empty);

            // Publish the rating updated event
            var ratingEvent = new RatingUpdatedEvent
            {
                SourceComponentId = this.ComponentId,
                MovieId = this.MovieId,
                NewRating = this.CurrentRate
            };

            EventAggregatorProvider.Instance.Publish(ratingEvent);
        }

        protected void btnClearRating_Click(object sender, EventArgs e)
        {
            CurrentRate = 0;
            RatingChanged?.Invoke(this, new RatingChangedEventArgs(MovieId, 0, CurrentRate));
        }

        // Method caLLed from Javascript via PageMethods
        [System.Web.Services.WebMethod]
        public static void Ratingchanged(int movieId, double rating)
        {
            // HandLe rating change from cLient
        }
    }
}