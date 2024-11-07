using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace api.DTOs.ReviewsDto
{
    public class AddReviewDto
    {
        [Required]
        [Column(TypeName = "nvarchar(MAX)")]
        public string Comment { get; set; } = "";

        [Column(TypeName = "nvarchar(MAX)")]
        public string? ReviewPhoto { get; set; } 

        [Range(1, 5)]
        public int Rating { get; set; }
    }
}
