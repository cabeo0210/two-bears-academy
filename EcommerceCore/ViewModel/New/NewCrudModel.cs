using EcommerceCore.Const;
using EcommerceCore.Models;
using EcommerceCore.Models ;
using Microsoft.AspNetCore.Http;

namespace EcommerceCore.ViewModel.New
{
    public class NewCrudModel : BaseNew
    {
        public IFormFile? FileImage { get; set; }
    }
}
