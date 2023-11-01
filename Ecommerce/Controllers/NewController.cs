using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EcommerceCore.Const;
using EcommerceCore.Models;
using EcommerceCore.Models;
using EcommerceCore.Models ;
using Ecommerce.Repositories;
using EcommerceCore.ViewModel;
using EcommerceCore.ViewModel.Category;
using EcommerceCore.ViewModel.New;
using EcommerceCore.ViewModel.Order;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
namespace Ecommerce.Controllers
{

    public class NewController : Controller
    {
        private EcommerceDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly NewRepository _newRepository;

        public NewController(EcommerceDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _newRepository = new NewRepository(_dbContext, _mapper);
        }

        public IActionResult Index(string keyword, int selectType)
        {
            var data = _newRepository.BuildQuery(
                    x => !x.IsDeleted);
            if (keyword != null && keyword != "")
            {
                data = data.Where(x =>
                   EF.Functions.Like(x.Title!, $"%{keyword}%")
                   || EF.Functions.Like(x.Content!, $"%{keyword}%")
               );
            }

            data = data.OrderByDescending(x => x.CreatedAt);
            var result = _mapper.Map<List<NewViewModel>>(data.ToList());
            return View(result);
        }

        //CREATE
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(NewCrudModel newCrudModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var imageUrl = await UploadFileToCloud(ufile: newCrudModel.FileImage);
                    newCrudModel.Image = imageUrl;
                    newCrudModel.IsActive = true;
                    _newRepository.Add(newCrudModel);
                    await _newRepository.CommitAsync();
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    return View("Error");
                }
            }
            else
            {
                return View(newCrudModel);
            }
        }

        //EDIT
        public ActionResult Edit(int id)
        {
            var data = _newRepository.FirstOrDefault(x => x.Id == id);
            var result = _mapper.Map<NewCrudModel>(data);
            var errorModel = new ErrorViewModel();
            if (data == null)
            {
                errorModel.ErrorMessage = "Lỗi không tìm thấy new";
                return View("Error", errorModel);
            }
            else
            {
                return View(result);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(NewCrudModel newCrudModel)
        {
            if (ModelState.IsValid)
            {
                var errorModel = new ErrorViewModel();
                try
                {
                    var news = _newRepository.FirstOrDefault(x => x.Id == newCrudModel.Id);
                    if (news != null)
                    {
                        news.UpdatedAt = DateTime.Now;
                        news.Title = newCrudModel.Title;
                        news.Content = newCrudModel.Content;

                        if (newCrudModel.FileImage != null)
                        {
                            var imageUrl = await UploadFileToCloud(newCrudModel.FileImage);
                            news.Image = imageUrl;
                        }


                        await _newRepository.CommitAsync();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        errorModel.ErrorMessage = "Lỗi không tìm thấy tin tức";
                        return View("Error", errorModel);
                    }
                }
                catch (Exception)
                {
                    errorModel.ErrorMessage = "Lỗi khi chỉnh sửa tin tức";
                    return View("Error", errorModel);
                }
            }
            else
            {
                return View(newCrudModel);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var news = _newRepository.FirstOrDefault(x => x.Id == id);

            if (news != null)
            {
                news.UpdatedAt = DateTime.Now;
                news.IsDeleted = true;
                await _newRepository.CommitAsync();
                return new JsonResult("Xóa thành công tin tức");
            }
            else
            {
                return new JsonResult("Không tìm thấy tin tức");
            }
        }


        private async Task<String> UploadFileToCloud(IFormFile ufile)
        {
            var image = UploadFile.AddPhoto(ufile, "new");
            return image;
        }

        [HttpPost]
        public async Task<IActionResult> SetStatus(int id)
        {
            var news = _newRepository.FirstOrDefault(x => x.Id == id);

            if (news != null)
            {
                news.UpdatedAt = DateTime.Now;
                news.IsActive = !news.IsActive;
                await _newRepository.CommitAsync();
                return new JsonResult("Thay đổi trạng thái thành công");
            }
            else
            {
                return new JsonResult("Không tìm thấy tin tức");
            }
        }

    }
}
