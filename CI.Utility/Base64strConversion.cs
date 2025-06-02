using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CI.Utility
{
    public class Base64strConversion
    {
        private readonly IConfiguration _config;

        public Base64strConversion(IConfiguration config)
        {
            _config = config;
        }

        public string convertToBase64String(string fileName, string fileType, string directory)
        {
            string subFolder = string.Empty;
            if (fileType == "Image")
            {
                subFolder = _config["FileUpload:ImagesFolder"];
            }
            if (fileType == "Doc")
            {
                subFolder = _config["FileUpload:DocsFolder"];
            }
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), 
                _config["FileUpload:MainFolder"], subFolder, directory);
            FileStream fs = new FileStream(Path.Combine(folderPath, fileName), FileMode.Open);
            BinaryReader br = new BinaryReader(fs);
            byte[] bytes = br.ReadBytes((Int32)fs.Length);
            fs.Close();
            return Convert.ToBase64String(bytes, 0, bytes.Length);
        }


        public List<string> convertToBase64(string filesName, string fileType, string directory)
        {
            List<string> result = new List<string>();
            FileStream fs;
            BinaryReader br;
            byte[] bytes;

            if (!string.IsNullOrEmpty(filesName))
            {
                string subFolder = string.Empty;
                if (fileType == "images")
                {
                    subFolder = _config["FileUpload:ImagesFolder"];
                }
                if (fileType == "docs")
                {
                    subFolder = _config["FileUpload:DocsFolder"];
                }
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(),
                    _config["FileUpload:MainFolder"], directory, subFolder);

                if (!filesName.Contains(","))
                {
                    fs = new FileStream(Path.Combine(folderPath, filesName), FileMode.Open);
                    br = new BinaryReader(fs);
                    bytes = br.ReadBytes((Int32)fs.Length);
                    result.Add(Convert.ToBase64String(bytes, 0, bytes.Length));
                    fs.Close();
                }
                else
                {
                    List<string> files = filesName.Split(',').ToList();
                    foreach(string file in files)
                    {
                        fs = new FileStream(Path.Combine(folderPath, file.Trim()), FileMode.Open);
                        br = new BinaryReader(fs);
                        bytes = br.ReadBytes((Int32)fs.Length);
                        result.Add(Convert.ToBase64String(bytes, 0, bytes.Length));
                        fs.Close();
                    }                    
                }

            }
            return result; 
        }
    }
}
