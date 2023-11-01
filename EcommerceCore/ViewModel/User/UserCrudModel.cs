using EcommerceCore.Const;
using EcommerceCore.Models;
using Microsoft.AspNetCore.Http;

namespace EcommerceCore.ViewModel.User
{
    public class UserCrudModel : BaseUser
    {
        public IFormFile? FileImage { get; set; }
        public string ErrorMessage { get; set; }
    }
}
