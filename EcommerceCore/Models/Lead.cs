using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceCore.Models;

public class BaseLead : BaseClass
{
    [Key]
    public int LeadId { get; set; }
    public int ClaimUserId { get; set; }
    public Guid LeadGuid { get; set; }
    [Required(ErrorMessage = "Không được để trống")]
    [MaxLength(200, ErrorMessage = "Không được vượt quá 255 ký tự")]
    public string Name { get; set; }
    public string Email { get; set; }
    [Required(ErrorMessage = "Không được để trống")]
    [MaxLength(20)]
    public string Phone { get; set; }
    public string Position { get; set; }
    public string Note { get; set; }
    public string Source { get; set; }
}

public class Lead : BaseLead
{
    [ForeignKey(nameof(ClaimUserId))]
    public User User { get; set; }
}