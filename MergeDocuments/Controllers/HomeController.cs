using MergeDocuments.Models;
using MergeHelper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MergeDocuments.Controllers
{
    public class HomeController : Controller
    {
        [Obsolete]
        private IHostingEnvironment Environment;
        private readonly IHelper _helper;

        [Obsolete]
        public HomeController(IHostingEnvironment _environment, IHelper helper)
        {
            Environment = _environment;
            _helper = helper;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult MergerIntoOne()
        {
            return View();
        }
        [HttpPost]
        [Obsolete]
        public IActionResult MergerIntoOne(List<IFormFile> postedFiles)
        {
            string path = Path.Combine(Environment.WebRootPath, "Uploads");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            List<string> uploadedFiles = new List<string>();
            CopyToServer(postedFiles, path, uploadedFiles);
            _helper.MergeDocument(Path.Combine(path, $"MergeDocumentWebFile{DateTime.Now.Ticks}.docx"), uploadedFiles);
            DeleteFromServer(uploadedFiles);
            return View();
        }

        private static void DeleteFromServer(List<string> uploadedFiles)
        {
            foreach (string postedFile in uploadedFiles)
            {
                System.IO.File.Delete(postedFile);
            }
        }

        private static void CopyToServer(List<IFormFile> postedFiles, string path, List<string> uploadedFiles)
        {
            foreach (IFormFile postedFile in postedFiles)
            {
                string fileName = Path.GetFileName(postedFile.FileName);
                using FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create);
                postedFile.CopyTo(stream);
                uploadedFiles.Add(Path.Combine(path, fileName));
            }
        }
    }
}
