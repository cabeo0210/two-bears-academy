using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceCore.Const
{
    public class SysEnum
    {
        public static int PageSize = 12;
        public enum Gender
        {
            [Display(Name = "Nam")]
            Male = 0,
            [Display(Name = "Nữ")]
            Female = 1,
            [Display(Name = "Khác")]
            Other = 2,
        }
        public enum Role
        {
            [Display(Name = "Admin")]
            Admin = 0,
            [Display(Name = "Staff")]
            Staff = 1,
            [Display(Name = "EndUser")]
            EndUser = 2,
            [Display(Name = "Sale")]
            Sale = 3
        }
        public enum StatusEnroll
        {
            [Display(Name = "Đăng ký")]
            Register = 0,
            [Display(Name = "Cọc giữ chỗ")]
            Deposit = 1,
            [Display(Name = "Đóng học phí")]
            Tuition = 2,
            [Display(Name = "Trả cọc chưa có lớp ")]
            ReturnTuition = 3,
            [Display(Name = "Chuyển lớp khác")]
            ChangeClass = 4
        }
        public enum ProductStatus
        {
            [Display(Name = "Tất cả")]
            All = 0,
            [Display(Name = "Đang hoạt động")]
            Active = 1,
            [Display(Name = "Hết hàng")]
            OutOfStock = 2,
            [Display(Name = "Đã ẩn")]
            Hidden = 3,
        }

        public enum OrderStatus
        {
            [Display(Name = "Đang xử lý")]
            Pending = 0,
            [Display(Name = "Đã hủy")]
            Cancel = 1,
            [Display(Name = "Đã hoàn thành")]
            Completed = 2,

        }
        public enum PaymentStatus
        {
            [Display(Name = "Chưa thanh toán")]
            Pending = 0,
            [Display(Name = "Đã thanh toán")]
            Completed = 2,
        }
        public enum ShippingStatus
        {
            [Display(Name = "Chờ xác nhận")]
            WaitingForConfirm = 0,
            [Display(Name = "Chờ lấy hàng")]
            WaitingForTake = 1,
            [Display(Name = "Đang giao")]
            InProgress = 2,
            [Display(Name = "Đã giao")]
            Completed = 3,
            [Display(Name = "Đã hủy")]
            Cancel = 4,
            [Display(Name = "Hoàn tiền")]
            Refund = 5,

        }

        public enum CouponType
        {
            [Display(Name = "Giảm thẳng")]
            Direct = 0,
            [Display(Name = "Giảm phần trăm")]
            Percent = 1,
        }

        public enum CouponStatus
        {
            [Display(Name = "Tất cả")]
            All = 0,
            [Display(Name = "Đang diễn ra")]
            Happening = 1,
            [Display(Name = "Kết thúc")]
            End = 2,
        }

        public enum ProductFeedbackType
        {
            [Display(Name = "Content")]
            Content = 0,
            [Display(Name = "Rating")]
            Rating = 1,
        }
        
        public enum LeadPosition
        {
            [Display(Name = "Lead")]
            Lead = 0,
            
        }
    }
}
