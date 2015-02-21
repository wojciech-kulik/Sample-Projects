using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.IO;

namespace ClientsManager.FilesAccess
{
    public sealed class ImageConverter
    {
        public static byte[] ConvertToBytes(BitmapImage bitmapImage)
        {
            if (bitmapImage == null) return null;
           
            WriteableBitmap image = new WriteableBitmap(bitmapImage);
            using (MemoryStream stream = new MemoryStream())
            {
                image.SaveJpeg(stream, image.PixelWidth, image.PixelHeight, 0, 100);
                return stream.ToArray();
            }
        }

        public static BitmapImage ConvertToImage(byte[] byteArray)
        {
            if (byteArray == null) return null;

            BitmapImage bitmapImage = new BitmapImage();
            using (MemoryStream stream = new MemoryStream(byteArray))
            {
                bitmapImage.SetSource(stream);
            }

            return bitmapImage;
        }
    }
}
