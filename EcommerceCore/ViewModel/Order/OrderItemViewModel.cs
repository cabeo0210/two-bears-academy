using EcommerceCore.Const;
using EcommerceCore.Models;
using EcommerceCore.Models ;
using EcommerceCore.ViewModel.Category;
using EcommerceCore.ViewModel.Product;
using EcommerceCore.ViewModel.User;

namespace EcommerceCore.ViewModel.Order
{
    public class OrderItemViewModel : BaseOrderItem
    {
        public OrderViewModel Order { get; set; }
        public ProductViewModel Product { get; set; }
    }
}
