using EcommerceCore.Models;
using EcommerceCore.Models ;
using EcommerceCore.ViewModel.Category;
using EcommerceCore.ViewModel.Product;
using EcommerceCore.ViewModel.User;

namespace EcommerceCore.ViewModel.Order
{
    public class OrderCrudModel : BaseOrder
    {
        public UserViewModel User { get; set; }
        public List<UserViewModel> ListUserViewModel { get; set; }
        public List<OrderItemViewModel> OrderItems { get; set; }
        public List<ProductViewModel> ListProductViewModel { get; set; }
        public string ProductIds { get; set; }
    }
}
