using System;
using System.ComponentModel.DataAnnotations;

namespace DataLoaderWithEFCore.Data.Models
{
    public class Movie
    {
        [Key]
        public Guid Id { get; set; }

        [MaxLength(500), Required]
        public string Title { get; set; }

        [MaxLength(50), Required]
        public string Genre { get; set; }

        [Required]
        public DateTime ReleaseDateUtc { get; set; }
    }
}
