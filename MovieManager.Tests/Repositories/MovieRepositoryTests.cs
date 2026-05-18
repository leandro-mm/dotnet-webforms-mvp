using FluentAssertions;
using System;
using System.Linq;
using WebForms_MovieManager.Models;
using WebForms_MovieManager.Repositories;


namespace MovieManager.Tests.Repositories
{
    public class MovieRepositoryTests
    {
        private readonly MovieRepository _repository;

        public MovieRepositoryTests()
        {
            _repository = new MovieRepository();
        }

        [Fact]
        public void GetAllMovies_ShouldReturnAllMovies()
        {
            //Act
            var movies = _repository.GetAllMovies();

            //Assert
            movies.Should().NotBeNull();
            movies.Should().HaveCountGreaterThan(0);
        }


        [Fact]
        public void AddMovie_ShouldIncreaseMovieCount()
        {
            //Arrange
            var initialCount = _repository.GetAllMovies().Count();

            var newMovie = new Movie
            {
                MovieTitle = "Test Director",
                Director ="Test Director",
                ReleaseYear = DateTime.Now.Year,
                Genre = "Test Genre",
                Rating =7
            };

            //Act
            _repository.AddMovie(newMovie);
            var finalCount = _repository.GetAllMovies().Count();

            //Assert
            finalCount.Should().Be(initialCount + 1);
            newMovie.Id.Should().BeGreaterThan(0);
        }

        [Fact]
        public void GetMovieById_WithValidId_ShouldReturnCorrectMovie()
        {
            //Arrange
            var movies = _repository.GetAllMovies();
            var firstMovie = movies.First();

            //Act
            var retrievedMovie = _repository.GetMovieById(firstMovie.Id);

            //Assert
            retrievedMovie.Should().NotBeNull();
            retrievedMovie.Id.Should().Be(firstMovie.Id);
            retrievedMovie.MovieTitle.Should().Be(firstMovie.MovieTitle);
        }

        [Fact]
        public void GetMovieById_WithInValidId_ShouldReturnCorrectMovie()
        {            

            //Act
            var retrievedMovie = _repository.GetMovieById(-1);

            //Assert
            retrievedMovie.Should().BeNull();
        }

        [Fact]
        public void UpdateMovie_ShouldModifyExistingtMovie()
        {
            //Arrange            
            var movies = _repository.GetAllMovies();
            var movieToUpdate = movies.First();
            var originalTitle = movieToUpdate.MovieTitle;
            movieToUpdate.MovieTitle = "Updated Title";


            //Act
            _repository.UpdateMovie(movieToUpdate);
            var updatedMovie = _repository.GetMovieById(movieToUpdate.Id);

            //Assert
            updatedMovie.MovieTitle.Should().Be("Updated Title");

            // CLeanup - revert change
            movieToUpdate.MovieTitle = originalTitle;
            _repository.UpdateMovie(movieToUpdate);
        }

        [Fact]
        public void UpdateMovie_WithNoExistingId_ShouldThrowException()
        {
            //Arrange            
            var nonExistingMovie = new Movie
            {
                Id = -1,
                MovieTitle = "non existing",
                Director = "non existing",
                ReleaseYear = DateTime.Now.Year,
                Genre = "non existing",
                Rating = 7
            };

            //Act
            var exception = Record.Exception(() => _repository.UpdateMovie(nonExistingMovie));

            //Assert
            exception.Should().BeNull();
        }

        [Fact]
        public void DeleteMovie_WithValidId_ShouldRemoveMovie()
        {
            //Arrange            
            var newMovie = new Movie
            {                
                MovieTitle = "non existing",
                Director = "non existing",
                ReleaseYear = DateTime.Now.Year,
                Genre = "non existing",
                Rating = 7
            };

            _repository.AddMovie(newMovie);
            var movieId = newMovie.Id;
            var initialCount = _repository.GetAllMovies().Count();

            //Act
            _repository.DeleteMovieById(movieId);
            var finalCount =_repository.GetAllMovies().Count();
            _repository.DeleteMovieById(movieId);

            //Assert
            finalCount.Should().Be(initialCount - 1);            
        }

        [Fact]
        public void DeleteMovie_WithInValidId_ShouldNotThrow()
        {
            //Act            
            var exception = Record.Exception(() => _repository.DeleteMovieById(-1));



            //Assert
            exception.Should().BeNull();
        }


        [Fact]
        public void MovieExists_WithValidId_ShouldReturnTrue()
        {
            //Arrange            
            var movies = _repository.GetAllMovies();
            var existingId = movies.First().Id;


            //Act
            var exists = _repository.MovieExists(existingId);

            //Assert
            exists.Should().BeTrue();
        }

        [Fact]
        public void MovieExists_WithInValidId_ShouldReturnFalse()
        {            
            //Act
            var exists = _repository.MovieExists(-1);

            //Assert
            exists.Should().BeFalse();
        }

        [Fact]
        public void GetAllMovies_ShouldReturnOrderedByCreateDateDescending()
        {
            //Arrange
            var initialCount = _repository.GetAllMovies().Count();

            var oldMovie = new Movie
            {
                MovieTitle = "Old Movie",
                Director = "Old",
                ReleaseYear = DateTime.Now.Year-(int.Parse(DateTime.Now.ToString("yy"))),
                Genre = "old Genre",
                Rating = 7
            };

            var newMovie = new Movie
            {
                MovieTitle = "new Movie",
                Director = "new",
                ReleaseYear = DateTime.Now.Year,
                Genre = "new Genre",
                Rating = 7
            };

            _repository.AddMovie(oldMovie);
            System.Threading.Thread.Sleep(10); // Ensure different timestamps
            _repository.AddMovie(newMovie);

            //Act
            _repository.AddMovie(newMovie);
            var movies = _repository.GetAllMovies().ToList();

            //Assert
            movies.First().MovieTitle.Should().Be("new Movie");
        }
    }
}
