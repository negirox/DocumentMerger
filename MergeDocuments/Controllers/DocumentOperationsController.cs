using MergeHelper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MergeDocuments.Controllers
{
    public class DocumentOperationsController : Controller
    {
        [Obsolete]
        private IHostingEnvironment Environment;
        private readonly IHelper _helper;

        [Obsolete]
        public DocumentOperationsController(IHostingEnvironment _environment, IHelper helper)
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
        public IActionResult MergeDocuments(List<IFormFile> postedFiles)
        {
            if (postedFiles.Any())
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
            }
            return RedirectToAction(nameof(MergerIntoOne));
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
        public IActionResult MergeDocuments()
        {
            return View();
        }


    }
}
