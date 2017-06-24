using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows;

// Some notes on WPF images: 
// One abstract general class, several specialised subclasses that specialize for use in XAML, Encoding etc
// ImageSource   Immutable
// BitMapSource  Immutable, more focussed on Bitmap instead of display??? 

// RenderTargetBitmap   Renders visuals to bitmap, can be changed
// WritableBitmapSource Can be changed once, next immutable

// General idea:
// 1) Decode from file, BitmapSource
// 2) Use FormatConvertedBitmap to convert to Bgra32, Pgra32 or so pixelformat, destinationformat  
// 3) Copy byte array FROM image (or part of image) 
// 4) Use WritebleBitmap to create new image from byte array

// In general (Delphi(scanlines), Lazarus, C# Winforms, GHDU etc. always fundemental questions
// for image processing and algoritms: build/convert to your own standard representation(bytes OR reals) 
// OR stay close to current system
// You are free to adapt the copy actions to your own float R,G,B representation instead of bytes.

namespace ImageProcessing
{
    public static class Image2PixelArray
    {
        // NOTE: IMAGE CONVENTION PixelColor[iy,ix] with origin TOP LEFT, iy downwards, ix to the right
        // We can choose any array defined by the copy action in CopyPixelsTopLeft2 
        // However to transform back to a WritebleBitmap we use the PixelsTopLeft[iy,ix] convention 

        // Construct a PixelColorTopLeft[iy,ix] array for 2D processing the RGB Pixels
        //public static PixelColor[,] GetPixelsTopLeftFromFilename(string _name, int DecodeHW = 0)
        //{
        //    // First given filename to Bgra
        //    // refinement: Add bool UseBgra to class MyBitmap, move this part to that class 
        //    BitmapImage image1 = new BitmapImage();
        //    image1.BeginInit();
        //    image1.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;  // (BitmapCreateOptions.DelayCreation |
        //    image1.CacheOption = BitmapCacheOption.OnLoad;
        //    image1.UriSource = new Uri(_name);

        //    // For histograms: make squared, smaller images
        //    if (DecodeHW != 0)
        //    {
        //        image1.DecodePixelWidth = DecodeHW;
        //        image1.DecodePixelHeight = DecodeHW;
        //    }
        //    image1.EndInit();

        //    // Use FormatConvertedBitmap for the encoding
        //    // .. we could do this in GetPixels           
        //    BitmapSource imageBgra = new FormatConvertedBitmap(image1, PixelFormats.Bgra32, null, 0);

        //    return GetPixelsTopLeft(imageBgra);
        //}

        // from http://stackoverflow.com/questions/1176910/finding-specific-pixel-colors-of-a-bitmapimage


        //usage
        // var pixels = GetPixels(image);
        // if(pixels[7, 3].Red > 4)
        //public static PixelColor[,] GetPixelsTopLeft(BitmapSource source)
        //{
        //    // note: we have done this already.
        //    // I suppose that if you can read the image, you can convert it to Bgra32
        //    if (source.Format != PixelFormats.Bgra32)
        //        source = new FormatConvertedBitmap(source, PixelFormats.Bgra32, null, 0);

        //    int width = source.PixelWidth;
        //    int height = source.PixelHeight;
        //    PixelColor[,] result = new PixelColor[height, width];

        //    //source.CopyPixels1(result, width * 4, 0);
        //    source.CopyPixelsTopLeft2(result);
        //    return result;
        //}

        //private static void CopyPixelsTopLeft2(this BitmapSource source, PixelColor[,] pixels)
        //{
        //    var height = source.PixelHeight;
        //    var width = source.PixelWidth;

        //    int sizeInBytes = source.PixelWidth * ((source.Format.BitsPerPixel + 7) / 8);

        //    // Step 1. Dump to pixels to 1D array (CopyPixels works also on rectangles) ...
        //    var pixelsUint = new UInt32[height * width];
        //    source.CopyPixels(pixelsUint, sizeInBytes, 0);

        //    // Step 2. To 2D array if we want 2D indexing
        //    // Our convention in the copy action is here PixelsTopLeft
        //    // these steps minor processing time in comparision of reading image
        //    for (int y = 0; y < height; y++)
        //    {
        //        for (int x = 0; x < width; x++)
        //        {
        //            pixels[y, x] = new PixelColor { ColorBGRA = pixelsUint[y * width + x] };
        //        }
        //    }
        //}

        public static int GetH(PixelColor[,] PixelsTopLeft) { return PixelsTopLeft.GetLength(0); }
        public static int GetW(PixelColor[,] PixelsTopLeft) { return PixelsTopLeft.GetLength(1); }

        public static Texture2D Texture2DFromPixelsTopLeft(PixelColor[,] pixels)
        {
            int ImgH = pixels.GetLength(0);
            int ImgW = pixels.GetLength(1);

            var texture2D = new Texture2D(ImgW, ImgH);

            var c = new Color32[pixels.Length];
            for(var i = 0; i < ImgH; i++)
            { 
                for(var j = 0; j < ImgW; j++)
                {
                    var p = pixels[i, j];
                    var col = new Color32(p.Red, p.Green, p.Blue, p.Alpha);
                    c[i * ImgW + j] = col;
                }
            }

            texture2D.SetPixels32(c);
            // DPI must be same as original image, otherwise rescaling display can occur (NoScaling)
            //var wBitmap = new WriteableBitmap(ImgW, ImgH, DpiX, DpiY, System.Windows.Media.PixelFormats.Bgra32, null);

            // 1) copy PixelsOut to bitmap
            // qq to do: ImgW or ImgW-1 in rect??; stride=0 seems to work ...
            //Int32Rect sourceRect = new Int32Rect(0, 0, ImgW, ImgH);

            //wBitmap.WritePixels(sourceRect, PixelsTopLeft, ImgW * 4, 0);

            // 2) set Image1.Source=wBitmap; e.g. handle change source in Bitmap.
            return texture2D;
        }

        public static PixelColor[,] GetPixelsTopLeft(Texture2D sourceImage)
        {
            int width = sourceImage.width;
            int height = sourceImage.height;
            PixelColor[,] result = new PixelColor[height, width];

            Color32[] pixels = sourceImage.GetPixels32();

            for (var i = 0; i < height; i++)
            {
                for (var j = 0; j < width; j++)
                {
                    var p = pixels[i*width + j];
                    //var col = new Color32(p.Red, p.Green, p.Blue, p.Alpha);
                    var pixelColor = new PixelColor()
                    {
                        Blue = p.b,
                        Green = p.g,
                        Red = p.r,
                        Alpha = p.a
                    };

                    result[i, j] = pixelColor;
                }
            }

            return result;
        }

        // Return a bitmapsource using a writable bitmap
        //    public static WriteableBitmap BitmapSourceFromPixelsTopLeft
        //                    (PixelColor[,] PixelsTopLeft, double DpiX = 96.0, double DpiY = 96.0)
        //    {
        //        // Note: normally WritebleBitmap is used with lock, write to BackBuffer and unlock
        //        // Maybe for the way we use it here we could use newly created othr type of bitmap??

        //        int ImgH = PixelsTopLeft.GetLength(0);
        //        int ImgW = PixelsTopLeft.GetLength(1);

        //        // DPI must be same as original image, otherwise rescaling display can occur (NoScaling)
        //        var wBitmap = new WriteableBitmap(ImgW, ImgH, DpiX, DpiY, System.Windows.Media.PixelFormats.Bgra32, null);

        //        // 1) copy PixelsOut to bitmap
        //        // qq to do: ImgW or ImgW-1 in rect??; stride=0 seems to work ...
        //        Int32Rect sourceRect = new Int32Rect(0, 0, ImgW, ImgH);

        //        wBitmap.WritePixels(sourceRect, PixelsTopLeft, ImgW * 4, 0);

        //        // 2) set Image1.Source=wBitmap; e.g. handle change source in Bitmap.
        //        return wBitmap;
        //    }

        //    public static PixelColor[,] GetPixelsTopLeft(Texture2D sourceImage)
        //    {
        //        // note: we have done this already.
        //        //// I suppose that if you can read the image, you can convert it to Bgra32
        //        //if (source.Format != PixelFormats.Bgra32)
        //        //    source = new FormatConvertedBitmap(source, PixelFormats.Bgra32, null, 0);

        //        int width = sourceImage.width;
        //        int height = sourceImage.height;
        //        PixelColor[,] result = new PixelColor[height, width];

        //        //source.CopyPixels1(result, width * 4, 0);
        //        Color32[] pixels = sourceImage.GetPixels32();
        //        Debug.Log(pixels);
        //        //.CopyPixelsTopLeft2(result);
        //        return result;
        //    }
    }
}
