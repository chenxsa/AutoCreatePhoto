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
    using System.Drawing.Imaging;

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
            image =new Bitmap( Reduce(image, 1024));
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
            //image1 =Reduce(image1, 512);
            //image2 = Reduce(image2, 512);
            int width = image1.Width+image2.Width;
            int height = image2.Height+image2.Height;
            if (width>height)
            {
                height = width;
            }
            else
            {
                width = height;
            }
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
            Graphics graphics = Graphics.FromImage(image);

            graphics.InterpolationMode = InterpolationMode.High;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.Clear(Color.Transparent);
            graphics.DrawImage(originalImage, new Rectangle(0, 0, toWidth, height), new Rectangle(0, 0, originalImage.Width, originalImage.Height), GraphicsUnit.Pixel);

            return image;

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
        /// <summary>
        ///  截取有效图片
        /// </summary> 
        /// <returns>处理后的图</returns>
        public unsafe static Image CutEffectiveImage(Image img)
        {

            if (img == null) return null;
            // 建立GraphicsPath, 给我们的位图路径计算使用   
            GraphicsPath g = new GraphicsPath(FillMode.Alternate);
            Bitmap bitmap = new Bitmap(img);
            int width = bitmap.Width;
            int height = bitmap.Height;
            BitmapData bmData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            byte* p = (byte*)bmData.Scan0;
            int offset = bmData.Stride - width * 3;
            int p0, p1, p2;         // 记录左上角0，0座标的颜色值  
            p0 = p[0];
            p1 = p[1];
            p2 = p[2]; 
            Point yminP = new Point(0, 0);
            Point ymaxP = new Point(0, 0);
            Point xminP = new Point(0, 0);
            Point xmaxP = new Point(0, 0);
            // 行座标 ( Y col )   
            for (int y = 0; y < height; y++)
            {
                // 列座标 ( X row )   
                for (int x = 0; x < width; x++)
                {
                    //如果不是背景色
                    if (!IsWriteOrGren(p[0], p[1], p[2]))
                    {
                        if (y < yminP.Y || yminP.Y==0)
                        {
                            yminP = new Point(x, y);
                        }
                        else if (y > ymaxP.Y)
                        {
                            ymaxP = new Point(x, y);
                        }
                        if (x < xminP.X  || xminP.X==0)
                        {
                            xminP = new Point(x, y);
                        }
                        else if(x > xmaxP.X)
                        {
                            xmaxP = new Point(x, y);
                        }
                    } 
                    p += 3;                                   //下一个内存地址  
                }
                p += offset;
            }
            bitmap.UnlockBits(bmData);

            Image result = CaptureImage(bitmap, xminP.X, yminP.Y, xmaxP.X - xminP.X, ymaxP.Y - yminP.Y); 
             
            return new Bitmap(result);

        }
        static bool IsWriteOrGren(int r,int g,int b) {
            if (r<170 || g<170 || b<170)
            {
                return false;
            }
            int split = Math.Abs(r - g);
            split = Math.Abs(r - b) > split ? Math.Abs(r - b) : split;
            split = Math.Abs(b - g) > split ? Math.Abs(b - g) : split;
            if (split>25)
            {
                return false;
            }
            return true;
        }
        #region 从大图中截取一部分图片
        /// <summary>
        /// 从大图中截取一部分图片
        /// </summary>
        /// <param name="fromImage">来源图片</param>        
        /// <param name="offsetX">从偏移X坐标位置开始截取</param>
        /// <param name="offsetY">从偏移Y坐标位置开始截取</param> 
        /// <param name="width">保存图片的宽度</param>
        /// <param name="height">保存图片的高度</param>
        /// <returns></returns>
        public static Image CaptureImage(Image fromImage, int offsetX, int offsetY,  int width, int height)
        { 
            //创建新图位图
            Bitmap bitmap = new Bitmap(width, height);
            //创建作图区域
            Graphics graphic = Graphics.FromImage(bitmap);
            //截取原图相应区域写入作图区
            graphic.DrawImage(fromImage, 0, 0, new Rectangle(offsetX, offsetY, width, height), GraphicsUnit.Pixel);
            //从作图区生成新图
            return Image.FromHbitmap(bitmap.GetHbitmap()); 
        }
        #endregion
        /// <summary>
        /// 色彩调整
        /// </summary>
        /// <param name="bmp">原始图</param>
        /// <param name="rVal">r增量</param>
        /// <param name="gVal">g增量</param>
        /// <param name="bVal">b增量</param>
        /// <returns>处理后的图</returns>
        public static Bitmap KiColorBalance(Bitmap bmp, int rVal, int gVal, int bVal)
        {

            if (bmp == null)
            {
                return null;
            }


            int h = bmp.Height;
            int w = bmp.Width;

            try
            {
                if (rVal > 255 || rVal < -255 || gVal > 255 || gVal < -255 || bVal > 255 || bVal < -255)
                {
                    return null;
                }

                BitmapData srcData = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

                unsafe
                {
                    byte* p = (byte*)srcData.Scan0.ToPointer();

                    int nOffset = srcData.Stride - w * 3;
                    int r, g, b;

                    for (int y = 0; y < h; y++)
                    {
                        for (int x = 0; x < w; x++)
                        {

                            b = p[0] + bVal;
                            if (bVal >= 0)
                            {
                                if (b > 255) b = 255;
                            }
                            else
                            {
                                if (b < 0) b = 0;
                            }

                            g = p[1] + gVal;
                            if (gVal >= 0)
                            {
                                if (g > 255) g = 255;
                            }
                            else
                            {
                                if (g < 0) g = 0;
                            }

                            r = p[2] + rVal;
                            if (rVal >= 0)
                            {
                                if (r > 255) r = 255;
                            }
                            else
                            {
                                if (r < 0) r = 0;
                            }

                            p[0] = (byte)b;
                            p[1] = (byte)g;
                            p[2] = (byte)r;

                            p += 3;
                        }

                        p += nOffset;


                    }
                } // end of unsafe

                bmp.UnlockBits(srcData);

                return bmp;
            }
            catch
            {
                return null;
            }

        } // end of color
        //使用方法调用GenerateHighThumbnail()方法即可
        //参数oldImagePath表示要被缩放的图片路径
        //参数newImagePath表示缩放后保存的图片路径
        //参数width和height分别是缩放范围宽和高
        public static void GenerateHighThumbnail(string oldImagePath, string newImagePath, int width, int height)
        {
            System.Drawing.Image oldImage = System.Drawing.Image.FromFile(oldImagePath);
            int newWidth = AdjustSize(width, height, oldImage.Width, oldImage.Height).Width;
            int newHeight = AdjustSize(width, height, oldImage.Width, oldImage.Height).Height;
            //。。。。。。。。。。。
            System.Drawing.Image thumbnailImage = oldImage.GetThumbnailImage(newWidth, newHeight, new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback), IntPtr.Zero);
            System.Drawing.Bitmap bm = new System.Drawing.Bitmap(thumbnailImage);
            //处理JPG质量的函数
            System.Drawing.Imaging.ImageCodecInfo ici = GetEncoderInfo("image/jpeg");
            if (ici != null)
            {
                System.Drawing.Imaging.EncoderParameters ep = new System.Drawing.Imaging.EncoderParameters(1);
                ep.Param[0] = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)100);
                bm.Save(newImagePath, ici, ep);
                //释放所有资源，不释放，可能会出错误。
                ep.Dispose();
                ep = null;
            }
            ici = null;
            bm.Dispose();
            bm = null;
            thumbnailImage.Dispose();
            thumbnailImage = null;
            oldImage.Dispose();
            oldImage = null;
        }


        private static bool ThumbnailCallback()
        {
            return false;
        }


        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }


        public struct PicSize
        {
            public int Width;
            public int Height;
        }


        public static PicSize AdjustSize(int spcWidth, int spcHeight, int orgWidth, int orgHeight)
        {
            PicSize size = new PicSize();
            // 原始宽高在指定宽高范围内，不作任何处理 
            if (orgWidth <= spcWidth && orgHeight <= spcHeight)
            {
                size.Width = orgWidth;
                size.Height = orgHeight;
            }
            else
            {
                // 取得比例系数 
                float w = orgWidth / (float)spcWidth;
                float h = orgHeight / (float)spcHeight;
                // 宽度比大于高度比 
                if (w > h)
                {
                    size.Width = spcWidth;
                    size.Height = (int)(w >= 1 ? Math.Round(orgHeight / w) : Math.Round(orgHeight * w));
                }
                // 宽度比小于高度比 
                else if (w < h)
                {
                    size.Height = spcHeight;
                    size.Width = (int)(h >= 1 ? Math.Round(orgWidth / h) : Math.Round(orgWidth * h));
                }
                // 宽度比等于高度比 
                else
                {
                    size.Width = spcWidth;
                    size.Height = spcHeight;
                }
            }
            return size;
        }
    }

}
