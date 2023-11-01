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
using EcommerceCore.ViewModel.Setting;
using EcommerceCore.ViewModel.Order;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
namespace Ecommerce.Controllers
{

    public class SettingController : Controller
    {
        private EcommerceDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly SettingRepository _settingRepository;

        public SettingController(EcommerceDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _settingRepository = new SettingRepository(_dbContext, _mapper);
        }

        public IActionResult Index()
        {
            var data = _settingRepository.FirstOrDefault(
                    x => !x.IsDeleted);

            if (data != null)
            {
                var result = _mapper.Map<SettingViewModel>(data);
                return RedirectToAction("Edit", result);
            }
            else
            {
                return RedirectToAction("Create");
            }

        }

        //CREATE
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(SettingCrudModel settingCrudModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    settingCrudModel.IsActive = true;
                    _settingRepository.Add(settingCrudModel);
                    await _settingRepository.CommitAsync();
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    return View("Error");
                }
            }
            else
            {
                return View(settingCrudModel);
            }
        }

        //EDIT
        public ActionResult Edit(int id)
        {
            var data = _settingRepository.FirstOrDefault(x => x.Id == id);
            var result = _mapper.Map<SettingCrudModel>(data);
            var errorModel = new ErrorViewModel();
            if (data == null)
            {
                errorModel.ErrorMessage = "Lỗi không tìm thấy thông tin thanh toán";
                return View("Error", errorModel);
            }
            else
            {
                return View(result);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SettingCrudModel settingCrudModel)
        {
            if (ModelState.IsValid)
            {
                var errorModel = new ErrorViewModel();
                try
                {
                    var setting = _settingRepository.FirstOrDefault(x => x.Id == settingCrudModel.Id);
                    if (setting != null)
                    {
                        setting.UpdatedAt = DateTime.Now;
                        setting.SettingConfig = settingCrudModel.SettingConfig;
                        setting.IsActive = settingCrudModel.IsActive;
                        await _settingRepository.CommitAsync();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        errorModel.ErrorMessage = "Lỗi không tìm thấy thông tin thanh toán";
                        return View("Error", errorModel);
                    }
                }
                catch (Exception)
                {
                    errorModel.ErrorMessage = "Lỗi khi chỉnh sửa thông tin thanh toán";
                    return View("Error", errorModel);
                }
            }
            else
            {
                return View(settingCrudModel);
            }
        }

    }
}
