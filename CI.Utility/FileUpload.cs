using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CI.Utility
{
    public class FileUpload
    {
        private readonly IConfiguration _config;

        public FileUpload(IConfiguration config)
        {
            _config = config;
        }

        public string uploadFile(List<IFormFile> files, string directory)
        {
            string dbfileName = string.Empty;
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    string fileType = ContentDispositionHeaderValue.Parse(file.ContentDisposition).Name.Trim('"');
                    string subFolder = string.Empty;
                    if (fileType == "images")
                    {
                        string extension = Path.GetExtension(file.FileName);
                        string contentType = file.ContentType;
                        string[] imageExtensions = { ".png", ".jpg", ".jpeg" };
                        string[] imageContenttype = { "image/png", "image/jpeg" };
                        if (!imageExtensions.Contains(extension.ToLower()) || !imageContenttype.Contains(contentType.ToLower()))
                        {
                            string validationmsg = "1";
                            return validationmsg;
                        }
                        else
                        {
                            subFolder = _config["FileUpload:ImagesFolder"];
                        }
                    }
                    if (fileType == "docs")
                    {
                        string extension = Path.GetExtension(file.FileName);
                        string contentType = file.ContentType;
                        string[] docExtensions = { ".pdf", ".docx", ".doc", ".xls", ".xlsx" };
                        string[] docContenttype = { "application/pdf", "application/vnd.ms-excel", 
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "application/msword",
                            "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "application/octet-stream"};
                        if (!docExtensions.Contains(extension.ToLower()) || !docContenttype.Contains(contentType.ToLower()))
                        {
                            string validationmsg = "1";
                            return validationmsg;
                        }
                        else
                        {
                            subFolder = _config["FileUpload:DocsFolder"];
                        }
                    }
                    string folderPath = Path.Combine(_config["FileUpload:MainFolder"], directory,
                        subFolder);
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                    string pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderPath);
                    string originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    string fileName = directory + "_" + DateTime.Now.ToString("ddMMyyhhmmss") + "_" + originalFileName;
                    dbfileName += fileName + ", ";
                    using (var stream = new FileStream(Path.Combine(pathToSave, fileName), FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }
            }
            if (!string.IsNullOrEmpty(dbfileName))
            {
                dbfileName = dbfileName.Substring(0, dbfileName.Length - 2);
            }
            return dbfileName;
            //return Path.Combine(folderPath, fileName);
        }
    }
}
