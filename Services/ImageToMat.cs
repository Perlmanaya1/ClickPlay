using System;
using System.Collections.Generic;
using System.Drawing;

namespace Algoritem
{
    class ImageToMat
    {

    

        public ImageToMat()
        {

        }
        public string manage(string path)
        {

            Color[,] nisoi = GetColorMatrix(path);       
            int[,] nisoi2 = GetColorMatToInt(nisoi);
            ImageProccecing img = new ImageProccecing();
            string s= img.manage(nisoi2);
            return s;
        }
        

        public Color[,] GetColorMatrix(string bitmapFilePath)//Gets an image and turns it into a color matrix

        {                
            Bitmap b1 = new Bitmap(bitmapFilePath);//bitmap to the picture
            
            int hight = b1.Height;//the bitmap Height
            int width = b1.Width;//the bitmap Width
            Color[,] colorMatrix = new Color[hight, width];//a new color mat, with the bitmap size
            for (int i = 0; i < hight; i++)//Entering the appropriate color values for the matrix
            {
                for (int j = 0; j < width; j++)
                {
                    colorMatrix[i, j] = b1.GetPixel(j, i);
                }
            }
            return colorMatrix;
        }


        public Color[,] GetSmallColorMatrix(Color[,] MatColor)
        {
            int r = 0, g = 0, b = 0;
            Color color = new Color();
            color = Color.FromArgb(255, 255, 255);

            int inew = 0, jnew = 0;
            int row = MatColor.GetLength(0);
            int colum = MatColor.GetLength(1);
            while (row % 2 != 0)
            {
                row--;
            }
            while (colum % 2 != 0)
            {
                colum--;
            }
            Color[,] NewMatColor = new Color[row / 2, colum / 2];
            for (int i = 0; i < row; i += 2)
            {
                for (int j = 0; j < colum; j += 2)
                {
                    for (int l = 0; l < 2; l++)
                    {
                        for (int m = 0; m < 2; m++)
                        {
                            r += MatColor[i + l, j + m].R;
                            g += MatColor[i + l, j + m].G;
                            b += MatColor[i + l, j + m].B;
                        }



                    }
                    r = r /4 ;
                    g = g / 4;
                    b = b / 4;
                    color = Color.FromArgb(r, g, b);
                    r = 0;
                    g = 0;
                    b = 0;
                    NewMatColor[inew, jnew] = color;
                    if (jnew + 1 < colum / 2)
                    {
                        jnew++;
                    }
                    else
                    {
                        if ((jnew + 1) == colum /2)
                            jnew = 0;
                        inew++;
                    }
                }
            }
            return NewMatColor;
        }//פןנקציה שמקבלת מטריצת צבעים ומצמצמת אותה חלקי תשע

        public int[,] GetColorMatToInt(Color[,] colorMat)// A function that accepts a color matrix and turns it into zeros and ones
        {
            int[,] imageMatrix = new int[colorMat.GetLength(0), colorMat.GetLength(1)];//new int mat in size the color mat
            for (int i = 0; i < imageMatrix.GetLength(0); i++)//Enter the appropriate values, by color
            {
                for (int j = 0; j < imageMatrix.GetLength(1); j++)
                {
                    if (colorMat[i, j].R > 220)
                    {
                        imageMatrix[i, j] = 0;
                    }
                    else
                    {
                        imageMatrix[i, j] = 1;
                    }
                }
            }
            return imageMatrix;
        }
        public Bitmap MedianFilter(Bitmap Image, int Size)
        {
            
            Bitmap TempBitmap = Image;
            Bitmap NewBitmap = new Bitmap(TempBitmap.Width, TempBitmap.Height);
            Graphics NewGraphics = Graphics.FromImage(NewBitmap);
            NewGraphics.DrawImage(TempBitmap, new System.Drawing.Rectangle(0, 0, TempBitmap.Width, TempBitmap.Height), new System.Drawing.Rectangle(0, 0, TempBitmap.Width, TempBitmap.Height), System.Drawing.GraphicsUnit.Pixel);
            NewGraphics.Dispose();
            Random TempRandom = new Random();
            int ApetureMin = -(Size / 2);
            int ApetureMax = (Size / 2);
            for (int x = 0; x < NewBitmap.Width; ++x)
            {
                for (int y = 0; y < NewBitmap.Height; ++y)
                {
                    
                    List<int> RValues = new List<int>();
                    List<int> GValues = new List<int>();
                    List<int> BValues = new List<int>();
                    for (int x2 = ApetureMin; x2 < ApetureMax; ++x2)
                    {
                        int TempX = x + x2;
                        if (TempX >= 0 && TempX < NewBitmap.Width)
                        {
                            for (int y2 = ApetureMin; y2 < ApetureMax; ++y2)
                            {
                                int TempY = y + y2;
                                if (TempY >= 0 && TempY < NewBitmap.Height)
                                {
                                    Color TempColor = TempBitmap.GetPixel(TempX, TempY);
                                    RValues.Add(TempColor.R);
                                    GValues.Add(TempColor.G);
                                    BValues.Add(TempColor.B);
                                }
                            }
                        }
                    }
                    RValues.Sort();
                    GValues.Sort(); BValues.Sort();
                    Color MedianPixel = Color.FromArgb(RValues[RValues.Count / 2],
                        GValues[GValues.Count / 2],
                          BValues[BValues.Count / 2]);
                    NewBitmap.SetPixel(x, y, MedianPixel);
                }
            }
            return NewBitmap;
        }//פונקציה שמסננת רעשים לא ככ שימושית
    }
}



