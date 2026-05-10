using System;
using System.ComponentModel.DataAnnotations;

namespace WebForms_MovieManager.Models
{
    public class Movie
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Movie title is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage="Title Must be between 2 and 100 characters")]
        public string MovieTitle { get; set; }

        [Required(ErrorMessage = "Director name is required")]
        public string Director { get; set; }

        [Required(ErrorMessage = "Release year is required")]
        [Range(1888, 2026, ErrorMessage = "Release year must be between 1888 and 2026")] 
        public int ReleaseYear { get; set; }
        [Required(ErrorMessage = "Genre is required")]
        public string Genre { get; set; }

        [Range(0, 10, ErrorMessage = "Rating must be between 0 and 10")]
        public int Rating { get; set; }
        public DateTime CreatedDate { get; set; }

        public Movie()
        {
            CreatedDate = DateTime.Now;
            Rating = 0;
        }
    }
}