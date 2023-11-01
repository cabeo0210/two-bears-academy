using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EcommerceCore.Const;
using EcommerceCore.Models;
using EcommerceCore.Models;
using EcommerceCore.Models ;
using EcommerceCore.Models ;
using EcommerceCore.Models ;
using EcommerceCore.Models ;
using Ecommerce.Repositories;
using EcommerceCore.ViewModel;
using EcommerceCore.ViewModel.Category;
using EcommerceCore.ViewModel.Image;
using EcommerceCore.ViewModel.Product;
using EcommerceCore.ViewModel.ProductCategory;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using sib_api_v3_sdk.Model;

namespace Ecommerce.Controllers
{

    public class ProductController : Controller
    {
        private EcommerceDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ProductRepository _productRepository;
        private readonly ProductImageRepository _productImageRepository;
        private readonly ImageRepository _imageRepository;

        public ProductController(EcommerceDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _productRepository = new ProductRepository(_dbContext, _mapper);
            _productImageRepository = new ProductImageRepository(_dbContext, _mapper);
            _imageRepository = new ImageRepository(_dbContext, _mapper);
        }

        public ActionResult Index(string keyword, int? selectType)
        {

            var data = _productRepository.BuildQuery(
             x => !x.IsDeleted);
            if (keyword != null && keyword != "")
            {
                data = data.Where(x =>
                   EF.Functions.Like(x.Code!, $"%{keyword}%")
               );
            }

            if (selectType.HasValue)
            {
                data = data.Where(x => x.Status == selectType.Value);
            }

            data = data
                .Include(x => x.Category);
            data = data.OrderByDescending(x => x.CreatedAt);
            var result = _mapper.Map<List<ProductViewModel>>(data.ToList());
            return View(result);
        }

        //CREATE
        public ActionResult Create()
        {
            var model = new ProductCrudModel();
            var errorModel = new ErrorViewModel();
            model.ListCategoryViewModel = _dbContext.Categories.Where(x => !x.IsDeleted && x.IsActive).Select(x => _mapper.Map<CategoryViewModel>(x)).ToList();
            if (model.ListCategoryViewModel != null || model.ListCategoryViewModel.Count() != 0)
            {
                return View(model);
            }
            else
            {
                errorModel.ErrorMessage = "Bạn cần phải tạo danh mục trước!";
                return View("Error", errorModel);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductCrudModel productCrudModel)
        {
            if (ModelState.IsValid)
            {
                var errorModel = new ErrorViewModel();
                try
                {
                    //var uploadResult = await UploadFileToCloud(ufile: productCrudModel.FileImage);
                    _productRepository.Add(productCrudModel);
                    await _productRepository.CommitAsync(); ;

                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    errorModel.ErrorMessage = "Lỗi khi tạo sản phẩm";
                    return View("Error", errorModel);
                }
            }
            else
            {
                return View(productCrudModel);
            }
        }

        //EDIT
        public ActionResult Edit(int id)
        {
            var errorModel = new ErrorViewModel();
            var data = _productRepository.FirstOrDefault(x => x.Id == id);
            var result = _mapper.Map<ProductCrudModel>(data);
            result.ListCategoryViewModel = _dbContext.Categories.Where(x => !x.IsDeleted && x.IsActive).Select(x => _mapper.Map<CategoryViewModel>(x)).ToList();
            if (data == null)
            {
                errorModel.ErrorMessage = "Lỗi không tìm thấy sản phẩm";
                return View("Error", errorModel);
            }
            else
            {
                return View(result);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductCrudModel productCrudModel)
        {
            if (ModelState.IsValid)
            {
                var errorModel = new ErrorViewModel();
                try
                {
                    var product = _productRepository.FirstOrDefault(x => x.Id == productCrudModel.Id);
                    if (product != null)
                    {
                        product.UpdatedAt = DateTime.Now;
                        product.Name = productCrudModel.Name;
                        product.IsActive = productCrudModel.IsActive;
                        product.IsDeleted = false;
                        product.Description = productCrudModel.Description;
                        product.BasePrice = productCrudModel.BasePrice;
                        product.Code = productCrudModel.Code;
                        product.Status = productCrudModel.Status;
                        if (productCrudModel.CategoryId != null)
                        {
                            product.CategoryId = productCrudModel.CategoryId;
                        }
                        await _productRepository.CommitAsync();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        errorModel.ErrorMessage = "Lỗi không tìm thấy sản phẩm";
                        return View("Error", errorModel);
                    }
                }
                catch (Exception)
                {
                    errorModel.ErrorMessage = "Lỗi khi chỉnh sửa sản phẩm";
                    return View("Error", errorModel);
                }
            }
            else
            {
                return View(productCrudModel);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var product = _productRepository.FirstOrDefault(x => x.Id == id);
            if (product != null)
            {
                product.UpdatedAt = DateTime.Now;
                product.IsDeleted = true;
                await _productRepository.CommitAsync();
                return new JsonResult("Xóa sản phẩm thành công"); ;
            }
            else
            {
                return new JsonResult("Không tìm thấy sản phẩm");
            }
        }

        private async Task<String> UploadFileToCloud(IFormFile ufile)
        {

            var image = UploadFile.AddPhoto(ufile, "product");
            return image;
        }

        //IMAGES
        public ActionResult ProductImages(int productId)
        {
            var imageData = _productImageRepository.BuildQuery(x => x.ProductId == productId).Select(x => x.ImageId).ToList();
            var result = _imageRepository.BuildQuery(x => imageData.Contains(x.Id)).Select(z => _mapper.Map<ImageViewModel>(z)).ToList();
            ViewData["ProductId"] = productId;
            return View(result);
        }

        //CREATE PRODUCT IMAGE
        public ActionResult CreateProductImages(int productId)
        {
            var modelError = new ErrorViewModel();
            var data = _productRepository.FirstOrDefault(x => x.Id == productId);
            if (data != null)
            {
                var result = new ImageCrudModel();
                result.ProductId = data.Id;
                return View(result);
            }
            else
            {
                modelError.ErrorMessage = "Lỗi không tìm thấy sản phẩm";
                return View("Error", modelError);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateProductImages(ImageCrudModel imageCrudModel)
        {
            if (ModelState.IsValid)
            {
                var errorModel = new ErrorViewModel();
                try
                {
                    var uploadResult = await UploadFileToCloud(ufile: imageCrudModel.FileImage);
                    imageCrudModel.Url = uploadResult;
                    imageCrudModel.IsActive = true;
                    _imageRepository.Add(imageCrudModel);
                    await _imageRepository.CommitAsync();

                    var productImage = new ProductImageCrudModel { Image = imageCrudModel, ProductId = imageCrudModel.ProductId };
                    _productImageRepository.Add(productImage);
                    await _productImageRepository.CommitAsync();

                    return RedirectToAction("ProductImages", new { productId = imageCrudModel.ProductId });
                }
                catch (Exception)
                {
                    errorModel.ErrorMessage = "Lỗi khi tạo hình ảnh sản phẩm";
                    return View("Error", errorModel);
                }
            }
            else
            {
                return View(imageCrudModel);
            }
        }


        //EDIT PRODUCT IMAGE
        public ActionResult EditProductImages(int id, int productId)
        {
            var errorModel = new ErrorViewModel();
            var data = _imageRepository.FirstOrDefault(x => x.Id == id);
            var productDb = _productRepository.FirstOrDefault(x => x.Id == productId);
            var result = _mapper.Map<ImageCrudModel>(data);
            result.ProductId = productDb.Id;
            if (data == null)
            {
                errorModel.ErrorMessage = "Lỗi không tìm thấy sản phẩm";
                return View("Error", errorModel);
            }
            else
            {
                return View(result);
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditProductImages(ImageCrudModel imageCrudModel)
        {
            if (ModelState.IsValid)
            {
                var errorModel = new ErrorViewModel();
                try
                {
                    if (imageCrudModel.FileImage != null)
                    {
                        var imageFromDb = _imageRepository.FirstOrDefault(x => x.Id == imageCrudModel.Id);
                        var uploadResult = await UploadFileToCloud(ufile: imageCrudModel.FileImage);
                        imageFromDb.Url = uploadResult;
                        imageFromDb.UpdatedAt = DateTime.Now;
                        await _imageRepository.CommitAsync();
                    }
                    await _productImageRepository.CommitAsync();
                    return RedirectToAction("ProductImages", new { productId = imageCrudModel.ProductId });
                }
                catch (Exception)
                {
                    errorModel.ErrorMessage = "Lỗi khi chỉnh sửa hình ảnh sản phẩm";
                    return View("Error", errorModel);
                }
            }
            else
            {
                return View(imageCrudModel);
            }
        }

        //DELETE PRODUCT IMAGE
        [HttpPost]
        public async Task<IActionResult> DeleteProductImages(int id)
        {
            var productImageFromDb = _productImageRepository.BuildQuery(x => x.ImageId == id).AsNoTracking().FirstOrDefault();
            var data = _mapper.Map<ProductImageCrudModel>(productImageFromDb);
            _productImageRepository.Delete(data);
            await _productImageRepository.CommitAsync();
            return new JsonResult("Xóa hình ảnh sản phẩm thành công");
        }

        [HttpPost]
        public async Task<IActionResult> SetStatus(int id)
        {
            var product = _productRepository.FirstOrDefault(x => x.Id == id);
            if (product != null)
            {
                product.UpdatedAt = DateTime.Now;
                product.IsActive = !product.IsActive;
                await _productRepository.CommitAsync();
                return new JsonResult("Thay đổi trạng thái thành công");
            }
            else
            {
                return new JsonResult("Không tìm thấy sản phẩm");
            }
        }
    }
}
