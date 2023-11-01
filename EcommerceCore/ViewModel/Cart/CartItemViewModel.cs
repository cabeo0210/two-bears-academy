using EcommerceCore.Models;
using EcommerceCore.Models;
using EcommerceCore.ViewModel.Product;

namespace EcommerceCore.ViewModel.Cart
{
    public class CartItemViewModel : BaseCartItem
    {
        public ProductViewModel Product { get; set; }
    }
}
