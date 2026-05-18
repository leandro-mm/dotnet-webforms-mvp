using System;

using WebForms_MovieManager.Models;

namespace MovieManager.Tests
{
    public static class TestHelper
    {
        public static Movie CreateValidMovie()
        {
            return new Movie
            {
                MovieTitle = $"Test Movie {Guid.NewGuid()}",
                Director = "Test Director",
                ReleaseYear = DateTime.Now.Year,
                Genre = "Test Genre",
                Rating = 7
            };
        }

        public static Movie CreateInValidMovie()
        {
            return new Movie
            {
                MovieTitle = "",
                Director = "",
                ReleaseYear = DateTime.Now.Year-500,
                Genre = "",
                Rating = 700
            };
        }

        public static bool AreMovieEqual(Movie m1, Movie m2)
        {
            return m1.MovieTitle == m2.MovieTitle &&
                m1.Director == m2.Director &&
                m1.ReleaseYear == m2.ReleaseYear &&
                m1.Genre == m2.Genre &&
                Math.Abs(m1.Rating - m2.Rating) < 0.01;
        }
    }
}
