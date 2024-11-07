using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [ForeignKey("Restaurant")]
        public int RestaurantId { get; set; }

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

        [DataType(DataType.DateTime)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [DataType(DataType.DateTime)]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
