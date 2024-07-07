
using AutoMapper;
using EcommerceCore.ViewModel;
using EcommerceCore.ViewModel.User;
using EcommerceCore.Models;

namespace Ecommerce.Helper
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<UserViewModel, User>().ReverseMap();
            CreateMap<UserCrudModel, User>().ReverseMap();

        }
    }
}
