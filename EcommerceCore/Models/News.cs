using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Numerics;

namespace EcommerceCore.Models
{
    public class BaseNew : BaseClass
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Không được để trống")]
        [MaxLength(255, ErrorMessage = "Không được vượt quá 255 ký tự")]
        public string Title { get; set; }
        public string Image { get; set; }
        public string Content { get; set; }
    }

    public class New : BaseNew
    {

    }
}
