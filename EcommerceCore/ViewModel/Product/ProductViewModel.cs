using EcommerceCore.Models;
using EcommerceCore.ViewModel.Category;
using EcommerceCore.ViewModel.Image;
using EcommerceCore.ViewModel.ProductFeedback;
using EcommerceCore.ViewModel.ProductHistory;

namespace EcommerceCore.ViewModel.Product
{
    public class ProductViewModel : BaseProduct
    {
        public int Purchases { get; set; }
        public List<ProductImageViewModel> ProductImages { get; set; }
        public List<ImageViewModel> ListProductImage { get; set; }

        public ProductFeedbackCrudModel ProductFeedback { get; set; }
        public List<ProductFeedbackViewModel> ListProductFeedback { get; set; }

        public bool IsBought { get; set; }
        public int UserRatingCount { get; set; }
        public int ProductRatingCount { get; set; }

    }
}
