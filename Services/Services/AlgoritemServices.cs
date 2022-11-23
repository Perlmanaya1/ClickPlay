using Algoritem;
using Services.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Services
{
   public class AlgoritemServices:IAlgoritemServices
    {

        public string exec(string path)
        {
            ImageToMat i = new ImageToMat();
            return i.manage(path);
        }
    }
}
