using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Services.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CodeForProjects.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        string path;
        IAlgoritemServices services;
        public UploadController(IAlgoritemServices services)
        {
            this.services = services;
        }
        public bool IsAlive()
        {
            return true;
        }

        [HttpPost("single")]
        public string  UploadSingleFileAsync(IFormFile formFile)//getting file with client and returm a path to
                                                                           //the music file that creat
        {          
            
            long size = formFile == null ? 0 : formFile.Length;//the file size

            string path = @"F:\TheProject\Project\picture\";//the place that i save the files

            if (formFile?.Length > 0)////check if file is null...
            {
               
                var filePath = path + formFile.FileName;//Path.GetTempFileName();
                try
                {
                    using (var stream = System.IO.File.Create(filePath))//create the file in the path
                    {
                         formFile.CopyToAsync(stream);
                    }
                }
                catch(Exception x)
                {
                    string a = "a";
                }
               path = services.exec(filePath);//exec the algoritem
               return "{\"path\":\""+path+"\"}";//return the path to the musuc file that created
            }
             return null;         
        }
        [HttpGet("{id:int}")]

        public async Task<ActionResult> DownloadFile(int id)//return to the client, react the music file
        {
            try
            {
               
                 var filePath =  $"F://TheProject//Project//picture//MySound113.midi";//the music path
                //var filePath = "C:\\temp\\";
                var provider = new FileExtensionContentTypeProvider();
                if (!provider.TryGetContentType(filePath, out var contentType))
                {
                    contentType = "application/octet-stream";
                }
                if (System.IO.File.Exists(filePath))
                {
                    var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
                    return File(bytes, contentType, Path.GetFileName(filePath));
                }

            }
            catch (Exception ex)
            {

                Debug.WriteLine("Error!");
            }
            return null;
        }


    }
}
