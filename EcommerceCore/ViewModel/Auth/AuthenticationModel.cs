using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommrece.ViewModel
{
    public class AuthenticationModel
    {
        public string Name { get; set; }
        public int Role { get; set; }
        public int UserId { get; set; }
        public Guid UserGuid { get; set; }
        public string AvatarUrl { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime? VaildTime { get; set; }
        public DateTime? SubscriptionEndAt { get; set; }
    }
}
