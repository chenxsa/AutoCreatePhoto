using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCreatePhoto
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    public enum CombineMode
    {
        /// <summary>
        /// 上下
        /// </summary>
        /// <remarks></remarks>
        UptoDown = 0,
        /// <summary>
        /// 居中
        /// </summary>
        /// <remarks></remarks>
        LeftToRight = 1,
        /// <summary>
        /// 45°斜角
        /// </summary>
        /// <remarks></remarks>
        Bevel = 2,
        /// <summary>
        /// 135°斜角
        /// </summary>
        /// <remarks></remarks>
        UnBevel = 3
    }
    public static class ImageUtils
    {
        public static Image emptyImage = new Bitmap(0x2d, 0x2d);

        private static double CalDiameter(Image image, double angle, double imageAngle)
        {
            double x = 0.0;
            double num2 = 0.0;
            double num3 = Math.Abs(Tan(angle));
            if (((angle < imageAngle) || ((angle >= (180.0 - imageAngle)) && (angle <= (180.0 + imageAngle)))) || (angle > (360.0 - imageAngle)))
            {
                x = image.Width / 2;
            }
            else
            {
                x = ((double)image.Height) / (2.0 * num3);
            }
            if (((angle <= (180.0 - imageAngle)) && (angle >= imageAngle)) || ((angle >= (180.0 + imageAngle)) && (angle <= (360.0 - imageAngle))))
            {
                num2 = image.Height / 2;
            }
            else
            {
                num2 = (image.Width * num3) / 2.0;
            }
            return Math.Sqrt(Math.Pow(x, 2.0) + Math.Pow(num2, 2.0));
        }

        public static Point CalOffset(Image b, int angle)
        {
            angle = angle % 360;
            double d = (angle * 3.1415926535897931) / 180.0;
            double num2 = Math.Cos(d);
            double num3 = Math.Sin(d);
            int width = b.Width;
            int height = b.Height;
            int num6 = (int)Math.Max(Math.Abs((double)((width * num2) - (height * num3))), Math.Abs((double)((width * num2) + (height * num3))));
            int num7 = (int)Math.Max(Math.Abs((double)((width * num3) - (height * num2))), Math.Abs((double)((width * num3) + (height * num2))));
            return new Point((num6 - width) / 2, (num7 - height) / 2);
        }

        public static Bitmap Compose(Image image1, Image image2, Image bgImage, double angle)
        {
            if (image1 == null)
            {
                throw new ArgumentNullException("image1");
            }
            if (image2 == null)
            {
                throw new ArgumentNullException("image2");
            }
            angle = angle % 360.0;
            double d = (angle * 3.1415926535897931) / 180.0;
            double num2 = Math.Cos(d);
            double num3 = Math.Sin(d);
            double imageAngle = Math.Atan(((double)image1.Height) / ((double)image1.Width));
            imageAngle = (180.0 * imageAngle) / 3.1415926535897931;
            double num5 = Math.Atan(((double)image2.Height) / ((double)image2.Width));
            num5 = (180.0 * num5) / 3.1415926535897931;
            double num6 = CalDiameter(image1, angle, imageAngle);
            double num7 = CalDiameter(image2, angle, num5);
            double num8 = CalDiameter(emptyImage, angle, 45.0);
            double num9 = (num6 + num7) + (num8 * 2.0);
            int width = (((int)(200.0 + Math.Abs((double)(num9 * num2)))) + (image1.Width / 2)) + (image2.Width / 2);
            width = (width < image1.Width) ? (image1.Width + 200) : width;
            width = (width < image2.Width) ? (image2.Width + 200) : width;
            int height = (((int)(200.0 + Math.Abs((double)(num9 * num3)))) + (image2.Height / 2)) + (image1.Height / 2);
            height = (height < image1.Height) ? (image1.Height + 200) : height;
            height = (height < image2.Height) ? (image2.Height + 200) : height;
            if (width > height)
            {
                height = width;
            }
            else
            {
                width = height;
            }
            Bitmap image = new Bitmap(width, height);
            Graphics graphics = Graphics.FromImage(image);
            if (bgImage != null)
            {
                graphics.DrawImage(bgImage, 0, 0, image.Width, image.Height);
            }
            else
            {
                graphics.Clear(Color.White);
            }
            int x = 0;
            if (((angle >= 0.0) && (angle <= 90.0)) || ((angle >= 270.0) && (angle <= 360.0)))
            {
                x = 100;
            }
            else
            {
                x = (width - 100) - image1.Width;
            }
            int y = 0;
            if ((angle >= 0.0) && (angle <= 180.0))
            {
                y = (height - 100) - image1.Height;
            }
            else
            {
                y = 100;
            }
            Point point = new Point(x, y);
            graphics.DrawImage(image1, point);
            x = 0;
            if (((angle >= 0.0) && (angle <= 90.0)) || ((angle >= 270.0) && (angle <= 360.0)))
            {
                x = (width - 100) - image2.Width;
            }
            else
            {
                x = 100;
            }
            y = 0;
            if ((angle >= 0.0) && (angle <= 180.0))
            {
                y = 100;
            }
            else
            {
                y = (height - 100) - image2.Height;
            }
            Point point2 = new Point(x, y);
            graphics.DrawImage(image2, point2);
            graphics.ResetTransform();
            graphics.Save();
            graphics.Dispose();
            return image;
        }
        public static Bitmap NewCompose(Image image1, Image image2, Image bgImage,CombineMode combineMode)
        {
            if (image1 == null)
            {
                throw new ArgumentNullException("image1");
            }
            if (image2 == null)
            {
                throw new ArgumentNullException("image2");
            }
            int width = 1024;
            int height = 1024;
            Bitmap image = new Bitmap(width, height);
            Graphics graphics = Graphics.FromImage(image);
            //画底图 
            if (bgImage != null)
            {
                using (TextureBrush Txbrus = new TextureBrush(bgImage))
                {
                    Txbrus.WrapMode = System.Drawing.Drawing2D.WrapMode.Tile;
                    graphics.FillRectangle(Txbrus, 0, 0, image.Width - 1, image.Height - 1); 
                } 
            }
            else
            {
                graphics.Clear(Color.White);
            }
            int x=0, y = 0;
            Point point = new Point(x, y);
            //上下结构
            //取中间
            switch (combineMode)
            {
                case CombineMode.UptoDown:
                     x = image.Width / 2 -image1.Width / 2;
                     y = image.Height / 2 - image1.Height ;
                     point = new Point(x, y);
                    graphics.DrawImage(image1, point);
                    x = image.Width / 2 - image2.Width / 2;
                    y = image.Height / 2 ;
                    point = new Point(x, y);
                    graphics.DrawImage(image2, point);
                    graphics.ResetTransform();
                    graphics.Save();
                    graphics.Dispose();
                    break;
                case CombineMode.LeftToRight:
                    y = image.Height / 2 - image1.Height / 2; ;
                    x = image.Width / 2 - image1.Width;
                     point = new Point(x, y);
                    graphics.DrawImage(image1, point);
                    y = image.Height / 2 - image2.Height / 2; ;
                    x = image.Width / 2 ;
                    point = new Point(x, y);
                    graphics.DrawImage(image2, point);
                    graphics.ResetTransform();
                    graphics.Save();
                    graphics.Dispose(); 
                    break;
                case CombineMode.Bevel:
                    x = image.Width / 2 - image1.Width / 2-100;
                    y = image.Height / 2 - image1.Height;
                    point = new Point(x, y);
                    graphics.DrawImage(image1, point);
                    x = image.Width / 2 - image2.Width / 2+100;
                    y = image.Height / 2;
                    point = new Point(x, y);
                    graphics.DrawImage(image2, point);
                    graphics.ResetTransform();
                    graphics.Save();
                    graphics.Dispose();
                    break;
                case CombineMode.UnBevel:
                    y = image.Height / 2 - image1.Height / 2-100 ;
                    x = image.Width / 2 - image1.Width;
                    point = new Point(x, y);
                    graphics.DrawImage(image1, point);
                    y = image.Height / 2 - image2.Height / 2+100;
                    x = image.Width / 2;
                    point = new Point(x, y);
                    graphics.DrawImage(image2, point);
                    graphics.ResetTransform();
                    graphics.Save();
                    graphics.Dispose();
                    break;
                default:
                    break;
            }
           
            return image;
        }
        public static Image Reduce(Image originalImage, int toWidth)
        {
            int height = (int)((((double)toWidth) / ((double)originalImage.Width)) * originalImage.Height);
            Image image = new Bitmap(toWidth, height);
            using (Graphics graphics = Graphics.FromImage(image))
            {
                graphics.InterpolationMode = InterpolationMode.High;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.Clear(Color.Transparent);
                graphics.DrawImage(originalImage, new Rectangle(0, 0, toWidth, height), new Rectangle(0, 0, originalImage.Width, originalImage.Height), GraphicsUnit.Pixel);
                originalImage.Dispose();
                return image;
            }
        }

        public static Image Rotate(Image b, int angle)
        {
            angle = angle % 360;
            double d = (angle * 3.1415926535897931) / 180.0;
            double num2 = Math.Cos(d);
            double num3 = Math.Sin(d);
            int width = b.Width;
            int height = b.Height;
            int num6 = (int)Math.Max(Math.Abs((double)((width * num2) - (height * num3))), Math.Abs((double)((width * num2) + (height * num3))));
            int num7 = (int)Math.Max(Math.Abs((double)((width * num3) - (height * num2))), Math.Abs((double)((width * num3) + (height * num2))));
            Bitmap image = new Bitmap(num6, num7);
            Graphics graphics = Graphics.FromImage(image);
            graphics.InterpolationMode = InterpolationMode.Bilinear;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            Point point = new Point((num6 - width) / 2, (num7 - height) / 2);
            Rectangle rect = new Rectangle(point.X, point.Y, width, height);
            Point point2 = new Point(rect.X + (rect.Width / 2), rect.Y + (rect.Height / 2));
            graphics.TranslateTransform((float)point2.X, (float)point2.Y);
            graphics.RotateTransform((float)(360 - angle));
            graphics.TranslateTransform((float)-point2.X, (float)-point2.Y);
            graphics.DrawImage(b, rect);
            graphics.ResetTransform();
            graphics.Save();
            graphics.Dispose();
            return image;
        }

        private static double Tan(double angle)
        {
            double num = angle % 360.0;
            double a = (num * 3.1415926535897931) / 180.0;
            return Math.Tan(a);
        }
    }

}
