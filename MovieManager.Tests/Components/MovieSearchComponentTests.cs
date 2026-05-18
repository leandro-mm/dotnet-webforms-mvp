using Moq;
using MovieManager.Tests.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebForms_MovieManager.Components.MovieSearch;
using WebForms_MovieManager.Models;
using WebForms_MovieManager.Repositories;

namespace MovieManager.Tests.Components
{
    public class MovieSearchComponentTests
    {
        [Fact]
        public void Search_WithVa1idTerm_ShouldReturnMatchingMovies()
        {
            //Arrange
            var mockView = new Mock<IMovieSearchView>();
            var mockRepository = new Mock<IMovieRepository>();
            string textInput = "Inception";

            var movies = new[]
            {
                new Movie { Id = 1, MovieTitle= "Inception", Director = "Nolan" },
                new Movie { Id = 2, MovieTitle= "Interestelar", Director = "Nolan" },
                new Movie { Id = 3, MovieTitle= "The Dark Knight", Director = "Nolan" }
            };

            mockRepository.Setup(r => r.GetAllMovies()).Returns(movies);
            mockView.Setup(v => v.SearchTerm).Returns(textInput);

            // Track what gets set on the view
            int shownResultCount = 0;
            mockView.Setup(v => v.ShowSearchResult(It.IsAny<int>()))
                    .Callback<int>(count => shownResultCount = count);

            var presenter = new MovieSearchPresenter(mockView.Object, mockRepository.Object);

            //Act
            mockView.Raise(v => v.SearchTriggered += null, EventArgs.Empty);

            // Assert
            Assert.Equal(1, shownResultCount);
            mockView.Verify(v => v.HighlightSearch(textInput), Times.Once);

        }

        [Fact]
        public void Search_WithGenreFilter_ShouldReturnFilteredMovies()
        {
            //Arrange
            var mockView = new Mock<IMovieSearchView>();
            var mockRepository = new Mock<IMovieRepository>();
            string textInput = "Action";

            var movies = new[]
            {
                new Movie { Id = 1, MovieTitle= "Inception", Director = "Nolan", Genre="Action" },
                new Movie { Id = 2, MovieTitle= "Interestelar", Director = "Nolan" , Genre="Drama"},
                new Movie { Id = 3, MovieTitle= "The Dark Knight", Director = "Nolan", Genre="Action" }
            };

            mockRepository.Setup(r => r.GetAllMovies()).Returns(movies);
            mockView.Setup(v => v.SelectedGenre).Returns(textInput);

            // Track what gets set on the view
            int shownResultCount = 0;
            mockView.Setup(v => v.ShowSearchResult(It.IsAny<int>()))
                    .Callback<int>(count => shownResultCount = count);

            var presenter = new MovieSearchPresenter(mockView.Object, mockRepository.Object);

            //Act
            mockView.Raise(v => v.SearchTriggered += null, EventArgs.Empty);

            // Assert
            Assert.Equal(2, shownResultCount);

            // HighlightSearch should NOT be called when there's no search term
            mockView.Verify(v => v.HighlightSearch(It.IsAny<string>()), Times.Never);

        }
    }
}
