using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.DTOs.UserDto
{
    public class CreateUserDto
    {
        [Required]
        [StringLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string UserName { get; set; } = "";

        [Required]
        [StringLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string LastName { get; set; } = "";

        [Required]
        [StringLength(255)]
        [EmailAddress] 
        [Column(TypeName = "nvarchar(255)")]
        public string Email { get; set; } = "";

        [Required]
        [StringLength(255)]
        public string UserPassword { get; set; } = "";

        [StringLength(255)]
        [Column(TypeName = "nvarchar(255)")]
        public string Avatar { get; set; } =
            "https://cdn.pixabay.com/photo/2015/10/05/22/37/blank-profile-picture-973460_1280.png";
    }
}
