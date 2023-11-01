using EcommerceCore.Models;
using EcommerceCore.Models ;
using EcommerceCore.ViewModel.Category;
using Microsoft.AspNetCore.Http;

namespace EcommerceCore.ViewModel.Image
{
    public class ImageCrudModel : BaseImage
    {
        public IFormFile? FileImage { get; set; }
        public int ProductId { get; set; }
    }
}
