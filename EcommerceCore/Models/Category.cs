using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Numerics;

namespace EcommerceCore.Models
{
    public class BaseCategory : BaseClass
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Không được để trống")]
        [MaxLength(255, ErrorMessage = "Không được vượt quá 255 ký tự")]
        public string Name { get; set; }
    }

    public class Category : BaseCategory
    {

    }
}
