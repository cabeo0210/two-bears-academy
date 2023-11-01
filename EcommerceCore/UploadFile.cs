using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;

namespace Ecommerce
{
    public static class UploadFile
    {
        public static string AddPhoto(IFormFile file, string folder, int width = 500, int height = 500)
        {
            var acc = new Account
            (
                "dmpnbbnwk",
                "877785941692123",
                "zOtFd_I4Wb7nI0A08OxCu4X54Hc"
            );
            var _cloudinary = new Cloudinary(acc);
            try
            {
                var uploadResult = new ImageUploadResult();

                if (file.Length > 0)
                {
                    using var stream = file.OpenReadStream();

                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(file.FileName, stream),
                        Transformation = new Transformation().Height(height).Width(width).Crop("fill"),
                        Folder = folder
                    };

                    uploadResult = _cloudinary.UploadAsync(uploadParams).Result;
                }

                return uploadResult.SecureUrl.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


    }
}