using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace EcommerceCore.Models
{
    public abstract class BaseClass
    {
        public BaseClass()
        {
            UpdatedAt = DateTime.Now;
            CreatedAt = DateTime.Now;
        }
        public DateTime UpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; }

        [DefaultValue(true)]
        public bool IsActive { get; set; }

        [DefaultValue(false)]
        public bool IsDeleted { get; set; }
    }
}
