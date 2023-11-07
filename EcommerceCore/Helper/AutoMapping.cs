
using AutoMapper;
using EcommerceCore.ViewModel;
using EcommerceCore.ViewModel.Category;
using EcommerceCore.ViewModel.Order;
using EcommerceCore.ViewModel.Image;
using EcommerceCore.ViewModel.Product;
using EcommerceCore.ViewModel.ProductCategory;
using EcommerceCore.ViewModel.User;
using EcommerceCore.ViewModel.Coupon;
using EcommerceCore.ViewModel.Setting;
using EcommerceCore.ViewModel.New;
using EcommerceCore.ViewModel.ProductFeedback;
using EcommerceCore.ViewModel.Cart;
using EcommerceCore.ViewModel.ProductHistory;
using EcommerceCore.Models;
using EcommerceCore.ViewModel.Tuyen;
using EcommerceCore.ViewModel.TuyenSinh;

namespace Ecommerce.Helper
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<UserViewModel, User>().ReverseMap();
            CreateMap<UserCrudModel, User>().ReverseMap();

            CreateMap<ProductViewModel, Product>().ReverseMap();
            CreateMap<ProductCrudModel, Product>().ReverseMap();

            CreateMap<CategoryViewModel, Category>().ReverseMap();
            CreateMap<CategoryCrudModel, Category>().ReverseMap();

            CreateMap<ProductCategoryViewModel, ProductCategory>().ReverseMap();

            // Cart
            CreateMap<CartViewModel, Cart>().ReverseMap();
            CreateMap<CartCrudModel, Cart>().ReverseMap();

            // Cart Item
            CreateMap<CartItemViewModel, CartItem>().ReverseMap();
            CreateMap<CartItemCrudModel, CartItem>().ReverseMap();

            // Order
            CreateMap<OrderViewModel, Order>().ReverseMap();
            CreateMap<OrderCrudModel, Order>().ReverseMap();
            CreateMap<OrderItemViewModel, OrderItem>().ReverseMap();
            CreateMap<OrderItemCrudModel, OrderItem>().ReverseMap();
            CreateMap<ImageViewModel, Image>().ReverseMap();
            CreateMap<ImageCrudModel, Image>().ReverseMap();

            CreateMap<ProductImageViewModel, ProductImage>().ReverseMap();
            CreateMap<ProductImageCrudModel, ProductImage>().ReverseMap();

            CreateMap<CouponViewModel, Coupon>().ReverseMap();
            CreateMap<CouponCrudModel, Coupon>().ReverseMap();

            CreateMap<CouponHistoryViewModel, CouponHistory>().ReverseMap();
            CreateMap<CouponHistoryCrudModel, CouponHistory>().ReverseMap();

            CreateMap<SettingViewModel, Setting>().ReverseMap();
            CreateMap<SettingCrudModel, Setting>().ReverseMap();

            CreateMap<NewViewModel, New>().ReverseMap();
            CreateMap<NewCrudModel, New>().ReverseMap();

            CreateMap<ProductFeedbackViewModel, ProductFeedback>().ReverseMap();
            CreateMap<ProductFeedbackCrudModel, ProductFeedback>().ReverseMap();

            CreateMap<ProductHistoryViewModel, ProductHistory>().ReverseMap();
            CreateMap<ProductHistoryCrudModel, ProductHistory>().ReverseMap();
            
            // Leader
            CreateMap<LeaderCrudViewModel, Lead>().ReverseMap();
            
            
            
            CreateMap<EnrollViewModel, Enroll>().ReverseMap();
            CreateMap<EnrollCrudViewModel, Enroll>().ReverseMap();
        }
    }
}
