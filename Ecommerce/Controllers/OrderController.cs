using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EcommerceCore.Const;
using EcommerceCore.Models;
using EcommerceCore.Models ;
using EcommerceCore.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Reflection.PortableExecutable;
using System.Net.Mail;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Reflection.Metadata;
using System.Text;
using System.Net;
using EcommerceCore.ViewModel.User;
using Ecommerce.Repositories;
using EcommerceCore.ViewModel.Order;
using EcommerceCore.ViewModel.Category;
using EcommerceCore.ViewModel.Product;
using static EcommerceCore.Const.SysEnum;
using Microsoft.AspNetCore;
using Microsoft.SharePoint.Client;
using BitMiracle.LibTiff.Classic;

namespace Ecommerce.Controllers
{

    public class OrderController : Controller
    {

        private EcommerceDbContext _dbContext;
        private readonly IMapper _mapper;
        private OrderRepository _orderRepository;
        private UserRepository _userRepository;
        private OrderItemRepository _orderItemRepository;
        private ProductRepository _productRepository;
        private readonly IWebHostEnvironment _env;
        public OrderController(EcommerceDbContext dbContext, IMapper mapper, IWebHostEnvironment env)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _orderRepository = new OrderRepository(_dbContext, _mapper);
            _userRepository = new UserRepository(_dbContext, _mapper);
            _orderItemRepository = new OrderItemRepository(_dbContext, _mapper);
            _productRepository = new ProductRepository(_dbContext, _mapper);
            _env = env;
        }
        public IActionResult Index(string keyword,
            int? paymentStatus,
            int? orderStatus,
            int? shippingStatus
            )
        {
            var data = _orderRepository.BuildQuery(
                    x => !x.IsDeleted);
            if (keyword != null && keyword != "")
            {
                data = data.Where(x =>
                   EF.Functions.Like(x.Code!, $"%{keyword}%")
               );
            }
            if (paymentStatus != null)
            {
                data = data.Where(x => x.PaymentStatus == paymentStatus);
            }
            if (orderStatus != null)
            {
                data = data.Where(x => x.OrderStatus == orderStatus);
            }
            if (shippingStatus != null)
            {
                data = data.Where(x => x.ShippingStatus == shippingStatus);
            }
            data = data
                .Include(x => x.OrderItems)
                    .ThenInclude(x => x.Product);
            data = data.OrderByDescending(x => x.CreatedAt);
            var result = _mapper.Map<List<OrderViewModel>>(data.ToList());
            return View(result);
        }
        public IActionResult Create()
        {
            var model = new OrderCrudModel();
            model.ListUserViewModel = _userRepository
                .Where(x =>
                !x.IsDeleted
                && x.Role == (int)SysEnum.Role.EndUser
                && x.IsActive)
                .Select(x => _mapper.Map<UserViewModel>(x))
                .ToList();
            var product = _productRepository
                .BuildQuery(x => !x.IsDeleted
                && x.Quantity > 0)
                .Select(x => _mapper.Map<ProductViewModel>(x))
                .ToList();
            model.ListProductViewModel = product;
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Create(OrderCrudModel model)
        {
            //model.Code = "OD" + DateTime.Now.ToString("dd/MM/yyyy") + model.CreateByUserId;
            //model.Name = "";
            //_orderRepository.Add(model);

            Order order = new()
            {
                Name = "",
                Code =   "OD" + DateTime.Now.ToString("yyyyMMddHHmmss") + model.CreateByUserId,
                PaymentStatus = model.PaymentStatus,
                ShippingStatus = model.ShippingStatus,
                CreateByUserId = model.CreateByUserId,
                OrderStatus = model.OrderStatus,
                Address = model.Address,
            };
            await _orderRepository.AddAsync(order);
            await _orderRepository.CommitAsync();
            var productIdsData = model.ProductIds.Split(",");
            foreach (var item in productIdsData)
            {
                var convertToInt = Int32.Parse(item);
                var productItem = _productRepository.GetById(convertToInt);
                if (productItem != null)
                {
                    var oderItem = new OrderItem()
                    {
                        ProductId = productItem.Id,
                        Price = productItem.SellPrice,
                        PriceTotal = productItem.SellPrice,
                        Quantity = 1,
                        Order = order,
                        OrderId = order.Id,
                    };
                    await _orderItemRepository.AddAsync(oderItem);
                    order.Total += productItem.SellPrice;
                }
            }
            await _orderItemRepository.CommitAsync();
            return RedirectToAction("Index");
        }

        public IActionResult Update(int orderId)
        {
            var model = new OrderCrudModel();
            var order = _orderRepository
               .BuildQuery(x => x.Id == orderId)
               .Include(x => x.OrderItems)
                   .ThenInclude(x => x.Product)
               .FirstOrDefault();
            if (order != null)
            {
                model = _mapper.Map<OrderCrudModel>(order);
            }
            model.ListUserViewModel = _userRepository
                .Where(x =>
                !x.IsDeleted
                && x.Role == (int)SysEnum.Role.EndUser
                && x.IsActive)
                .Select(x => _mapper.Map<UserViewModel>(x))
                .ToList();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Update(OrderCrudModel model)
        {
            var order = _orderRepository
              .BuildQuery(x => x.Id == model.Id)
              .FirstOrDefault();
            if (order != null)
            {
                order.Address = model.Address;
                order.IsActive = true;
                order.PaymentStatus = model.PaymentStatus;
                order.ShippingStatus = model.ShippingStatus;
                order.OrderStatus = model.OrderStatus;
                order.CreateByUserId = model.CreateByUserId;
            }
            await _orderRepository.CommitAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int orderId)
        {
            var order = _orderRepository
              .BuildQuery(x => x.Id == orderId)
              .FirstOrDefault();
            if (order != null)
            {
                order.IsDeleted = true;
            }
            await _orderRepository.CommitAsync();
            Response.StatusCode = 200;
            return new JsonResult("Xóa thành công");
        }
        public async Task<IActionResult> GeneratePDF(int orderId)
        {
            List<string> listProductName = new List<string>();
            var order = _orderRepository
              .BuildQuery(x => x.Id == orderId)
              .Include(x => x.OrderItems)
              .ThenInclude(x => x.Product)
              .FirstOrDefault();
            if (order != null)
            {
                var user = _userRepository.GetById(order.CreateByUserId.Value);
                if (order.OrderItems != null)
                {
                    foreach (var orderItem in order.OrderItems)
                    {
                        var products = _productRepository.FirstOrDefault(x => x.Id == orderItem.ProductId);
                        if (products != null)
                        {
                            listProductName.Add(products.Name);
                        }
                    }
                }
                var productNameResult = String.Join(", ", listProductName.ToArray());
                var webRoot = _env.WebRootPath;
                var orderTemplate = Path.Combine(webRoot, "template/order_template.html");
                var orderTemplateBody = System.IO.File.ReadAllText(orderTemplate);
                orderTemplateBody = orderTemplateBody
                    .Replace("{{orderCode}}", order.Code!.ToString())
                    .Replace("{{userName}}", user.Name)
                    .Replace("{{address}}", order.Address)
                    .Replace("{{productName}}", productNameResult);
                var renderer = new HtmlToPdf();
                renderer.RenderHtmlAsPdf(orderTemplateBody).SaveAs("Order.pdf");

            }
            return RedirectToAction("Index");
        }


    }
}
