using EcommerceCore.Models;
using EcommerceCore.Models ;
using EcommerceCore.ViewModel.Category;
using EcommerceCore.ViewModel.Image;

namespace EcommerceCore.ViewModel.Product
{
    public class ProductImageCrudModel : BaseProductImage
    {
        public ImageCrudModel Image { get; set; }
        public int ProductId { get; set; }
    }
}
