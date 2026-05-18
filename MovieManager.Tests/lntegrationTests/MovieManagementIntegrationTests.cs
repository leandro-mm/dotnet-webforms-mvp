using FluentAssertions;
using MovieManager.Tests.Mocks;
using System;

using System.Linq;

using WebForms_MovieManager.Presenters;
using WebForms_MovieManager.Repositories;

namespace MovieManager.Tests.lntegrationTests
{
    public class MovieManagementIntegrationTests
    {
        [Fact]
        public void CompleteMovieWorkflow_ShouldWorkCorrectly()
        {
            //Arrange
            var view = new MockView();
            var repository = new MovieRepository();
            var presenter = new MoviePresenter(view, repository);

            view.MovieTitle = "Integration Test Movie";
            view.Director = "Test Director";
            view.ReleaseYear = DateTime.Now.Year.ToString();
            view.Genre = "Test Genre";
            view.Rating = "9";
            view.RaiseAddMovieEvent();

            //Assert - Movie was added
            view.SuccessMessage.Should().Contain("successfully");

            //Act - Load movies
            view.RaiseLoadMovieEvent();

            //Assert - Movie appears in grid
            view.MoviesDataSource.Should().Contain(m => m.MovieTitle == "Integration Test Movie");
            var addedMovie = view.MoviesDataSource.First(m => m.MovieTitle == "Integration Test Movie");

            //Act - Edit the movie
            view.MovieId = addedMovie.Id.ToString();
            view.RaiseEditMovieEvent();

            //Assert - Form popuLated for editing
            view.MovieTitle.Should().Be("Integration Test Movie");
            view.IsFormEditMode.Should().BeTrue();

            //Act - Update the movie
            view.MovieTitle = "Updated Integration Movie";
            view.Rating = "9";
            view.RaiseUpdateMovieEvent();

            //Assert - Movie was updated
            view.SuccessMessage.Should().Contain("update");
            view.RaiseLoadMovieEvent();
            view.MoviesDataSource.Should().Contain(m => m.MovieTitle == "Updated Integration Movie");

            view.MovieId = addedMovie.Id.ToString();
            view.RaiseDeleteMovieEvent();

            //Assert - Movie was deLeted
            view.SuccessMessage.Should().Contain("deleted");
            view.RaiseLoadMovieEvent();
            view.MoviesDataSource.Should().NotContain(m => m.Id == addedMovie.Id);

        }

        [Fact]
        public void ValidationErrors_ShouldPreventInvalidOperations()
        {
            //Arrange
            var view = new MockView();
            var repository = new MovieRepository();
            var presenter = new MoviePresenter(view, repository);
            var initialCount = repository.GetAllMovies().Count();

            view.MovieTitle = "";
            view.Director = "Test Director";
            view.ReleaseYear = DateTime.Now.Year.ToString();
            view.Genre = "Test Genre";
            view.Rating = "9";
            view.RaiseAddMovieEvent();

            //Assert - Movie was added
            view.ValidationErrors.Should().NotBeEmpty();
            repository.GetAllMovies().Count().Should().Be(initialCount);
        }
    }
}
