using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace ImageProcessing
{
    // Definition of colors
    // from http://stackoverflow.com/questions/1176910/finding-specific-pixel-colors-of-a-bitmapimage
    // Using this construction we can acces pixel.Blue etc.

    // Should we also introduce HSV in bytes in this construction + conversion??
    // Should we introduce some low level operations using bitmasks and shift >> operators
    [StructLayout(LayoutKind.Explicit)]
    public struct PixelColor
    {
        // 32 bit BGRA 
        [FieldOffset(0)]
        public UInt32 ColorBGRA;
        // 8 bit components
        [FieldOffset(0)]
        public byte Blue;
        [FieldOffset(1)]
        public byte Green;
        [FieldOffset(2)]
        public byte Red;
        [FieldOffset(3)]
        public byte Alpha;
    }

    // We use float (was more lightweight) instead of double  
    public struct PixelHsv
    {
        public float H;  // 0..360
        public float S;  // 0..1
        public float V;  // 0..1
    }

    // Many variants HSV conversion, this one used, test few pixels with online translator 
    // http://www.codeproject.com/Articles/19045/Manipulating-colors-in-NET-Part-1
    // Speed seems ok in relation to reading images
    public static class Conversions
    {
        // refinement: input RGB value, conversion to r,g,b needed?? 
        public static PixelHsv RGBtoHSB(int red, int green, int blue)
        {
            // normalize red, green and blue values
            float r = ((float)red / 255.0F);
            float g = ((float)green / 255.0F);
            float b = ((float)blue / 255.0F);

            // conversion start
            float max = Math.Max(r, Math.Max(g, b));
            float min = Math.Min(r, Math.Min(g, b));

            float h = 0.0F;
            if (max == min)
            {
                // undefined, without this test no error becomes NAN
            }
            else if (max == r && g >= b)
            {
                h = 60 * (g - b) / (max - min);
            }
            else if (max == r && g < b)
            {
                h = 60 * (g - b) / (max - min) + 360;
            }
            else if (max == g)
            {
                h = 60 * (b - r) / (max - min) + 120;
            }
            else if (max == b)
            {
                h = 60 * (r - g) / (max - min) + 240;
            }

            float s = (float)((max == 0) ? 0.0 : (1.0 - (min / max)));

            PixelHsv result = new PixelHsv();
            result.H = h;     // 0..360
            result.S = s;     // 0..1
            result.V = max;   // 0..1

            return result;
        }
    }
}

