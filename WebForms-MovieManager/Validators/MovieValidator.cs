using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using WebForms_MovieManager.Models;

namespace WebForms_MovieManager.Validators
{
    public class MovieValidator
    {
        public static List<string> ValidateMovie(Movie movie)
        {
            var validationContext = new ValidationContext(movie);

            var errors = new List<string>();
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator
                            .TryValidateObject(
                                movie,
                                validationContext,
                                validationResults, true
                            );

            if (!isValid)
            {
                errors.AddRange(validationResults
                                .Select(x => x.ErrorMessage));
            }

            if (movie.ReleaseYear > DateTime.Now.Year)
            {
                errors.Add(string.Format(
                    "Release year cannot be in the future: actual year (0), movie year (1)",
                    DateTime.Now.Year,
                    movie.ReleaseYear.ToString()));
            }

            return errors;
        }
    }
}