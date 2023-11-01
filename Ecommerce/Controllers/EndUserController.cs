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
using EcommerceCore.ViewModel.Product;
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
                    var uploadResult = await UploadFileToCloud(ufile: model.FileImage);
                    data.Avatar = uploadResult;
                    _userRepository.Add(data);
                    await _userRepository.CommitAsync();
                    return RedirectToAction("Index");
                }
                else
                {
                    //errorModel.ErrorMessage = "Lỗi khi tạo tài khoản";
                    return View(model);
                }


            }
            catch (Exception ex)
            {
                errorModel.ErrorMessage = "Lỗi khi tạo tài khoản";
                return View("Error", errorModel);
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
            var errorModel = new ErrorViewModel();
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.Email != null)
                    {
                        var curUser = _userRepository.FirstOrDefault(x => x.UserId == model.UserId);
                        if (curUser != null)
                        {
                            // Hok cho update Email => do email là unique
                            // Nếu muốn check thì mở comment dưới này

                            //var checkExistUser = _dbContext
                            //.Users
                            //.FirstOrDefault(
                            //    x => x.Email!.Trim().ToLower() == model.Email.Trim().ToLower()
                            //        && x.UserId != model.UserId);
                            //if (checkExistUser != null)
                            //{
                            //    throw new Exception("Email này đã tồn tại!");
                            //}

                            curUser.Name = model.Name!.Trim();
                            curUser.Gender = model.Gender;
                            curUser.Phone = model.Phone;
                            curUser.Password = model.Password!.Hash();
                            curUser.DateOfBirth = model.DateOfBirth;
                            curUser.UpdatedAt = DateTime.Now;

                        }
                        if (model.FileImage != null)
                        {
                            var uploadResult = await UploadFileToCloud(ufile: model.FileImage);
                            curUser.Avatar = uploadResult;
                        }
                        await _userRepository.CommitAsync();
                    }
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                errorModel.ErrorMessage = "Lỗi khi tạo tài khoản";
                return View("Error", errorModel);
                //throw new Exception(ex.Message);
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
            var errorModel = new ErrorViewModel();
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
                errorModel.ErrorMessage = "Lỗi khi tạo tài khoản";
                return View("Error", errorModel);
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

        public async Task<IActionResult> ResetPassword(int id)
        {
            try
            {
                var user = _dbContext.Users.FirstOrDefault(x => x.UserId == id);
                var webRoot = _env.WebRootPath;
                var resetPassword = Path.Combine(webRoot, "template/reset_password.html");
                var emailBody = System.IO.File.ReadAllText(resetPassword);
                StringBuilder builder = new StringBuilder();
                builder.Append(RandomNumber(1000, 999999));
                emailBody = emailBody.Replace("{{newPassword}}", builder.ToString());
                emailBody = emailBody.Replace("{{userName}}", user.Name);
                var client = new SendGridClient(_sendgridApiKey);
                var from_email = new EmailAddress(_senderEmail, _senderName);
                var subject = "Thiết lập lại mật khẩu";
                string receiverAddress = user.Email!.Trim();
                var to_email = new EmailAddress(receiverAddress, receiverAddress);
                var plainTextContent = emailBody;
                var htmlContent = emailBody;
                var msg = MailHelper.CreateSingleEmail(from_email, to_email, subject, plainTextContent, htmlContent);
                var response = await client.SendEmailAsync(msg).ConfigureAwait(false);
                user.Password = builder.ToString().Hash();
                await _dbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                var errorModel = new ErrorViewModel();
                errorModel.ErrorMessage = "Có lỗi xảy ra khi gửi mail";
                return View("Error", errorModel);
            }

        }
        private int RandomNumber(int min, int max)
        {
            // Random 6 số
            Random random = new Random();
            return random.Next(min, max);
        }
        private async Task<String> UploadFileToCloud(IFormFile ufile)
        {
            var image = UploadFile.AddPhoto(ufile, "user");
            return image;
        }
    }
}
