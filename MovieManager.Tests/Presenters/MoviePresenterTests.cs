using FluentAssertions;
using Moq;
using MovieManager.Tests.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebForms_MovieManager.Models;
using WebForms_MovieManager.Presenters;
using WebForms_MovieManager.Repositories;
using WebForms_MovieManager.Services;

namespace MovieManager.Tests.Presenters
{
    public class MoviePresenterTests : IDisposable
    {
        private readonly MockView _mockView;
        private readonly Mock<IMovieRepository> _mockRepository;
        private readonly Mock<IErrorLogger> _mockLogger;
        private readonly MoviePresenter _presenter;

        public MoviePresenterTests()
        {
            _mockView = new MockView();
            _mockRepository = new Mock<IMovieRepository>();
            _mockLogger = new Mock<IErrorLogger>();
            _presenter = new MoviePresenter(_mockView, _mockRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public void LoadMovies_Should_LoadAndBindMoviesToGrid()
        {
            //arrange
            var expectedMovies = new[]
            {
                new Movie{ Id=1, MovieTitle="Inception", Director="Nolan"},
                new Movie{ Id=2, MovieTitle="Interestelar", Director="Nolan"}
            };

            _mockRepository.Setup(r=>r.GetAllMovies()).Returns(expectedMovies);

            //act
            _mockView.RaiseLoadMovieEvent();

            //Assert
            _mockView.MoviesDataSource.Should().BeEquivalentTo(expectedMovies);
            _mockView.GridWasBound.Should().BeTrue();
            _mockRepository.Verify(v=>v.GetAllMovies(), Times.Once());
        }

        [Fact]
        public void AddMovie_WithValidData_ShouldAddMovieAndREfreshGrid()
        {
            //arrange
            var movieTitle = "The MAtrix";
            var director = "Wachowski";
            var releaseYear = "1999";
            var genre = "sci-fi";
            var rating = "8";

            _mockView.MovieTitle = movieTitle;
            _mockView.Director = director;
            _mockView.ReleaseYear = releaseYear;
            _mockView.Genre = genre;
            _mockView.Rating = rating;

            _mockRepository.Setup(m => m.AddMovie(It.IsAny<Movie>()));
            _mockRepository.Setup(m => m.GetAllMovies()).Returns(new[] { new Movie()});

            //act
            _mockView.RaiseAddMovieEvent();

            //assert
            _mockRepository
                .Verify(r => r.AddMovie(It.Is<Movie>(m =>
                                                m.MovieTitle == movieTitle && 
                                                m.Director == director &&
                                                m.ReleaseYear == int.Parse(releaseYear) &&
                                                m.Genre == genre &&
                                                m.Rating == int.Parse(rating)
                                            )));
            
            _mockView.GridWasBound.Should().BeTrue();
            _mockView.IsFormCleared.Should().BeTrue();
            _mockView.SuccessMessage.Should().Contain("successfully");
            _mockView.IsFormEditMode.Should().BeFalse();
        }

        [Fact]
        public void AddMovie_WithEmptyTitle_ShouldShowValidationError()
        {
            //arrange
            _mockView.MovieTitle= string.Empty;
            _mockView.Director = "director name";
            _mockView.ReleaseYear = "1999";
            _mockView.Genre = "Action";

            //act
            _mockView.RaiseAddMovieEvent();

            //assert
            _mockRepository.Verify(r => r.AddMovie(It.IsAny<Movie>()), Times.Never);
            _mockView.ValidationErrors.Should().Contain(e=>e.Contains("Movie title is required"));
            _mockView.GridWasBound.Should().BeFalse();
        }

        [Fact]
        public void AddMovie_WithInvalidYear_ShouldShowValidationError()
        {
            //arrange
            _mockView.MovieTitle = "1999";
            _mockView.Director = "director name";
            _mockView.ReleaseYear = "2030"; //future year
            _mockView.Genre = "Action";

            //act
            _mockView.RaiseAddMovieEvent();

            //assert
            _mockRepository.Verify(r => r.AddMovie(It.IsAny<Movie>()), Times.Never);
            _mockView.ValidationErrors.Should().Contain(e => e.Contains("cannot be in the future"));
            _mockView.GridWasBound.Should().BeFalse();
        }

        [Fact]
        public void AddMovie_WithRatingOutOfRange_ShouldShowValidationError()
        {
            //arrange
            _mockView.MovieTitle = "1999";
            _mockView.Director = "director name";
            _mockView.ReleaseYear = "2020"; 
            _mockView.Genre = "Action";
            _mockView.Rating = "15"; //above 10

            //act
            _mockView.RaiseAddMovieEvent();

            //assert
            _mockRepository.Verify(r => r.AddMovie(It.IsAny<Movie>()), Times.Never);
            _mockView.ValidationErrors.Should().Contain(e => e.Contains("Rating must be between 0 and 10"));
            _mockView.GridWasBound.Should().BeFalse();
        }

        [Fact]
        public void UpdateMovie_WithValidData_ShouldUpdateMovie()
        {
            //arrange
            var newMovieTitle = "Updated Title";
            _mockView.MovieId = "1";
            _mockView.MovieTitle = newMovieTitle;
            _mockView.Director = "director name";
            _mockView.ReleaseYear = "2020";
            _mockView.Genre = "Action";
            _mockView.Rating = "10";

            var existingMovie = new Movie {Id=1,MovieTitle="Old Title" };
            _mockRepository.Setup(r => r.GetMovieById(1)).Returns(existingMovie);
            _mockRepository.Setup(r => r.UpdateMovie(It.IsAny<Movie>()));
            _mockRepository.Setup(r => r.GetAllMovies()).Returns(new[] { new Movie() });

            //act
            _mockView.RaiseUpdateMovieEvent();

            //assert
            _mockRepository
               .Verify(r => r.UpdateMovie(It.Is<Movie>(m =>
                                               m.Id == 1 &&
                                               m.MovieTitle == newMovieTitle
                                           )), Times.Once);

            _mockView.SuccessMessage.Should().Contain("update successfully");
            _mockView.IsFormCleared.Should().BeTrue();
        }

        [Fact]
        public void UpdateMovie_WithNoMovieSelected_ShouldShowError()
        {
            //arrange
            var newMovieTitle = "Updated Title";
            _mockView.MovieId = "";
            _mockView.MovieTitle = newMovieTitle;

            //act
            _mockView.RaiseUpdateMovieEvent();

            //assert
            _mockRepository
               .Verify(r => r.UpdateMovie(It.IsAny<Movie>()), Times.Never);

            _mockView.ErrorMessage.Should().Contain("No movie selected");
        }

        public void Dispose()
        {
            _mockRepository.Reset();
        }

        [Fact]
        public void DeleteMovie_WithValidId_ShouldDeleteMovie()
        {
            //arrange            
            _mockView.MovieId = "1";
            _mockRepository.Setup(r => r.MovieExists(1)).Returns(true);
            _mockRepository.Setup(r => r.DeleteMovieById(1));
            _mockRepository.Setup(r => r.GetAllMovies()).Returns(new[] { new Movie()});

            //act
            _mockView.RaiseDeleteMovieEvent();

            //assert
            _mockRepository.Verify(r => r.DeleteMovieById(1), Times.Once);
            _mockView.SuccessMessage.Should().Contain("deleted successfully");
            _mockView.GridWasBound.Should().BeTrue();
            _mockView.IsFormCleared.Should().BeTrue();
        }

        [Fact]
        public void DeleteMovie_WithNoValidId_ShouldShowError()
        {
            //arrange            
            _mockView.MovieId = "999";
            _mockRepository.Setup(r => r.MovieExists(999)).Returns(false);

            //act
            _mockView.RaiseDeleteMovieEvent();

            //assert
            _mockRepository.Verify(r => r.DeleteMovieById(It.IsAny<int>()), Times.Never);
            _mockView.ErrorMessage.Should().Contain("Movie not found");
        }

        [Fact]
        public void EditMovie_WithValidId_ShouldPopulateFormAndSetEditMode()
        {
            //arrange
            _mockView.MovieId = "1";

            var movieTitle = "The MAtrix";
            var director = "Wachowski";
            var releaseYear = "1999";
            var genre = "sci-fi";
            var rating = "8";

            var movieEdit = new Movie
            {
                Id=1,
                MovieTitle=movieTitle,
                Director=director,
                ReleaseYear= int.Parse(releaseYear),
                Genre = genre,
                Rating = 8
            };


            _mockRepository.Setup(r => r.GetMovieById(1)).Returns(movieEdit);

            //act
            _mockView.RaiseEditMovieEvent();

            //assert
            _mockView.MovieTitle.Should().Be(movieTitle);
            _mockView.Director.Should().Be(director);
            _mockView.ReleaseYear.Should().Be(releaseYear);
            _mockView.Genre.Should().Be(genre);
            _mockView.Rating.Should().Be(rating);

            _mockView.IsFormEditMode.Should().BeTrue();
            _mockView.SuccessMessage.Should().Contain("Edit mode");
        }

        [Fact]
        public void EditMovie_WithinValidId_ShouldShowError()
        {
            //arrange
            _mockView.MovieId = "999";
            _mockRepository.Setup(r => r.GetMovieById(999)).Returns((Movie)null);


            //act
            _mockView.RaiseEditMovieEvent();

            //assert
            _mockView.IsFormEditMode.Should().BeFalse();
            _mockView.ErrorMessage.Should().Contain("Movie not found");
        }

        [Fact]
        public void ClearForm_ShouldResetFormAndExitEditMode()
        {
            //arrange
            _mockView.IsFormEditMode=true;


            //act
            _mockView.RaiseClearFormMovieEvent();

            //assert
            _mockView.IsFormCleared.Should().BeTrue();
            _mockView.IsFormEditMode.Should().BeFalse();
            _mockView.SuccessMessage.Should().Contain("Form Cleared");
        }

        [Theory]
        [InlineData("","Director","2020","Action","8","Movie title is required")]
        [InlineData("Name", "Director", "", "Action", "8", "Release year is required")]
        [InlineData("Name", "", "2020", "Action", "8", "Director name is required")]
        [InlineData("Name", "Director", "2020", "", "8", "Genre is required")]
        [InlineData("Name", "Director", "1800", "Genre", "8", "between 1888 and 2026")]
        [InlineData("Name", "Director", "2020", "Genre", "11", "between 0 and 10")]
        public void AddMovie_WithInvalidData_ShouldShowSpecifValidationErrors(
            string title,
            string director,
            string year,
            string genre,
            string rating,
            string expectedError)
        {
            //arrange
            _mockView.MovieTitle = title;
            _mockView.Director = director;
            _mockView.ReleaseYear = year;
            _mockView.Genre = genre;
            _mockView.Genre = genre;
            _mockView.Rating= rating;            

            //act
            _mockView.RaiseAddMovieEvent();

            //assert
            _mockRepository.Verify(r => r.AddMovie(It.IsAny<Movie>()), Times.Never);
            _mockView.ValidationErrors.Should().Contain(e => e.Contains(expectedError));
            _mockView.GridWasBound.Should().BeFalse();
        }
    }
}
