using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

#pragma warning disable CA1416
class concatenation
{
    private void CopyImageData(Bitmap src, int x, int y, BitmapData destData)
    {
        BitmapData srcData = src.LockBits(new Rectangle(0, 0, src.Width, src.Height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
        int Width = srcData.Width;
        int Height = srcData.Height;
        int SrcStride = srcData.Stride;
        int DestStride = destData.Stride;

        int destX = x;
        int destY = y;

        unsafe
        {
            byte* srcPtr = (byte*)srcData.Scan0;
            byte* destPtr = (byte*)destData.Scan0;

            for (int i = 0; i < Height; i++)
            {
                int DestRow = (destY + i) * DestStride;
                int SrcRow = i * SrcStride;
                for (int j = 0; j < Width; j++)
                {
                    byte pixel = (byte)(srcPtr[j + SrcRow]);
                    destPtr[DestRow + destX + j] = pixel;
                }
            }
        }
        src.UnlockBits(srcData);
    }
    public Bitmap concatenate(Bitmap image1, Bitmap image2, bool vertical = false)
    {

        if (image1.PixelFormat != PixelFormat.Format8bppIndexed || image2.PixelFormat != PixelFormat.Format8bppIndexed)
        {
            throw new ArgumentException("the images should be (8bpp) format");
        }

        int Width = image1.Width + image2.Width;
        int Height = Math.Max(image1.Height, image2.Height);

        if (vertical)
        {
            Width = Math.Max(image1.Width, image2.Width);
            Height = image1.Height + image2.Height;
        }


        Bitmap result = new Bitmap(Width, Height, PixelFormat.Format8bppIndexed);

        ColorPalette platte = result.Palette;
        for (int i = 0; i < 256; i++)
        {
            platte.Entries[i] = Color.FromArgb(i, i, i);
        }
        result.Palette = platte;

        BitmapData resultData = result.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

        if (vertical)
        {
            CopyImageData(image1, (Width - image1.Width) / 2, 0, resultData);
            CopyImageData(image2, (Width - image2.Width) / 2, image1.Height, resultData);
            return result;
        }
        else
        {
            CopyImageData(image1, 0, (Height - image1.Height) / 2, resultData);
            CopyImageData(image2, image1.Width, (Height - image2.Height) / 2, resultData);

            return result;
        }
    }
    public void Run()
    {

        Console.Write("enter the  path  for the first image : ");
        string path1 = Console.ReadLine();
        Console.Write("enter the  path  for the second image : ");
        string path2 = Console.ReadLine();
        
        Bitmap image1 = new Bitmap(path1);
        Bitmap image2 = new Bitmap(path2);

        string dir = @"results";
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);



        var watch = System.Diagnostics.Stopwatch.StartNew();


        concatenate(image1, image2, false).Save("results/horizontal_result.bmp");
        concatenate(image1, image2, true).Save("results/vertical_result.bmp");


        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;

        Console.WriteLine("Time taken: {0}ms", elapsedMs);
    }
    public static void Main(String[] args)
    {
        concatenation b = new concatenation();
        b.Run();
    }
}
