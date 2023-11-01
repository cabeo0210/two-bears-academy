using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EcommerceCore.Const;
using Ecommerce.Helper;
using EcommerceCore.Models;
using Ecommerce.Repositories;
using EcommerceCore.ViewModel;
using EcommerceCore.ViewModel.User;
using Ecommrece.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommerceCore;
namespace Ecommerce.Controllers
{

    public class UserController : Controller
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private EcommerceDbContext _dbContext;
        private UserRepository _userRepository;

        private readonly IMapper _mapper;

        public UserController(IHttpContextAccessor httpContextAccessor, EcommerceDbContext dbContext, IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
            _mapper = mapper;
            _userRepository = new UserRepository(_dbContext, _mapper);

        }

        public IActionResult Rename()
        {
            //IFileManagerService _file = new FileManagerService();


            return Json("");
        }



        public ActionResult Login()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult TermAndCondition()
        {
            return View();
        }
        public IActionResult Test()
        {
            _userRepository.Add(new UserCrudModel
            {
                Name = "hehe",
                Password = "12456",
                Email = "abc@gmail.com",
                Phone = "091494452",
            });

            return View();
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
            }
            HttpContext.Session.SetCurrentAuthentication(result);
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Error(string errorMessage)
        {
            ViewData["ErrorMessage"] = errorMessage;
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        private AuthenticationModel LoginValid(LoginViewModel loginModel)
        {

            var user = _dbContext.Users
            .FirstOrDefaultAsync(
               x => x.Email!.Trim().ToLower() == loginModel.Email!.Trim().ToLower()
               && x.Password == loginModel.Password
               && x.Role == (int)SysEnum.Role.Admin
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

    }
}
