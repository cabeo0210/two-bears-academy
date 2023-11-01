using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Numerics;

namespace EcommerceCore.Models
{
    public class BaseProductCategory : BaseClass
    {
        [Key]
        public int ProductCategoryId { get; set; }
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
    }

    public class ProductCategory : BaseProductCategory
    {
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; }
    }
}
