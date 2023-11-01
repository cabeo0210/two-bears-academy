using EcommerceCore.Const;
using EcommerceCore.Models;
using EcommerceCore.ViewModel.Cart;
using EcommerceCore.ViewModel.Setting;
using Microsoft.AspNetCore.Http;

namespace EcommerceCore.ViewModel.User
{
    public class UserViewModel : BaseUser
    {
        public string DateOfBirthDisplay
        {
            get
            {
                return DateOfBirth.ToString("dd/MM/yyyy");
            }
        }
        public CartViewModel Cart { get; set; }
        public string EnumGenderDisplay => ((SysEnum.Gender)Enum.Parse(typeof(SysEnum.Gender), this.Gender.ToString())).GetEnumDisplayName();
        public string StatusDisplay
        {
            get
            {
                if (IsActive)
                {
                    return "Đang hoạt động";
                }
                else
                {
                    return "Bị khóa";
                }
            }
        }

    }
}
