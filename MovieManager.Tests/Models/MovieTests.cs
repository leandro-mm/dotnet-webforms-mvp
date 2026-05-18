using FluentAssertions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using WebForms_MovieManager.Models;

namespace MovieManager.Tests.Models
{
    public class MovieTests
    {

        [Fact]
        public void Movie_DefaultConstructor_ShouldSetDefaultValues()
        {
            //Act
            var movie = new Movie();

            // Assert
            movie.Rating.Should().Be(0);
            movie.CreatedDate.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void Movie_Properties_ShouldBeSetAndGetCorrectly()
        {
            //Arrange            

            var movieTitle = "Test Movie";
            var director = "Test Director";
            var releaseYear = DateTime.Now.Year;
            var genre = "Test Genre";
            var rating = 7;
            var Id = 1;

            var movie = new Movie
            {
                Id= Id,
                MovieTitle = movieTitle,
                Director = director,
                ReleaseYear = releaseYear,
                Genre = genre,
                Rating = rating
            };

            //Assert
            movie.Id.Should().Be(Id);
            movie.MovieTitle.Should().Be(movieTitle);
            movie.Director.Should().Be(director);
            movie.ReleaseYear.Should().Be(releaseYear);
            movie.Genre.Should().Be(genre);
            movie.Rating.Should().Be(rating);
        }

        [Fact]
        public void Movie_Title_ValidationAttributeExists()
        {
            //Arrange            
            var movie = new Movie
            {
                MovieTitle = ""
            };

            var propertyName = nameof(movie.MovieTitle);

            var context = new ValidationContext(movie,null,null);
            var results = new List<ValidationResult>();

            //Act
            var isvalid = Validator.TryValidateObject(movie, context, results, true);

            //Assert
            isvalid.Should().BeFalse();
            results.Should().Contain(r => r.MemberNames.Contains(propertyName));
        }

        [Fact]
        public void ReleaseYear_RangeValidation_Throws()
        {
            //Arrange            
            var movie = new Movie
            {
                ReleaseYear = DateTime.Now.Year-500
            };

            var propertyName = nameof(movie.ReleaseYear);
            var context = new ValidationContext(movie, null, null);
            var results = new List<ValidationResult>();

            //Act
            var isvalid = Validator.TryValidateObject(movie, context, results, true);

            //Assert
            isvalid.Should().BeFalse();
            results.Should().Contain(r => r.MemberNames.Contains(propertyName));
        }

        [Fact]
        public void MovieRating_RangeValidation_Works()
        {
            //Arrange            
            var movie = new Movie
            {
                Rating=15
            };

            var propertyName = nameof(movie.Rating);
            var context = new ValidationContext(movie, null, null);
            var results = new List<ValidationResult>();

            //Act
            var isvalid = Validator.TryValidateObject(movie, context, results, true);

            //Assert
            isvalid.Should().BeFalse();
            results.Should().Contain(r => r.MemberNames.Contains(propertyName));
        }

        [Fact]
        public void MovieCreateDate_IsSetOnConstructor()
        {
            //Arrange            
            var movie1 = new Movie();
            System.Threading.Thread.Sleep(100);
            var movie2 = new Movie();

            //Assert
            movie2.CreatedDate.Should().BeAfter(movie1.CreatedDate);
        }
    }
}
