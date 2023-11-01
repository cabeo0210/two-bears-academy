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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
namespace Ecommerce.Controllers
{

    public class CategoryController : Controller
    {
        private EcommerceDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly CategoryRepository _categoryRepository;
        private readonly ProductRepository _productRepository;

        public CategoryController(EcommerceDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _categoryRepository = new CategoryRepository(_dbContext, _mapper);
            _productRepository = new ProductRepository(_dbContext, _mapper);
        }

        public ActionResult Index()
        {
            var data = _categoryRepository.BuildQuery(x => !x.IsDeleted).Select(x => _mapper.Map<CategoryViewModel>(x)).ToList();
            return View(data);
        }

        //CREATE
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryCrudModel categoryCrudModel)
        {
            if (ModelState.IsValid)
            {
                var errorModel = new ErrorViewModel();
                try
                {
                    _categoryRepository.Add(categoryCrudModel);
                    await _categoryRepository.CommitAsync();
                    //await _dbContext.AddAsync(_mapper.Map<Category>(categoryViewModel));
                    //await _dbContext.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    errorModel.ErrorMessage = "Lỗi khi tạo danh mục";
                    return View("Error", errorModel);
                }
            }
            else
            {
                return View(categoryCrudModel);
            }
        }

        //EDIT
        public ActionResult Edit(int id)
        {
            var data = _categoryRepository.FirstOrDefault(x => x.Id == id);
            var result = _mapper.Map<CategoryCrudModel>(data);
            var errorModel = new ErrorViewModel();
            if (data == null)
            {
                errorModel.ErrorMessage = "Lỗi không tìm thấy danh mục";
                return View("Error", errorModel);
            }
            else
            {
                return View(result);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CategoryCrudModel categoryCrudModel)
        {
            if (ModelState.IsValid)
            {
                var errorModel = new ErrorViewModel();
                try
                {
                    var category = _categoryRepository.FirstOrDefault(x => x.Id == categoryCrudModel.Id);
                    if (category != null)
                    {
                        category.UpdatedAt = DateTime.Now;
                        category.Name = categoryCrudModel.Name;
                        category.IsActive = categoryCrudModel.IsActive;
                        await _categoryRepository.CommitAsync();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        errorModel.ErrorMessage = "Lỗi không tìm thấy danh mục";
                        return View("Error", errorModel);
                    }
                }
                catch (Exception)
                {
                    errorModel.ErrorMessage = "Lỗi khi chỉnh sửa danh mục";
                    return View("Error", errorModel);
                }
            }
            else
            {
                return View(categoryCrudModel);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var category = _categoryRepository.FirstOrDefault(x => x.Id == id);
            var product = _productRepository.FirstOrDefault(x => x.CategoryId == category.Id);

            if (product != null)
            {
                return new JsonResult("Danh mục đang được gán với sản phẩm nên không thể xóa");
            }

            if (category != null)
            {
                category.UpdatedAt = DateTime.Now;
                category.IsDeleted = true;
                await _categoryRepository.CommitAsync();
                return new JsonResult("Xóa thành công danh mục");
            }
            else
            {
                return new JsonResult("Không tìm thấy danh mục");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetStatus(int id)
        {
            var category = _categoryRepository.FirstOrDefault(x => x.Id == id);
            var product = _productRepository.FirstOrDefault(x => x.CategoryId == category.Id);

            if (product != null)
            {
                return new JsonResult("Danh mục đang được gán với sản phẩm nên không thể thay đổi trạng thái");
            }

            if (category != null)
            {
                category.UpdatedAt = DateTime.Now;
                category.IsActive = !category.IsActive;
                await _categoryRepository.CommitAsync();
                return new JsonResult("Thay đổi trạng thái thành công");
            }
            else
            {
                return new JsonResult("Không tìm thấy danh mục");
            }
        }
    }
}
