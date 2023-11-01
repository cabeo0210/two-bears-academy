using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Numerics;

namespace EcommerceCore.Models
{
    public class BaseSetting : BaseClass
    {
        [Key]
        public int Id { get; set; }
        public string SettingConfig { get; set; }
    }

    public class Setting : BaseSetting
    {

    }
}
