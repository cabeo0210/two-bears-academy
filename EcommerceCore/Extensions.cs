using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Security.Cryptography;
using System.Dynamic;
using Microsoft.SharePoint.Client.Search.Query;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Http;

namespace EcommerceCore
{
    public static class Extensions
    {
        public static string GetEnumDisplayName(this Enum enumType)
        {
            return enumType.GetType().GetMember(enumType.ToString())
                           .First()
                           .GetCustomAttribute<DisplayAttribute>()
                           .Name;
        }
        public static string Hash(this string value)
        {
            return Convert.ToBase64String(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(value)));
        }
        public static string ConvertToVND(this decimal price)
        {
            var info = System.Globalization.CultureInfo.GetCultureInfo("vi-VN");
            if (price >= 0)
            {
                return String.Format(info, "{0:c}", price);
            }
            return "Broooooo check your input price";
        }

    }
}
