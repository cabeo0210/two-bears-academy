using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Numerics;
using EcommerceCore.Models ;

namespace EcommerceCore.Models
{
    public class BaseUser : BaseClass
    {
        [Key]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Không được để trống")]
        [MaxLength(255, ErrorMessage = "Không được vượt quá 255 ký tự")]
        public string Password { get; set; }
        public Guid UserGuid { get; set; }

        [Required(ErrorMessage = "Không được để trống")]
        [MaxLength(100, ErrorMessage = "Không được vượt quá 255 ký tự")]
        public string Name { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Không được để trống")]
        public string Email { get; set; }
        public string Avatar { get; set; }

        [Required(ErrorMessage = "Không được để trống")]
        [MaxLength(20)]
        public string Phone { get; set; }
        public string Address { get; set; }
        public int Gender { get; set; }
        public int Role { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string FcmToken { get; set; }
        public Guid ResetPasswordGuid { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public DateTime? JoinedAt { get; set; }
        public int? CartId { get; set; }
    }
    public class User : BaseUser
    {
        [ForeignKey(nameof(CartId))]
        public Cart Cart { get; set; }
        public List<Lead> Leads { get; set; } 
    }
}
