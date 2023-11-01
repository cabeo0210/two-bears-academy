using EcommerceCore.Const;
using EcommerceCore.Models;
using EcommerceCore.Models ;
using EcommerceCore.Models ;
using EcommerceCore.ViewModel.Category;

namespace EcommerceCore.ViewModel.Coupon
{
    public class CouponViewModel : BaseCoupon
    {
        public string CouponPriceTypeDisplay
        {
            get
            {
                switch (CouponPriceType)
                {
                    case (int)SysEnum.CouponType.Direct:
                        return SysEnum.CouponType.Direct.GetEnumDisplayName();
                    default:
                        return SysEnum.CouponType.Percent.GetEnumDisplayName();
                }
            }

        }
    }
}
