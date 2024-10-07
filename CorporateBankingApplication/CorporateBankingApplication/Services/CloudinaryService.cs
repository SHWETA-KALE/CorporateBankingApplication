using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace CorporateBankingApplication.Services
{
    public class CloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService()
        {
            Account account = new Account(
                System.Configuration.ConfigurationManager.AppSettings["CloudinaryCloudName"],
                System.Configuration.ConfigurationManager.AppSettings["CloudinaryApiKey"],
                System.Configuration.ConfigurationManager.AppSettings["CloudinaryApiSecret"]
            );
            _cloudinary = new Cloudinary(account);
        }

        public string UploadClientFile(HttpPostedFileBase file,string clientName)
        {
            if (file == null || file.ContentLength <= 0)
            {
                throw new Exception("File is required");
            }

            //// Upload file to Cloudinary
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, file.InputStream),
                PublicId = $"ClientDocuments/{clientName}/{Path.GetFileNameWithoutExtension(file.FileName)}", // Set without extension
                UseFilename = false, // Don't use the original file name
                UniqueFilename = false,
                Overwrite = true // Overwrite if a file with the same name exists
            };

            var uploadResult = _cloudinary.Upload(uploadParams);

            // Return the URL of the uploaded file
            return uploadResult.SecureUrl.ToString();
        }
        public string UploadBeneficiaryFile(HttpPostedFileBase file, string beneficiaryName)
        {
            if (file == null || file.ContentLength <= 0)
            {
                throw new Exception("File is required");
            }

            // Upload file to Cloudinary
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, file.InputStream),
                PublicId = $"BeneficiaryDocuments/{beneficiaryName}/{Path.GetFileNameWithoutExtension(file.FileName)}", // Optional: You can set folder structure and file name
                UseFilename = false,
                UniqueFilename = false,
                Overwrite = true // Overwrite if a file with the same name exists
            };

            var uploadResult = _cloudinary.Upload(uploadParams);

            // Return the URL of the uploaded file
            return uploadResult.SecureUrl.ToString();
        }
    }
}