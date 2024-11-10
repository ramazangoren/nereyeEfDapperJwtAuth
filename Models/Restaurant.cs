using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    [Table("Restaurants")]
    public class Restaurant
    {
        [Key]
        public int RestaurantId { get; set; }

        // Foreign key to Users table
        [ForeignKey("User")]
        public int UserId { get; set; }

        // [Required]
        [StringLength(255)]
        [Column(TypeName = "nvarchar(255)")]
        public string RestaurantName { get; set; } = "";

        public int? RestaurantCode { get; set; }

        [Required]
        [StringLength(255)]
        [Column(TypeName = "nvarchar(255)")]
        public string FullAddress { get; set; } = "";

        [Required]
        [StringLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string Latitude { get; set; } = "";

        [Required]
        [StringLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string Longitude { get; set; } = "";

        // [Required]
        [StringLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string EstablishmentType { get; set; } = "";

        [Required]
        [Phone]
        [StringLength(20)]
        [Column(TypeName = "nvarchar(20)")]
        public string Phone { get; set; } = "";

        [Required]
        [Column(TypeName = "nvarchar(MAX)")]
        public string ExtraInfo { get; set; } = "";

        [Required]
        [Column(TypeName = "nvarchar(MAX)")]
        public string AboutUs { get; set; } = "";

        // Boolean fields
        public bool CreditCard { get; set; }
        public bool Cash { get; set; }
        public bool FoodCards { get; set; }
        public bool AppleGooglePay { get; set; }
        public bool OutdoorSitting { get; set; }
        public bool PetsAllowed { get; set; }
        public bool Alcohol { get; set; }
        public bool Parking { get; set; }
        public bool OffersDelivery { get; set; }
        public bool OffersTakeout { get; set; }
        public bool GoodForGroups { get; set; }
        public bool GoodForKids { get; set; }
        public bool FullBar { get; set; }
        public bool TakesReservation { get; set; }
        public bool WaiterService { get; set; }
        public bool SelfService { get; set; }
        public bool HasTV { get; set; }
        public bool FreeWifi { get; set; }
        public bool StreetParking { get; set; }
        public bool BeerAndWineOnly { get; set; }
        public bool Italian { get; set; }
        public bool Hookah { get; set; }
        public bool Burger { get; set; }
        public bool HotDogs { get; set; }
        public bool FastFood { get; set; }
        public bool Breakfast { get; set; }
        public bool Doner { get; set; }
        public bool HalalFood { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(MAX)")]
        public string ImageUrls { get; set; } = "";

        // Opening and closing times for each day
        [Required]
        public TimeSpan MondayOpens { get; set; } = TimeSpan.Zero;

        [Required]
        public TimeSpan MondayCloses { get; set; } = TimeSpan.Zero;

        [Required]
        public TimeSpan TuesdayOpens { get; set; } = TimeSpan.Zero;

        [Required]
        public TimeSpan TuesdayCloses { get; set; } = TimeSpan.Zero;

        public TimeSpan WednesdayOpens { get; set; } = TimeSpan.Zero;

        public TimeSpan WednesdayCloses { get; set; } = TimeSpan.Zero;

        [Required]
        public TimeSpan ThursdayOpens { get; set; } = TimeSpan.Zero;

        [Required]
        public TimeSpan ThursdayCloses { get; set; } = TimeSpan.Zero;

        [Required]
        public TimeSpan FridayOpens { get; set; } = TimeSpan.Zero;

        [Required]
        public TimeSpan FridayCloses { get; set; } = TimeSpan.Zero;

        [Required]
        public TimeSpan SaturdayOpens { get; set; } = TimeSpan.Zero;

        [Required]
        public TimeSpan SaturdayCloses { get; set; } = TimeSpan.Zero;

        [Required]
        public TimeSpan SundayOpens { get; set; } = TimeSpan.Zero;

        [Required]
        public TimeSpan SundayCloses { get; set; } = TimeSpan.Zero;

        // Timestamp fields
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [DataType(DataType.DateTime)]
        public DateTime? UpdatedAt { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;
        public bool? OpenStatus { get; set; }
    }
}
