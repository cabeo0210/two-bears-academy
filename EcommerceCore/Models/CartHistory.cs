using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Numerics;


namespace EcommerceCore.Models
{
    public class BaseCartHistory : BaseClass
    {
        [Key]
        public int Id { get; set; }
        public string CartHistoryJson { get; set; }
        public DateTime CheckoutAt { get; set; }
    }

    public class CartHistory : BaseCartHistory
    {

    }
}
