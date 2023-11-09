using AutoMapper;
using EcommerceCore.Const;
using Ecommerce.Helper;
using EcommerceCore.Models;
using Ecommerce.Repositories;
using EcommerceCore.ViewModel;
using EcommerceCore.ViewModel.Category;
using EcommerceCore.ViewModel.New;
using EcommerceCore.ViewModel.Order;
using EcommerceCore.ViewModel.Product;
using EcommerceCore.ViewModel.User;
using EcommerceEndUser.Permission;
using Ecommrece.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using EcommerceCore;

namespace Ecommerce.EndUser.Controllers
{
    public class HomeEndUserController : Controller
    {
        private readonly ProductRepository _productRepository;
        private readonly CategoryRepository _categoryRepository;
        private readonly OrderRepository _orderRepository;
        private readonly NewRepository _newRepository;
        private readonly UserRepository _userRepository;
        private EcommerceDbContext _dbContext;
        private readonly IMapper _mapper;
        public HomeEndUserController(EcommerceDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _productRepository = new ProductRepository(_dbContext, _mapper);
            _categoryRepository = new CategoryRepository(_dbContext, _mapper);
            _orderRepository = new OrderRepository(_dbContext, _mapper);
            _newRepository = new NewRepository(_dbContext, _mapper);
            _userRepository = new UserRepository(_dbContext, _mapper);
        }
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(UserCrudModel model)
        {
            var errorModel = new ErrorViewModel();
            try
            {

                if (model.Email != null)
                {
                    var checkExistUser = _userRepository.FirstOrDefault(x => x.Email!.Trim().ToLower() == model.Email.Trim().ToLower() && !x.IsDeleted);
                    if (checkExistUser != null)
                    {
                        throw new Exception("Email này đã tồn tại!");
                    }
                }
                if (ModelState.IsValid)
                {
                    User user = new()
                    {
                        Email = model.Email!.Trim().ToLower(),
                        Name = model.Name!.Trim(),
                        DateOfBirth = model.DateOfBirth,
                        Gender = model.Gender,
                        Phone = model.Phone,
                        Password = model.Password!.Hash(),
                        Role = (int)SysEnum.Role.EndUser,
                        IsActive = true,
                        IsDeleted = false,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                    };
                    var data = _mapper.Map<UserCrudModel>(user);
                    //var uploadResult = await UploadFile(ufile: model.FileImage);
                    //data.Avatar = uploadResult;
                    _userRepository.Add(data);
                    await _userRepository.CommitAsync();
                    return RedirectToAction("Login");
                }
                else
                {
                    model.ErrorMessage = "Lỗi khi tạo tài khoản";
                    return View(model);
                }

            }
            catch (Exception ex)
            {
                model.ErrorMessage = "Lỗi khi tạo tài khoản";
                return View(model);
            }

        }
        [HttpPost]
        public ActionResult Login(LoginViewModel loginModel)
        {
            var result = new AuthenticationModel();
            if (loginModel.Password != null)
            {
                loginModel.Password = loginModel.Password.Hash();

            }
            try
            {
                result = LoginValid(loginModel);
            }
            catch (Exception)
            {
                loginModel.ErrorMessage = MessageConst.InvalidInfoFor("đăng nhập");
                return View(loginModel);
            }

            if (result == null)
            {

                return RedirectToAction("Error", new { errorMessage = MessageConst.InvalidInfoFor("đăng nhập") });
                //throw new Exception(MessageConst.InvalidInfoFor("đăng nhập"));
            }
            HttpContext.Session.SetCurrentAuthentication(result);
            var lastRequestURL = HttpContext.Session.GetString(TextConstant.LastRequestURL);

            if (string.IsNullOrEmpty(lastRequestURL))
            {
                return Redirect("/");
            }
            else
            {
                return Redirect(lastRequestURL);
            }
        }
        private AuthenticationModel LoginValid(LoginViewModel loginModel)
        {

            var user = _dbContext.Users
            .FirstOrDefaultAsync(
               x => x.Email!.Trim().ToLower() == loginModel.Email!.Trim().ToLower()
               && x.Password == loginModel.Password
               && x.Role == (int)SysEnum.Role.EndUser
               && x.IsActive
               && !x.IsDeleted
            ).Result;

            if (user == null)
            {
                throw new Exception(MessageConst.InvalidInfoFor("đăng nhập"));
            }
            if (user.IsActive == false)
            {
                throw new Exception("Tài khoản đang bị khóa. Hãy liên hệ quản trị viên nếu có nhầm lẫn hoặc có nhu cầu mở lại tài khoản.");
            }
            var authenticationModel = CreateAuthModel(user);
            return authenticationModel;
        }
        private AuthenticationModel CreateAuthModel(User user)
        {
            var authenticationModel = new AuthenticationModel()
            {
                Name = user.Name,
                UserGuid = user.UserGuid,
                UserId = user.UserId,
                AvatarUrl = string.IsNullOrEmpty(user.Avatar) ? "/img/no-avatar.png" : user.Avatar,
                Email = user.Email,
                Phone = user.Phone,
            };
            return authenticationModel;
        }
        public IActionResult Index(
            string keyword,
            int? selectType
            )
        {
            var data = _productRepository.BuildQuery(
              x => x.IsActive
              && !x.IsDeleted);
            if (keyword != null && keyword != "")
            {
                data = data.Where(x =>
                   EF.Functions.Like(x.Name!, $"%{keyword}%")
               );
            }

            if (selectType.HasValue)
            {
                data = data.Where(x => x.CategoryId == selectType.Value);
            }
            var categories = _categoryRepository
                 .BuildQuery(x =>
                 !x.IsDeleted
                 && x.IsActive)
                 .Select(x => _mapper.Map<CategoryViewModel>(x))
                 .ToList();
            ViewData["CategoryList"] = categories;
            data = data
                .Include(x => x.Category)
                .Include(x => x.ProductImages)
                .ThenInclude(x => x.Image);
            data = data.OrderByDescending(x => x.CreatedAt);
            var result = _mapper.Map<List<ProductViewModel>>(data.ToList());
            return View(result);
        }
        // public IActionResult Privacy()
        // {
        //     return View();
        // }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [LoginRequired]
        public IActionResult OrderHistory()
        {
            var user = HttpContext.Session.GetCurrentAuthentication();
            var userOrderHistory = _orderRepository.BuildQuery(x => x.CreateByUserId == user.UserId && !x.IsDeleted)
                .Include(x => x.OrderItems)
                    .ThenInclude(x => x.Product)
                .Select(x => _mapper.Map<OrderViewModel>(x))
                .ToList();
            return View(userOrderHistory);
        }
        public IActionResult News()
        {
            var news = _newRepository.BuildQuery(x => x.IsActive && !x.IsDeleted).Select(x => _mapper.Map<NewViewModel>(x));
            return View(news);
        }
        public IActionResult NewsDetail(int id)
        {
            var news = _newRepository.FirstOrDefault(x => x.Id == id && x.IsActive && !x.IsDeleted);
            var result = _mapper.Map<NewViewModel>(news);
            return View(result);
        }
    }
}