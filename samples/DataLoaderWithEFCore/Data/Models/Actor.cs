using System;
using System.ComponentModel.DataAnnotations;

namespace DataLoaderWithEFCore.Data.Models
{
    public class Actor
    {
        [Key]
        public Guid Id { get; set; }

        [MaxLength(200), Required]
        public string Name { get; set; }

        [MaxLength(2), Required]
        public string CountryCode { get; set; }

        [Required]
        public Guid MovieId { get; set; }
    }
}
