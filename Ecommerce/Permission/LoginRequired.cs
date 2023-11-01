using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2016.Excel;
using Ecommerce.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.Permission
{
    public class LoginRequired : ActionFilterAttribute, IActionFilter
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext context = filterContext.HttpContext;
            var currentAuthentication = context.Session.GetCurrentAuthentication();
            base.OnActionExecuting(filterContext);
            if (currentAuthentication == null)
            {
               
                filterContext.Result = new RedirectResult("/user/login");
                return;
            }
        }
    }
}
