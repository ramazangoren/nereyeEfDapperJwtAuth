using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.DTOs.ProductDto
{
    public class UpdateProductDto
    {
        [Required]
        [StringLength(255)]
        [Column(TypeName = "nvarchar(255)")]
        public string ProductName { get; set; } = "";

        [Required]
        [Column(TypeName = "nvarchar(MAX)")]
        public string ProductPhoto { get; set; } = "";

        [Required]
        [StringLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string Category { get; set; } = "";

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal ProductPrice { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(MAX)")]
        public string ProductExplanation { get; set; } = "";

        [Required]
        [StringLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        public string ProductPreparationTime { get; set; } = "";
    }
}
