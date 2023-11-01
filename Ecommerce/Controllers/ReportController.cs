using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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
using ClosedXML.Excel;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using Microsoft.AspNetCore.Components.RenderTree;
using DocumentFormat.OpenXml.Spreadsheet;
using EcommerceCore.Const;

namespace Ecommerce.Controllers
{

    public class ReportController : Controller
    {

        private EcommerceDbContext _dbContext;
        private readonly IMapper _mapper;
        private OrderRepository _orderRepository;
        private UserRepository _userRepository;
        private OrderItemRepository _orderItemRepository;
        private ProductRepository _productRepository;
        private readonly IWebHostEnvironment _env;
        public ReportController(EcommerceDbContext dbContext, IMapper mapper, IWebHostEnvironment env)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _orderRepository = new OrderRepository(_dbContext, _mapper);
            _userRepository = new UserRepository(_dbContext, _mapper);
            _orderItemRepository = new OrderItemRepository(_dbContext, _mapper);
            _productRepository = new ProductRepository(_dbContext, _mapper);
            _env = env;
        }
        public IActionResult Index(
            string keyword,
            DateTime? startTime,
            DateTime? endTime
            )
        {
            List<ProductViewModel> productsVM = new List<ProductViewModel>();
            var data = _orderRepository.BuildQuery(
                    x => !x.IsDeleted
                    && x.ShippingStatus == (int)SysEnum.ShippingStatus.Completed
                    && x.OrderStatus == (int)SysEnum.OrderStatus.Completed
                    && x.PaymentStatus == (int)SysEnum.PaymentStatus.Completed
                    );
            if (keyword != null && keyword != "")
            {
                data = data.Where(x =>
                   EF.Functions.Like(x.Code!, $"%{keyword}%")
               );
            }
            if (startTime != null && startTime.HasValue && endTime != null && endTime.HasValue)
            {
                data = data.Where(x => x.CreatedAt >= startTime && x.CreatedAt <= endTime);
            }
           
            data = data
                .Include(x => x.User)
                .Include(x => x.OrderItems)
                    .ThenInclude(x => x.Product);
            data = data.OrderByDescending(x => x.CreatedAt);
            var result = _mapper.Map<List<OrderViewModel>>(data.ToList());

            var orderItems = _orderItemRepository.BuildQuery(x => x.Product != null).ToList();
            if (orderItems != null && orderItems.Any())
            {
                var productIds = (orderItems.GroupBy(item => item.ProductId)
                        .Select(itemGroup => new { Item = itemGroup.Key, Count = itemGroup.Count() })
                        .OrderByDescending(Item => Item.Count)
                        .ThenBy(Item => Item.Item)
                     ).ToList();
                //.Select(x => x.Item);
                if (productIds != null && productIds.Any())
                {
                    foreach (var item in productIds)
                    {
                        var productItem = _productRepository.FirstOrDefault(x => x.Id == item.Item);
                        var productItemVM = _mapper.Map<ProductViewModel>(productItem);
                        productItemVM.Purchases = item.Count;
                        productsVM.Add(productItemVM);
                    }
                }
                //var products = _productRepository.BuildQuery(x => productIds.Contains(x.Id)).ToList();

                //if(products != null && products.Any())
                //{
                //    productsVM = _mapper.Map<List<ProductViewModel>>(products);

                //}

            }
            ViewData["TopProducts"] = productsVM;
            return View(result);
        }

        public IActionResult ExportExcel(
            DateTime? startTime,
            DateTime? endTime
            )
        {
            try
            {
                var data = _orderRepository.BuildQuery(
                   x => !x.IsDeleted
                   && x.ShippingStatus == (int)SysEnum.ShippingStatus.Completed
                   && x.OrderStatus == (int)SysEnum.OrderStatus.Completed
                   && x.PaymentStatus == (int)SysEnum.PaymentStatus.Completed
                   );
                if (startTime != null && startTime.HasValue && endTime != null && endTime.HasValue)
                {
                    data = data.Where(x => x.CreatedAt >= startTime && x.CreatedAt <= endTime);
                }
                data = data
                    .Include(x => x.User)
                    .Include(x => x.OrderItems)
                        .ThenInclude(x => x.Product);
                data = data.OrderByDescending(x => x.CreatedAt);
                var result = _mapper.Map<List<OrderViewModel>>(data.ToList());
                var listExport = result.ToList().Select(x => new
                {
                    Ma_don_hang = x.Code,
                    Khach_hang = x.User.Name,
                    Dia_chi = x.Address,
                    Ngay_tao = x.CreatedAt.ToString("dd-mm-yyyy"),
                    San_pham = GetProductName(x),
                }).ToList();

                var workbook = new XLWorkbook();
                IXLWorksheet worksheet = workbook.Worksheets.Add("MIO-Report");

                var allProperty = listExport[0].GetType().GetProperties();
                int index = 1;
                foreach (var pro in allProperty)
                {
                    worksheet.Cell(1, index).Value = pro.Name;
                    index++;
                }
                for (index = 1; index <= listExport.Count; index++)
                {
                    int i = 1;
                    foreach (var pro in allProperty)
                    {
                        var valueObject = listExport[index - 1].GetType().GetProperty(pro.Name);
                        worksheet.Cell(index + 1, i).Value = valueObject.GetValue(listExport[index - 1]);
                        i++;
                    }
                }
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    string fileName = "Profit-Report-" + Guid.NewGuid().ToString();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName + ".xlsx");
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

        }

        private string GetProductName(OrderViewModel order)
        {
            List<string> listProductName = new List<string>();

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
            return productNameResult;

        }

    }
}
