using FluentAssertions;
using System;
using System.Linq;
using WebForms_MovieManager.Models;
using WebForms_MovieManager.Validators;

namespace MovieManager.Tests.Validators
{
    public  class MovieValidatorTests
    {
        [Fact]
        public void ValidateMovie_WithVa1idMovie_ShouldReturnNoErrors()
        {
            //Arrange
            var movie = new Movie
            {
                MovieTitle ="The GodFather",
                Director ="Francis Ford Coppola",
                ReleaseYear = 1972,
                Genre = "Drama",
                Rating = 9
            };

            var errors = MovieValidator.ValidateMovie(movie);
            //Assert
            errors.Should().BeEmpty();
        }

        [Fact]
        public void ValidateMovie_WithEmptyTitle_ShouldReturnError()
        {
            //Arrange
            var movie = new Movie
            {
                MovieTitle = "",
                Director = "Francis Ford Coppola",
                ReleaseYear = 1972,
                Genre = "Drama",
                Rating = 9
            };

            var errors = MovieValidator.ValidateMovie(movie);
            //Assert
            errors.Should().Contain(e => e.Contains("Movie title is required"));
        }

        [Fact]
        public void ValidateMovie_WithTitleTooLong_ShouldReturnError()
        {
            //Arrange
            var movie = new Movie
            {
                MovieTitle = new string('A',101),
                Director = "Francis Ford Coppola",
                ReleaseYear = 1972,
                Genre = "Drama",
                Rating = 9
            };

            var errors = MovieValidator.ValidateMovie(movie);
            //Assert
            errors.Should().Contain(e => e.Contains("between 2 and 100 characters"));
        }

        public void ValidateMovie_WithTitleTooShort_ShouldReturnError()
        {
            //Arrange
            var movie = new Movie
            {
                MovieTitle = new string('A', 1),
                Director = "Francis Ford Coppola",
                ReleaseYear = 1972,
                Genre = "Drama",
                Rating = 9
            };

            var errors = MovieValidator.ValidateMovie(movie);
            //Assert
            errors.Should().Contain(e => e.Contains("between 2 and 100 characters"));
        }

        [Fact]
        public void ValidateMovie_WithFutureReleaseYear_ShouldReturnError()
        {
            //Arrange
            var movie = new Movie
            {
                MovieTitle = "Future Movie",
                Director = "Francis Ford Coppola",
                ReleaseYear = DateTime.Now.Year + 1,
                Genre = "Drama",
                Rating = 9
            };

            var errors = MovieValidator.ValidateMovie(movie);
            //Assert
            errors.Should().Contain(e => e.Contains("cannot be in the future"));
        }

        [Fact]
        public void ValidateMovie_WithOutOfRangeYear_ShouldReturnError()
        {
            //Arrange
            var movie = new Movie
            {
                MovieTitle = "Future Movie",
                Director = "Francis Ford Coppola",
                ReleaseYear = DateTime.Now.Year - 500,
                Genre = "Drama",
                Rating = 9
            };

            var errors = MovieValidator.ValidateMovie(movie);
            //Assert
            errors.Should().Contain(e => e.Contains($"between 1888 and {DateTime.Now.Year}"));
        }

        [Fact]
        public void ValidateMovie_WithEmptyGenre_ShouldReturnError()
        {
            //Arrange
            var movie = new Movie
            {
                MovieTitle = "Future Movie",
                Director = "Francis Ford Coppola",
                ReleaseYear = DateTime.Now.Year,
                Genre = "",
                Rating = 9
            };

            var errors = MovieValidator.ValidateMovie(movie);
            //Assert
            errors.Should().Contain(e => e.Contains("Genre is required"));
        }

        [Fact]
        public void ValidateMovie_WithRatingBellowZero_ShouldReturnError()
        {
            //Arrange
            var movie = new Movie
            {
                MovieTitle = "Future Movie",
                Director = "Francis Ford Coppola",
                ReleaseYear = DateTime.Now.Year,
                Genre = "",
                Rating = -1
            };

            var errors = MovieValidator.ValidateMovie(movie);
            //Assert
            errors.Should().Contain(e => e.Contains("between 0 and 10"));
        }

        [Fact]
        public void ValidateMovie_WithMultipleErrors_ShouldReturnError()
        {
            //Arrange
            var movie = new Movie
            {
                MovieTitle = "",
                Director = "",
                ReleaseYear = DateTime.Now.Year-500,
                Genre = "",
                Rating = -1
            };

            var errors = MovieValidator.ValidateMovie(movie);
            //Assert
            errors.Should().Contain(e => e.Contains("between 0 and 10"));
            errors.Should().HaveCountGreaterThanOrEqualTo(4);
            errors.Should().Contain(e => e.Contains("Movie title is required"));
            errors.Should().Contain(e => e.Contains("Director name is required"));
            errors.Should().Contain(e => e.Contains($"Release year must be between 1888 and {DateTime.Now.Year}"));
            errors.Should().Contain(e => e.Contains("Genre is required"));
        }

    }
}
