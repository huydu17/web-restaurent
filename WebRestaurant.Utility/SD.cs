using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebRestaurant.Utility
{
    public class SD
    {
        //Role
        public const string Role_Admin = "Admin";
        public const string Role_Employee = "Employee";
        public const string Role_Customer = "Customer";

        //PaymentMethod
        public const string CashOnDelivery = "Thanh toán khi nhận hàng";
        public const string PaymentOnline = "Thanh toán Online";

        //OrderStatus
		public const string StatusPending = "Chờ xác nhận";
        public const string StatusApproved = "Đã xác nhận";
        public const string StatusInTransit = "Đang vận chuyển";
        public const string StatusShipped = "Đã giao";
        public const string StatusCancelled = "Đã huỷ";

        //OrderPaymentOnline
        public const string PaymentStatusCOD = "Chưa thanh toán";
        public const string PaymentStatusApproved = "Đã thanh toán";
        public const string PaymentRefund = "Đã hoàn tiền";
        //StatusCateogy
        public const string Active = "Hoạt động";
        public const string Inactive = "Ngừng hoạt động";
        //StatusProduct
        public const string InStock = "Còn hàng";
        public const string OutOfStock = "Hết hàng";
        //PageSize
        public const int PageSize = 4;
        //Session Cart
        public const string SessionCart = "SessionCart";
        //Display
        public const string Display = "Hiển thị";
        public const string NotDisplayed = "Không hiển thị";
    }
}
