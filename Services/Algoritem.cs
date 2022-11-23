using System;

namespace Algoritem
{
    public class Algoritem
    {
        public string exec(string path)
        {
            ImageToMat i = new ImageToMat();
            return i.manage(path);
        }
    }
}
