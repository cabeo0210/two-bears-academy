using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EcommerceCore.Const;
using EcommerceCore.Models;
using EcommerceCore.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using sib_api_v3_sdk.Client;
using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Client;
using sib_api_v3_sdk.Model;
using System.Reflection.PortableExecutable;
using System.Net.Mail;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Reflection.Metadata;
using System.Text;
using System.Net;
using EcommerceCore.ViewModel.User;
using Ecommerce.Repositories;
using EcommerceCore;

namespace Ecommerce.Controllers
{

    public class EndUserController : Controller
    {

        private EcommerceDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;
        private string _sendgridApiKey = "";
        private string _senderEmail = "";
        private string _senderName = "";
        private UserRepository _userRepository;
        public EndUserController(EcommerceDbContext dbContext, IMapper mapper, IWebHostEnvironment env, IConfiguration config)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _env = env;
            _config = config;
            _userRepository = new UserRepository(_dbContext, _mapper);
            _sendgridApiKey = _config.GetValue<string>("SendGrid:ApiKey", _sendgridApiKey);
            _senderEmail = _config.GetValue<string>("SendGrid:SenderEmail", _senderEmail);
            _senderName = _config.GetValue<string>("SendGrid:SenderName", _senderName);
        }
        public IActionResult Index()
        {
            var data = _userRepository.Where(
                    x => !x.IsDeleted
                    && x.Role != (int)SysEnum.Role.Admin)
                .ToList();
            var result = _mapper.Map<List<UserViewModel>>(data);
            return View(result);
        }
        public IActionResult Create()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(UserCrudModel model)
        {
            try
            {

               // TODO
                return View(model);



            }
            catch (Exception ex)
            {
                return View("Error", "Lỗi khi tạo tài khoản");
            }

        }
        public IActionResult Edit(int id)
        {
            var data = _userRepository.FirstOrDefault(x => x.UserId == id);
            var result = _mapper.Map<UserCrudModel>(data);
            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserCrudModel model)
        {
            try
            {
               // TODO
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View("Error",  "Lỗi khi tạo tài khoản");
            }

        }

        public IActionResult Delete(int id)
        {
            var data = _dbContext.Users.FirstOrDefault(x => x.UserId == id);
            var result = _mapper.Map<UserCrudModel>(data);
            return View(result);
        }

        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var data = _dbContext.Users.FirstOrDefault(x => x.UserId == id);
                if (data != null)
                {
                    data.IsDeleted = true;
                    data.UpdatedAt = DateTime.Now;

                }
                await _userRepository.CommitAsync();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View("Error", ex.Message);
                //throw new Exception(ex.Message);
            }

        }
        public async Task<IActionResult> UpdateStatus(int id, bool isActive)
        {
            var data = _dbContext.Users.FirstOrDefault(x => x.UserId == id);
            if (data != null)
            {
                data.IsActive = !isActive;
            }
            await _userRepository.CommitAsync();
            return RedirectToAction("Index");
        }
    }
}
