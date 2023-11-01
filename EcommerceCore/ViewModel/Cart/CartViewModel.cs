using EcommerceCore.Models;
using EcommerceCore.Models;
using EcommerceCore.ViewModel.Coupon;

namespace EcommerceCore.ViewModel.Cart
{
    public class CartViewModel : BaseCart
    {
        public List<CartItemViewModel> CartItems { get; set; }
        public CouponViewModel Coupon { get; set; }
    }
}
