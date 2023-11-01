
using EcommerceCore.Models;

namespace EcommerceCore.ViewModel.ProductFeedback
{
    public class ProductFeedbackViewModel : BaseProductFeedback
    {
        public EcommerceCore.ViewModel.User.UserViewModel User { get; set; }
    }
}
