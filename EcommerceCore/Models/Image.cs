using System.ComponentModel.DataAnnotations;

namespace EcommerceCore.Models
{
    public class BaseImage : BaseClass
    {
        [Key]
        public int Id { get; set; }
        public string Url { get; set; }
    }

    public class Image : BaseImage
    {

    }
}
