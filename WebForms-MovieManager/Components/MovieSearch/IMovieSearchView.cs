
using System;
using System.Collections.Generic;

using System.Web;
using WebForms_MovieManager.Components.Base;
using WebForms_MovieManager.Models;

namespace WebForms_MovieManager.Components.MovieSearch
{
    public interface IMovieSearchView : IComponentView<IEnumerable<Movie>>
    {
        #region Properties
        string SearchTerm { get; set; }
        string SelectedGenre { get; set; }
        int? SelectedYear { get; set; }
        double? MinimumRating { get; set; }
        #endregion

        #region DDL Options
        IEnumerable<string> Genres { get; set; }
        IEnumerable<int> AvailablYears { get; set; }
        #endregion

        #region Events
        event EventHandler SearchTriggered;
        event EventHandler SearchCleared;
        event EventHandler FilteredChange;
        #endregion

        #region Methods
        void ShowSearchResult(int resultCount);
        void SetAdvancedSearchVisible(bool visible);
        void HighlightSearch(string term);
        #endregion



    }
}