using System;
using System.Collections.Generic;
using System.Linq;
using WebForms_MovieManager.Models;

namespace WebForms_MovieManager.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        private static List<Movie> _movies = new List<Movie>();
        private static int _nextId = 1;

        static MovieRepository()
        {
            _movies.Add(new Movie 
            {  
                Id = _nextId++,
                MovieTitle = "Inception",
                Director = "Christopher Nolan",
                ReleaseYear = 2010,
                Genre = "Sci-Fi",
                Rating = 8
            });
        }
        public void AddMovie(Movie movie)
        {
            movie.Id = _nextId++;
            movie.CreatedDate = DateTime.Now;
            _movies.Add(movie);
        }

        public void DeleteAllMovies()
        {
            _movies.Clear();
        }

        public void DeleteMovieById(int id)
        {
            var movie = GetMovieById(id);
            if (movie != null)
            {
                _movies.Remove(movie);
            }
        }

        public IEnumerable<Movie> GetAllMovies()
        {
           return _movies.OrderByDescending(m=>m.CreatedDate);
        }

        public Movie GetMovieById(int id)
        {
            return _movies.FirstOrDefault(m=>m.Id == id);
        }

        public bool MovieExists(int id)
        {
            return _movies.Any(m=>m.Id == id);
        }

        public void UpdateMovie(Movie movie)
        {
            var existingMovie = GetMovieById(movie.Id);
            if (existingMovie != null)
            {
                existingMovie.MovieTitle = movie.MovieTitle;
                existingMovie.Director = movie.Director;
                existingMovie.ReleaseYear = movie.ReleaseYear;
                existingMovie.Genre = movie.Genre;
                existingMovie.Rating = movie.Rating;                
            }
        }
    }
}