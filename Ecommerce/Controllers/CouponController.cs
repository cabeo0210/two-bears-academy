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
using EcommerceCore.ViewModel.Coupon;
using EcommerceCore.ViewModel.Order;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
namespace Ecommerce.Controllers
{

    public class CouponController : Controller
    {
        private EcommerceDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly CouponRepository _couponRepository;
        private readonly CouponHistoryRepository _couponHistoryRepository;
        private readonly ProductRepository _productRepository;

        public CouponController(EcommerceDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _productRepository = new ProductRepository(_dbContext, _mapper);
            _couponRepository = new CouponRepository(_dbContext, _mapper);
            _couponHistoryRepository = new CouponHistoryRepository(_dbContext, _mapper);
        }

        public IActionResult Index(string keyword, int selectType)
        {
            var data = _couponRepository.BuildQuery(
                    x => !x.IsDeleted);
            if (keyword != null && keyword != "")
            {
                data = data.Where(x =>
                   EF.Functions.Like(x.Code!, $"%{keyword}%")
                   || EF.Functions.Like(x.Name!, $"%{keyword}%")
               );
            }

            switch (selectType)
            {
                case (int)SysEnum.CouponStatus.Happening:
                    data = data.Where(x => x.TimeStart.Date <= DateTime.Now.Date && x.TimeEnd.Date >= DateTime.Now.Date);
                    break;
                case (int)SysEnum.CouponStatus.End:
                    data = data.Where(x => x.TimeEnd.Date <= DateTime.Now.Date);
                    break;
                default:
                    break;
            }

            data = data.OrderByDescending(x => x.CreatedAt);
            var result = _mapper.Map<List<CouponViewModel>>(data.ToList());
            return View(result);
        }

        //CREATE
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CouponCrudModel couponCrudModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    couponCrudModel.IsActive = true;
                    _couponRepository.Add(couponCrudModel);
                    await _couponRepository.CommitAsync();
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    return View("Error");
                }
            }
            else
            {
                return View(couponCrudModel);
            }
        }

        //EDIT
        public ActionResult Edit(int id)
        {
            var data = _couponRepository.FirstOrDefault(x => x.Id == id);
            var result = _mapper.Map<CouponCrudModel>(data);
            var errorModel = new ErrorViewModel();
            if (data == null)
            {
                errorModel.ErrorMessage = "Lỗi không tìm thấy coupon";
                return View("Error", errorModel);
            }
            else
            {
                return View(result);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CouponCrudModel couponCrudModel)
        {
            if (ModelState.IsValid)
            {
                var errorModel = new ErrorViewModel();
                try
                {
                    var coupon = _couponRepository.FirstOrDefault(x => x.Id == couponCrudModel.Id);
                    if (coupon != null)
                    {
                        coupon.UpdatedAt = DateTime.Now;
                        coupon.Name = couponCrudModel.Name;
                        coupon.Code = couponCrudModel.Code;
                        coupon.LimitationTimes = couponCrudModel.LimitationTimes;
                        coupon.CouponPriceType = couponCrudModel.CouponPriceType;
                        coupon.CouponPriceValue = couponCrudModel.CouponPriceValue;
                        coupon.TimeStart = couponCrudModel.TimeStart;
                        coupon.TimeEnd = couponCrudModel.TimeEnd;
                        await _couponRepository.CommitAsync();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        errorModel.ErrorMessage = "Lỗi không tìm thấy mã khuyến mãi";
                        return View("Error", errorModel);
                    }
                }
                catch (Exception)
                {
                    errorModel.ErrorMessage = "Lỗi khi chỉnh sửa mã khuyến mãi";
                    return View("Error", errorModel);
                }
            }
            else
            {
                return View(couponCrudModel);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var coupon = _couponRepository.FirstOrDefault(x => x.Id == id);

            if (coupon != null)
            {
                coupon.UpdatedAt = DateTime.Now;
                coupon.IsDeleted = true;
                await _couponRepository.CommitAsync();
                return new JsonResult("Xóa thành công mã khuyến mãi");
            }
            else
            {
                return new JsonResult("Không tìm thấy mã khuyến mãi");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetStatus(int id)
        {
            var coupon = _couponRepository.FirstOrDefault(x => x.Id == id);

            if (coupon != null)
            {
                coupon.UpdatedAt = DateTime.Now;
                coupon.IsActive = !coupon.IsActive;
                await _couponRepository.CommitAsync();
                return new JsonResult("Thay đổi trạng thái thành công");
            }
            else
            {
                return new JsonResult("Không tìm thấy mã khuyến mãi");
            }
        }
    }
}
