using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web.Helpers;

namespace VaWorks.Web.Extensions
{
    public static class WebImageExtension
    {
        private static readonly IDictionary<string, ImageFormat> TransparencyFormats =
           new Dictionary<string, ImageFormat>(StringComparer.OrdinalIgnoreCase) { { "png", ImageFormat.Png }, { "gif", ImageFormat.Gif }, { "jpg", ImageFormat.Jpeg } };

        public static WebImage Resize(this WebImage image, int width)
        {
            double aspectRatio = (double)image.Width / image.Height;
            var height = Convert.ToInt32(width / aspectRatio);

            ImageFormat format;

            if (!TransparencyFormats.TryGetValue(image.ImageFormat.ToLower(), out format)) {
                return image.Resize(width, height);
            }

            using (Image resizedImage = new Bitmap(width, height)) {
                using (var source = new Bitmap(new MemoryStream(image.GetBytes()))) {
                    using (Graphics g = Graphics.FromImage(resizedImage)) {
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.DrawImage(source, 0, 0, width, height);
                    }
                }

                using (var ms = new MemoryStream()) {
                    resizedImage.Save(ms, format);
                    return new WebImage(ms.ToArray());
                }
            }
        }
    }
}