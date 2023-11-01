using EcommerceCore.Const;
using EcommerceCore.Models;
using EcommerceCore.Models ;
using EcommerceCore.ViewModel.Category;
using EcommerceCore.ViewModel.Product;
using EcommerceCore.ViewModel.User;

namespace EcommerceCore.ViewModel.Order
{
    public class OrderViewModel : BaseOrder
    {
        public UserViewModel User { get; set; }
        public List<OrderItemViewModel> OrderItems { get; set; }
        public string OrderStatusDisplay => ((SysEnum.OrderStatus)Enum.Parse(typeof(SysEnum.OrderStatus), this.OrderStatus.ToString())).GetEnumDisplayName();
        public string PaymentStatusDisplay => ((SysEnum.PaymentStatus)Enum.Parse(typeof(SysEnum.PaymentStatus), this.PaymentStatus.ToString())).GetEnumDisplayName();
        public string ShippingStatusDisplay => ((SysEnum.ShippingStatus)Enum.Parse(typeof(SysEnum.ShippingStatus), this.ShippingStatus.ToString())).GetEnumDisplayName();
    }
}
