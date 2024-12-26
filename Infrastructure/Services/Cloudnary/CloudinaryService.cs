using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Infrastructure.Services
{
    public class CloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        public string UploadImage(IFormFile image)
        {
            if (image == null || image.Length == 0)
                return null;
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(image.FileName,image.OpenReadStream())
            };

            var uploadResult = _cloudinary.Upload(uploadParams);
            return uploadResult.SecureUrl.AbsoluteUri;  // 返回圖片的 URL
        }
    }
}
