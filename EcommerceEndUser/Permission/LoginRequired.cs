using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2016.Excel;
using EcommerceCore.Const;
using Ecommerce.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceEndUser.Permission
{
    public class LoginRequired : ActionFilterAttribute, IActionFilter
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext context = filterContext.HttpContext;
            HttpRequest request = filterContext.HttpContext.Request;
            var currentAuthentication = context.Session.GetCurrentAuthentication();
            base.OnActionExecuting(filterContext);
            if (currentAuthentication == null)
            {
                if (request.Method == "GET")
                {
                    context.Session.SetString(TextConstant.LastRequestURL, request.GetDisplayUrl());
                }
                filterContext.Result = new RedirectResult("/HomeEndUser/Login");
                return;
            }
        }
    }
}
