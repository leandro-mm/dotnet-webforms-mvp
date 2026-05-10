using System.Collections.Generic;
using WebForms_MovieManager.Models;

namespace WebForms_MovieManager.Repositories
{
    public interface IMovieRepository
    {
        IEnumerable<Movie> GetAllMovies();
        Movie GetMovieById(int id);
        void AddMovie(Movie movie);
        void UpdateMovie(Movie movie);
        void DeleteMovieById(int id);
        void DeleteAllMovies();
        bool MovieExists(int id);
    }
}
