using AutoMapper;
using Ecommerce.Helper;
using EcommerceCore.Models;
using Ecommerce.Repositories;
using EcommerceCore.ViewModel.User;
using Microsoft.AspNetCore.Mvc;
using SendGrid.Helpers.Mail;
using SendGrid;
using System.Diagnostics;
using System.Text;
using EcommerceCore;

namespace Ecommerce.EndUser.Controllers
{
    public class UserProfileController : Controller
    {
        private readonly UserRepository _userRepository;
        private readonly IMapper _mapper;
        private EcommerceDbContext _dbContext;

        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;
        private string _sendgridApiKey = "";
        private string _senderEmail = "";
        private string _senderName = "";
        public UserProfileController(EcommerceDbContext dbContext, IMapper mapper, IWebHostEnvironment env, IConfiguration config)
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
            var user = HttpContext.Session.GetCurrentAuthentication();
            var data = _userRepository.FirstOrDefault(x => x.UserId == user.UserId && x.IsActive && !x.IsDeleted);
            var result = _mapper.Map<UserCrudModel>(data);
            return View(result);
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
                //return RedirectToAction("Index");
                return Logout();
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
        public IActionResult Logout()
        {
            HttpContext.Session.SetCurrentAuthentication(null);
            return RedirectToAction("Login", "HomeEndUser");
        }

        // public IActionResult Edit(int id)
        // {
        //     var data = _userRepository.FirstOrDefault(x => x.UserId == id);
        //     var result = _mapper.Map<UserCrudModel>(data);
        //     return View(result);
        // }

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
                            curUser.Name = model.Name!.Trim();
                            curUser.Gender = model.Gender;
                            curUser.Phone = model.Phone;
                            curUser.DateOfBirth = model.DateOfBirth;
                            curUser.UpdatedAt = DateTime.Now;
                            curUser.FirstName = model.FirstName;
                            curUser.LastName = model.LastName;
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

        private async Task<String> UploadFileToCloud(IFormFile ufile)
        {
            var image = UploadFile.AddPhoto(ufile, "user");
            return image;
        }

    }
}