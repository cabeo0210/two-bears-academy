using AutoMapper;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using EcommerceCore.Const;
using Ecommerce.Helper;
using EcommerceCore.Models;
using Ecommerce.Repositories;
using EcommerceCore.ViewModel.Image;
using EcommerceCore.ViewModel.Product;
using EcommerceCore.ViewModel.ProductFeedback;
using EcommerceCore.ViewModel.ProductHistory;
using EcommerceEndUser.Permission;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace Ecommerce.EndUser.Controllers
{
    public class ProductEndUserController : Controller
    {
        private readonly ProductRepository _productRepository;
        private readonly ProductImageRepository _productImageRepository;
        private readonly ImageRepository _imageRepository;
        private readonly ProductFeedbackRepository _productFeedbackRepository;
        private readonly OrderRepository _orderRepository;
        private readonly ProductHistoryRepository _productHistoryRepository;
        private EcommerceDbContext _dbContext;
        private readonly IMapper _mapper;
        public ProductEndUserController(EcommerceDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _productRepository = new ProductRepository(_dbContext, _mapper);
            _productImageRepository = new ProductImageRepository(_dbContext, _mapper);
            _imageRepository = new ImageRepository(_dbContext, _mapper);
            _productFeedbackRepository = new ProductFeedbackRepository(_dbContext, _mapper);
            _orderRepository = new OrderRepository(_dbContext, _mapper);
            _productHistoryRepository = new ProductHistoryRepository(_dbContext, _mapper);
        }
        public async Task<IActionResult> Index(int id)
        {


            //Get product
            var data = _productRepository.FirstOrDefault(x => x.Id == id);
            var result = _mapper.Map<ProductViewModel>(data);
            //Get image product
            var imageData = _productImageRepository.BuildQuery(x => x.ProductId == id).Select(x => x.ImageId).ToList();
            var listProductImage = _imageRepository.BuildQuery(x => imageData.Contains(x.Id)).Select(z => _mapper.Map<ImageViewModel>(z)).ToList();
            if (listProductImage != null && listProductImage.Count() > 0)
            {
                result.ListProductImage = listProductImage;
            }
            else
            {
                result.ListProductImage = new List<ImageViewModel>();
            }

            //Get list feedback of this product
            var productFeedback = new ProductFeedbackCrudModel();
            productFeedback.ProductId = id;
            result.ProductFeedback = productFeedback;
            var listProductFeedback = _productFeedbackRepository.BuildQuery(x => x.ProductId == id
            && x.IsActive
            && !x.IsDeleted
            && x.Type == (int)SysEnum.ProductFeedbackType.Content)
                .Include(x => x.User)
                .Select(x => _mapper.Map<ProductFeedbackViewModel>(x))
                .ToList();
            if (listProductFeedback != null && listProductFeedback.Count() > 0)
            {
                result.ListProductFeedback = listProductFeedback;
            }
            else
            {
                result.ListProductFeedback = new List<ProductFeedbackViewModel>();
            }
            //get user
            var user = HttpContext.Session.GetCurrentAuthentication();
            if (user != null)
            {
                //Check if user is buy product then they can add feedback about the product 
                var userOrder = _orderRepository.BuildQuery(x => x.CreateByUserId == user.UserId && x.IsActive && !x.IsDeleted && x.OrderItems.Any(y => y.ProductId == id) && x.OrderStatus == (int)SysEnum.OrderStatus.Completed)
                    .Include(x => x.OrderItems)
                        .ThenInclude(x => x.Product)
                    .FirstOrDefault();
                if (userOrder != null)
                {
                    result.IsBought = true;
                }
                //Add viewed product
                await CreateProductHistory(user.UserId, id);

                //Check user rating
                var userRating = _productFeedbackRepository.BuildQuery(x => x.ProductId == id && x.Type == (int)SysEnum.ProductFeedbackType.Rating);
                if (userRating != null && userRating.Count() > 0)
                {   //Get how many star that user rating
                    result.UserRatingCount = Int32.Parse(userRating.FirstOrDefault(x => x.CreateByUserId == user.UserId) == null ? "0" : userRating.FirstOrDefault(x => x.CreateByUserId == user.UserId).Content);
                    //Get how many review that product rated
                    result.ProductRatingCount = userRating.Count();
                }
            }

            return View(result);
        }

        public IActionResult ProductHistory(int userId)
        {
            var productHistory = _productHistoryRepository.BuildQuery(x => x.UserId == userId)
                .Include(x => x.Product)
                    .ThenInclude(x => x.ProductImages)
                        .ThenInclude(x => x.Image)
                .Select(x => _mapper.Map<ProductHistoryViewModel>(x))
                .ToList();
            return View(productHistory);
        }

        private async Task<IActionResult> CreateProductHistory(int userId, int productId)
        {

            var isExist = _productHistoryRepository.FirstOrDefault(x => x.UserId == userId && x.ProductId == productId);
            if (isExist != null)
            {
                //do nothing
                return Ok();
            }
            else
            {
                var productHistory = new ProductHistoryCrudModel
                {
                    ProductId = productId,
                    UserId = userId,
                    IsActive = true,
                };
                _productHistoryRepository.Add(productHistory);
                try
                {
                    await _productHistoryRepository.CommitAsync();
                    return Ok();
                }
                catch (Exception e)
                {
                    return RedirectToAction("Index", new { id = productId });
                }
            }

        }

        [LoginRequired]
        public IActionResult CreateProductFeedback(int id)
        {
            return RedirectToAction("Index", new { id = id });
        }

        [HttpPost]
        public async Task<IActionResult> CreateProductFeedback(ProductFeedbackCrudModel productFeedbackCrudModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    productFeedbackCrudModel.CreateByUserId = HttpContext.Session.GetCurrentAuthentication().UserId;
                    productFeedbackCrudModel.IsActive = true;
                    _productFeedbackRepository.Add(productFeedbackCrudModel);
                    await _productRepository.CommitAsync(); ;

                    return RedirectToAction("Index", new { id = productFeedbackCrudModel.ProductId });
                    //return new JsonResult("Gửi feedback thành công");
                }
                catch (Exception)
                {
                    return View("Error");
                }
            }
            else
            {
                return RedirectToAction("Index", new { id = productFeedbackCrudModel.Id });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateProductRatingFeedback(int id, string content)
        {
            var user = HttpContext.Session.GetCurrentAuthentication();
            if (user != null)
            {
                try
                {
                    var productFeedback = new ProductFeedbackCrudModel
                    {
                        ProductId = id,
                        CreateByUserId = user.UserId,
                        Type = (int)SysEnum.ProductFeedbackType.Rating,
                        IsActive = true,
                        Content = content,
                    };
                    _productFeedbackRepository.Add(productFeedback);
                    await _productRepository.CommitAsync();

                    return RedirectToAction("Index", new { id = id });
                }
                catch (Exception)
                {
                    return View("Error");
                }
            }
            else
            {
                return Ok();
            }

        }
    }
}