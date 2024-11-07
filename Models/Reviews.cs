using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class Reviews
    {
        [Key]
        public int ReviewId { get; set; }

        [ForeignKey("Restaurant")]
        public int RestaurantId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(MAX)")]
        public string Comment { get; set; } = ""; // Changed from ProductName to Review

        [Column(TypeName = "nvarchar(MAX)")]
        public string? ReviewPhoto { get; set; } // Made nullable to match SQL

        [Range(1, 5)]
        public int Rating { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
