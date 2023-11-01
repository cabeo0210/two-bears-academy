using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Reflection;

namespace EcommerceCore.Const
{
    public static class SystemDisplay
    {
        public static string LogoFavicon = "logo_favicon.jpg";
        public static string LogoFaviconLight = "favicon-light.png";
        public static string LogoFaviconDark = "favicon-dark.png";
        public static string LogoText = "logo_text.png";
        public static string DisplayToMainCenter = "toMainCenter";
        public static string DisplayThumnailImageMedium = "thumnailImageMedium";
        public static string DisplayThumnailImageSmall = "thumnailImageSmall";
        public static string DisplayThumnailImageLarge = "thumnailImageLarge";
        public static string DisplayBannerImage = "thumnailBanner";
    }

    public static class SystemText
    {
        public static string RequiredSpan = "<span class='text-danger'>(*)</span>";
        public static string TextDanger = "text-danger";
    }


    public static class SystemCardTableClass
    {
        public static string ClassParent = "card block-cover";
        public static string ClassHeader = "card-header with-border py-3";
        public static string ClassHeaderTitle = "card-title LightDark";
        public static string ClassHeaderControls = "card-controls pull-right";
        public static string ClassBody = "card-body";
    }

    public static class SystemStyleCard
    {
        public static string Card = "card";
        public static string CardHeader = "card-header bg-primary py-3";
        public static string CardBody = "card-body";
    }

    public static class SystemStyleForm
    {
        public static string ThemeForm = "form theme-form";
        public static string ThemeFooter = "card-footer text-end";
    }
}
