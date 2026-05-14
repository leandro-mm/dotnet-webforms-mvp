using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebForms_MovieManager.Models;

namespace WebForms_MovieManager.Components.MovieSearch
{
    public class MovieSearchModel
    {
        public string SearchTerm { get; set; }
        public string Genre { get; set; }
        public int? Year { get; set; }
        public double? MinRating { get; set; }
        public IEnumerable<Movie> Results { get; set; }
        public int TotalResults { get; set; }
        public bool HasResults => TotalResults > 0;
    }
}