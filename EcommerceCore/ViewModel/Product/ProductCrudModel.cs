using EcommerceCore.Models;
using EcommerceCore.ViewModel.Category;
using Microsoft.AspNetCore.Http;

namespace EcommerceCore.ViewModel.Product
{
    public class ProductCrudModel : BaseProduct
    {
        public IFormFile? FileImage { get; set; }
        public List<CategoryViewModel>? ListCategoryViewModel { get; set; }
    }
}
