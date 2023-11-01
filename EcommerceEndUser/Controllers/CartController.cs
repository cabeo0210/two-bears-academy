using AutoMapper;
using ClosedXML.Report.Utils;
using DocumentFormat.OpenXml.ExtendedProperties;
using EcommerceCore.Const;
using Ecommerce.Helper;
using EcommerceCore.Models;
using Ecommerce.Repositories;
using EcommerceCore.ViewModel.Cart;
using EcommerceCore.ViewModel.Order;
using EcommerceCore.ViewModel.User;
using EcommerceEndUser.Permission;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Diagnostics;

namespace Ecommerce.EndUser.Controllers
{
    [LoginRequired]
    public class CartController : Controller
    {
        private readonly ProductRepository _productRepository;
        private readonly UserRepository _userRepository;
        private readonly CartRepository _cartRepository;
        private readonly CartItemRepository _cartItemRepository;
        private readonly CouponRepository _couponRepository;
        private readonly CouponHistoryRepository _couponHistoryRepository;
        private readonly SettingRepository _settingRepository;
        private readonly OrderRepository _orderRepository;
        private readonly OrderItemRepository _orderItemRepository;
        private EcommerceDbContext _dbContext;
        private readonly IMapper _mapper;
        public CartController(EcommerceDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _productRepository = new ProductRepository(_dbContext, _mapper);
            _userRepository = new UserRepository(_dbContext, _mapper);
            _cartRepository = new CartRepository(_dbContext, _mapper);
            _cartItemRepository = new CartItemRepository(_dbContext, _mapper);
            _couponRepository = new CouponRepository(_dbContext, _mapper);
            _couponHistoryRepository = new CouponHistoryRepository(_dbContext, _mapper);
            _settingRepository = new SettingRepository(_dbContext, _mapper);
            _orderRepository = new OrderRepository(_dbContext, _mapper);
            _orderItemRepository = new OrderItemRepository(_dbContext, _mapper);
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                CartViewModel model = new();
                var user = HttpContext.Session.GetCurrentAuthentication();
                var userData = _userRepository.FirstOrDefault(x => x.UserId == user.UserId);
                var cart = _cartRepository
                    .BuildQuery(x => x.Id == userData.CartId)
                    .Include(x => x.Coupon)
                    .Include(x => x.CartItems)
                        .ThenInclude(x => x.Product)
                        .ThenInclude(x => x.ProductImages)
                        .ThenInclude(x => x.Image)
                    .FirstOrDefault();
                if (cart != null)
                {
                    await UpdateCartTotalPrice(cart.Id);
                }
                model = _mapper.Map<CartViewModel>(cart);
                return View(model);
            }
            catch (Exception ex)
            {
                Response.Cookies.Delete("token");
                return Redirect("/user/login");
            }
        }

        public IActionResult Checkout()
        {
            var user = HttpContext.Session.GetCurrentAuthentication();
            UserViewModel model = new UserViewModel();
            if (user != null)
            {
                var userData = _userRepository
                    .Where(x => x.UserId == user.UserId)
                    .Include(x => x.Cart)
                    .ThenInclude(x => x.CartItems)
                    .ThenInclude(x => x.Product)
                    .ThenInclude(x => x.ProductImages)
                    .ThenInclude(x => x.Image)
                    .FirstOrDefault();
                model = _mapper.Map<UserViewModel>(userData);
            }
            var settingConfig = _settingRepository.FirstOrDefault(x => x.IsActive && !x.IsDeleted);
            ViewData["SettingConfig"] = settingConfig.SettingConfig;
            return View(model);
        }
        public async Task<IActionResult> SettleUp()
        {
            List<int> productIdsData = new();
            var user = HttpContext.Session.GetCurrentAuthentication();
            UserViewModel userModel = new UserViewModel();
            var tmpCartId = 0;
            if (user != null)
            {
                var userData = _userRepository
                    .Where(x => x.UserId == user.UserId)
                    .Include(x => x.Cart)
                    .ThenInclude(x => x.CartItems)
                    .ThenInclude(x => x.Product)
                    .ThenInclude(x => x.ProductImages)
                    .ThenInclude(x => x.Image)
                    .FirstOrDefault();
                userModel = _mapper.Map<UserViewModel>(userData);
                // Lưu tạm CartId
                tmpCartId = userModel.CartId.Value;
            }
            Order order = new()
            {
                Name = "",
                Code =   "OD" + DateTime.Now.ToString("yyyyMMddHHmmss") + user.UserId,
                PaymentStatus = (int)SysEnum.PaymentStatus.Pending,
                ShippingStatus = (int)SysEnum.ShippingStatus.WaitingForConfirm,
                CreateByUserId = user.UserId,
                OrderStatus = (int)SysEnum.OrderStatus.Pending,
                Address = userModel.Address,
            };
            await _orderRepository.AddAsync(order);
            await _orderRepository.CommitAsync();
            foreach (var cartItem in userModel.Cart.CartItems)
            {
                productIdsData.Add(cartItem.ProductId.Value);
            }
            foreach (var item in productIdsData)
            {
                var productItem = _productRepository.GetById(item);
                if (productItem != null)
                {
                    var oderItem = new OrderItem()
                    {
                        ProductId = productItem.Id,
                        Price = productItem.SellPrice,
                        PriceTotal = productItem.SellPrice,
                        Quantity = 1,
                        Order = order,
                        OrderId = order.Id,
                    };
                    await _orderItemRepository.AddAsync(oderItem);
                }
            }
            order.Total = userModel.Cart.Total;
            await _orderItemRepository.CommitAsync();


            // Sau khi ord thì xóa CartItem, Cart
            var existCartItem = _cartItemRepository.BuildQuery(x => x.CartId == userModel.CartId).ToList();
            if (existCartItem != null && existCartItem.Any())
            {
                foreach (var cartItemRemove in existCartItem)
                {
                    _cartItemRepository.DeleteById(cartItemRemove.Id);

                }
            }
            userModel.CartId = null;
            await _userRepository.CommitAsync();
            RemoveCart(tmpCartId, user.UserId);
            await _cartRepository.CommitAsync();
            return RedirectToAction("OrderHistory", "HomeEndUser");


        }
        public async Task<IActionResult> UpdateCartItem(
            int cartItemId,
            bool isAdd
            )
        {
            var checkCartItem = _cartItemRepository.FirstOrDefault(x => x.Id == cartItemId);
            if (checkCartItem != null)
            {
                var product = _productRepository.FirstOrDefault(x => x.Id == checkCartItem.ProductId);
                var cart = _cartRepository
                    .BuildQuery(x => x.Id == checkCartItem.CartId)
                    .Include(x => x.CartItems)
                    .FirstOrDefault();
                if (product != null)
                {

                    // Case này user cộng thêm item
                    // Dzô bảng Product coi còn đủ Quantity hok 
                    if (isAdd)
                    {
                        if (product.Quantity > 0)
                        {
                            product.Quantity--;
                            checkCartItem.Quantity++;
                        }
                        else
                        {
                            throw new Exception("Sản phẩm không đủ số lượng");
                        }
                    }
                    else
                    {
                        // Này dzô case giảm item
                        // Follow 2 case dưới đây
                        // Case 1: Nếu cart đang có nhìu hơn 1 item

                        if (cart.CartItems.Count >= 2)
                        {
                            // Step 1: Lấy Quantity của CartItem hiện tại
                            // Nếu Quantity hiện tại lớn hơn 1 => Update lại Quantity cho Product
                            if (checkCartItem.Quantity > 1)
                            {
                                checkCartItem.Quantity--;
                                product.Quantity++;
                            }
                            else
                            {
                                // Còn nếu quantity hiện tại là 1 => tức là sau khi trừ nó sẽ về 0 => Xóa lun CartItem
                                product.Quantity++;
                                _cartItemRepository.DeleteById(checkCartItem.Id);
                            }
                        }
                        else
                        {
                            // Case này là Cart chỉ có 1 duy nhất 1 item
                            // Check tương tự case trên với case này sẽ xóa lun cái cart

                            if (checkCartItem.Quantity > 1)
                            {
                                checkCartItem.Quantity--;
                                product.Quantity++;
                            }
                            else
                            {
                                // Có xóa cart nên phải set CartId trong User về null trước rồi mỡi xóa được
                                product.Quantity++;
                                var user = HttpContext.Session.GetCurrentAuthentication();
                                if (user != null)
                                {

                                    var userData = _userRepository.FirstOrDefault(x => x.UserId == user.UserId);
                                    userData.CartId = null;
                                    await _userRepository.CommitAsync();
                                }
                                _cartItemRepository.DeleteById(checkCartItem.Id);
                                RemoveCart(cart.Id, user.UserId);
                            }
                        }
                    }

                    // Sau khi update quantity xong thì call lại hàm UpdateCartTotalPrice
                }
                await _productRepository.CommitAsync();

            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> RemoveCartItem(int cartItemId)
        {
            var checkCartItem = _cartItemRepository.FirstOrDefault(x => x.Id == cartItemId);
            if (checkCartItem != null)
            {
                var product = _productRepository.FirstOrDefault(x => x.Id == checkCartItem.ProductId);
                if (product != null)
                {
                    product.Quantity++;
                    await _productRepository.CommitAsync();
                }
                var cart = _cartRepository.BuildQuery(x => x.Id == checkCartItem.CartId)
                    .Include(x => x.CartItems)
                    .FirstOrDefault();
                if (cart != null)
                {
                    // Nếu cart đang có hơn 1 item thì update lại giá
                    // Khum thì xóa lun cái cart
                    if (cart.CartItems.Count >= 2)
                    {
                        cart.Total -= product.SellPrice;

                        _cartItemRepository.DeleteById(checkCartItem.Id);

                    }
                    else
                    {
                        // Muốn xóa cart thì cái CartId trong bảng User phải bằng null
                        // Dzô bảng User set nó null trước rồi mới xóa được
                        // Do xóa Cart là xóa cứng
                        var user = HttpContext.Session.GetCurrentAuthentication();
                        if (user != null)
                        {
                            var userData = _userRepository.FirstOrDefault(x => x.UserId == user.UserId);
                            userData.CartId = null;
                        }
                        await _userRepository.CommitAsync();
                        _cartItemRepository.DeleteById(checkCartItem.Id);
                        //_cartRepository.DeleteById(cart.Id);
                        RemoveCart(cart.Id, user.UserId);
                    }
                }
                await _cartRepository.CommitAsync();
                await _cartItemRepository.CommitAsync();
            }
            return RedirectToAction("Index");
        }
        public void RemoveCart(int cartId, int userId)
        {
            // Xóa Cart xóa lun lịch sử dùng coupon của User

            var couponHistory = _couponHistoryRepository.FirstOrDefault(x => x.UserId == userId);
            if (couponHistory != null)
            {
                _couponHistoryRepository.DeleteById(couponHistory.Id);
            }
            _cartRepository.DeleteById(cartId);
        }
        public async Task<IActionResult> CreateCart(int productId)
        {
            var user = HttpContext.Session.GetCurrentAuthentication();
            if (productId != 0)
            {
                await AddToCart(user.UserId, productId);

            }
            return RedirectToAction("Index");
        }
        private async Task<bool> AddToCart(
            int userId,
            int productId
            )
        {
            var user = _userRepository.FirstOrDefault(x => x.UserId == userId);
            if (user != null)
            {
                Cart cart = new();
                CartItem cartItem = new();
                var product = _productRepository
                    .FirstOrDefault(x => x.Id == productId
                    && x.IsActive
                    && !x.IsDeleted
                    && x.Quantity > 0);
                if (product == null)
                {
                    throw new Exception("Sản phẩm không đủ số lượng");
                }
                var checkCart = _cartRepository
                        .BuildQuery(x => x.Id == user.CartId)
                        .Include(x => x.CartItems)
                        .FirstOrDefault();
                if (checkCart == null)
                {
                    cart.CreatedAt = DateTime.Now;
                    cart.UpdatedAt = DateTime.Now;
                    cart.IsActive = true;
                    cart.IsDeleted = false;
                    cart.CouponId = null;
                    await _cartRepository.AddAsync(cart);
                    await _cartRepository.CommitAsync();


                    cartItem.ProductId = product.Id;
                    cartItem.Quantity = 1;
                    cartItem.Cart = cart;
                    cartItem.CartId =  cart.Id;
                    cartItem.CreatedAt = DateTime.Now;
                    cartItem.UpdatedAt = DateTime.Now;
                    cartItem.IsActive = true;
                    cartItem.IsDeleted = false;
                    await _cartItemRepository.AddAsync(cartItem);
                    await _cartItemRepository.CommitAsync();

                    cart.Total = product.SellPrice;
                    user.CartId = cart.Id;
                    await _userRepository.CommitAsync();

                }
                else
                {
                    if (checkCart.CartItems.Any(x => x.ProductId == productId))
                    {
                        foreach (var item in checkCart.CartItems)
                        {
                            if (item.ProductId == productId)
                            {
                                item.Quantity++;

                            }
                        }
                    }
                    else
                    {
                        CartItem newCartItem = new()
                        {
                            ProductId = product.Id,
                            Quantity = 1,
                            CartId =  checkCart.Id,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now,
                            IsActive = true,
                            IsDeleted = false,
                        };
                        await _cartItemRepository.AddAsync(newCartItem);
                    }
                }
                product.Quantity--;
                await _productRepository.CommitAsync();
                await _cartItemRepository.CommitAsync();
                await _cartRepository.CommitAsync();
            }
            return true;
        }

        private async Task<bool> UpdateCartTotalPrice(
            int cartId
            )
        {
            var cart = _cartRepository
                .BuildQuery(x => x.Id == cartId)
                .Include(x => x.Coupon)
                .Include(x => x.CartItems)
                .ThenInclude(x => x.Product)
                .FirstOrDefault();
            if (cart != null)
            {
                cart.Total = 0;
                if (cart.CartItems != null && cart.CartItems.Any())
                {

                    foreach (var cartItem in cart.CartItems)
                    {
                        cart.Total += cartItem.Product.SellPrice * cartItem.Quantity;
                    }

                    var coupon = _couponRepository.FirstOrDefault(x => x.Id == cart.CouponId);
                    if (coupon != null)
                    {
                        if (coupon.CouponPriceType == (int)SysEnum.CouponType.Direct)
                        {
                            if (coupon.CouponPriceValue >= cart.Total)
                            {
                                cart.Total = 0;
                            }
                            else
                            {
                                cart.Total -= coupon.CouponPriceValue;

                            }
                        }
                        else if (coupon.CouponPriceType == (int)SysEnum.CouponType.Percent)
                        {
                            cart.Total -= cart.Total * coupon.CouponPriceValue / 100;
                        }
                    }
                    await _cartRepository.CommitAsync();
                }
            }
            return true;
        }
        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(string coupon, int cartId)
        {
            var cart = _cartRepository.BuildQuery(x => x.Id == cartId).Include(x => x.Coupon).FirstOrDefault();
            var user = HttpContext.Session.GetCurrentAuthentication();
            if (cart != null)
            {
                if (coupon != "")
                {
                    coupon = coupon.Trim();
                }
                var couponData = _couponRepository
                    .FirstOrDefault(x => x.Code == coupon
                    && x.IsActive
                    && !x.IsDeleted
                    &&  DateTime.Now >= x.TimeStart
                    &&  DateTime.Now <= x.TimeEnd);
                if (couponData != null)
                {

                    // Nếu coupon khác null => Dzô bảng CouponHistory check coi còn đủ coupon để xài hay khum
                    var counponHistory = _couponHistoryRepository
                        .BuildQuery(x => x.CouponId == couponData.Id)
                        .ToList();
                    if (counponHistory != null && counponHistory.Any())
                    {
                        if (counponHistory.Count() >= couponData.LimitationTimes)
                        {
                            return new JsonResult("Không thể áp dụng coupon!");
                        }
                    }

                    // 1 User chỉ có 1 Cart
                    // Nên mỗi User chỉ được áp dụng Coupon 1 lần
                    // Case 1: Cart chưa áp dụng coupon nào hết => Tạo record ở bảng CouponHistory
                    if (cart.CouponId == null)
                    {
                        CouponHistory couponHistory = new()
                        {
                            UserId = user.UserId,
                            CouponId = couponData.Id,
                        };
                        await _couponHistoryRepository.AddAsync(couponHistory);
                        cart.CouponId = couponData.Id;
                    }
                    // Case 2: Cart đã áp dụng coupon nào hết => Update lại CouponId là đượt

                    else
                    {
                        var userCounponHistory = _couponHistoryRepository.FirstOrDefault(x => x.UserId == user.UserId);
                        if (userCounponHistory != null)
                        {
                            userCounponHistory.CouponId = couponData.Id;
                            cart.CouponId = couponData.Id;

                        }
                    }

                }
                else
                {
                    return new JsonResult("Coupon không hợp lệ!");
                }
            }
            await _couponHistoryRepository.CommitAsync();
            await _couponRepository.CommitAsync();
            Response.StatusCode = 200;
            return new JsonResult("Áp dụng thành công");
        }
    }
}