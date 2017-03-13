using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Drawing;

namespace piview
{
    class ImageFileUtils
    {
        public static ImageSource LoadPPM(out int out_width, out int out_height, FileStream fileStream)
        {
            ImageSource imgSource = null;
            out_width = 0;
            out_height = 0;

            string ppm_type = null;
            string ppm_wh = null;
            string ppm_range = null;

            StreamReader streamReader = new StreamReader(fileStream, Encoding.Default);

            try
            {
                ppm_type = streamReader.ReadLine();
                ppm_wh = streamReader.ReadLine();
                ppm_range = streamReader.ReadLine();
            }
            catch (Exception)
            {
                return null;
            }

            if (ppm_type.Equals("P3"))
            {
                string[] imgsize = ppm_wh.Split(' ');
                if (imgsize.Length != 2)
                {
                    return null;
                }

                int width = Convert.ToInt32(imgsize[0]);
                int height = Convert.ToInt32(imgsize[1]);
                int range = Convert.ToInt32(ppm_range);

                imgSource = LoadPPM_P3_DATA(streamReader, width, height, range);
                out_width = width;
                out_height = height;
            }
            else if (ppm_type.Equals("P6"))
            {
                string[] imgsize = ppm_wh.Split(' ');
                if (imgsize.Length != 2)
                {
                    return null;
                }

                int width = Convert.ToInt32(imgsize[0]);
                int height = Convert.ToInt32(imgsize[1]);
                int range = Convert.ToInt32(ppm_range);

                imgSource = LoadPPM_P6_DATA(streamReader, width, height, range);
                out_width = width;
                out_height = height;
            }

            return imgSource;
        }

        public static ImageSource LoadPPM_P3_DATA(StreamReader strReader, int width, int height, int range)
        {
            ImageSource imgSource = null;

            Bitmap bmp = new Bitmap(width, height);

            float fRange = range;
            float fR, fG, fB;
            string line = null;

            int y = 0;
            int x = 0;
            while (null != (line = strReader.ReadLine()))
            {
                string[] txtClrs = line.Split(' ');

                if (txtClrs.Length % 3 != 0)
                {
                    return null;
                }

                for (int i = 0; i < txtClrs.Length; i += 3)
                {
                    if (x >= width)
                    {
                        ++y;
                        x = 0;
                    }

                    if (x >= width && y >= height)
                        break;

                    fR = Convert.ToInt32(txtClrs[i]);
                    fG = Convert.ToInt32(txtClrs[i + 1]);
                    fB = Convert.ToInt32(txtClrs[i + 2]);

                    fR = (fR / fRange) * 255.0f;
                    fG = (fG / fRange) * 255.0f;
                    fB = (fB / fRange) * 255.0f;

                    bmp.SetPixel(x, y, System.Drawing.Color.FromArgb((int)fR, (int)fG, (int)fB));
                    ++x;
                }
            }

            imgSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                bmp.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

            return imgSource;
        }


        public static ImageSource LoadPPM_P6_DATA(StreamReader strReader, int width, int height, int range)
        {
            ImageSource imgSource = null;
            Bitmap bmp = new Bitmap(width, height);

            float fRange = range;
            float fR, fG, fB;

            int y = 0;
            int x = 0;

            byte[] clrbuf = new byte[3];
            BinaryReader binReader = new BinaryReader(strReader.BaseStream);

            binReader.BaseStream.Seek(0, SeekOrigin.Begin);
            int flCount = 0;
            while (true)
            {
                if (flCount == 3)
                    break;

                try
                {
                    byte lf_lable = binReader.ReadByte();
                    if (lf_lable == '\n')
                        ++flCount;
                }
                catch (Exception)
                {
                    return imgSource;
                }
            }


            while (true)
            {
                try
                {
                    clrbuf[0] = binReader.ReadByte();
                    clrbuf[1] = binReader.ReadByte();
                    clrbuf[2] = binReader.ReadByte();
                }
                catch (Exception)
                {
                    break;
                }

                if (x >= width)
                {
                    ++y;
                    x = 0;
                }

                if (y >= height)
                    break;


                fR = clrbuf[0];
                fG = clrbuf[1];
                fB = clrbuf[2];

                fR = (fR / fRange) * 255.0f;
                fG = (fG / fRange) * 255.0f;
                fB = (fB / fRange) * 255.0f;

                bmp.SetPixel(x, y, System.Drawing.Color.FromArgb((int)fR, (int)fG, (int)fB));

                ++x;
            }

            imgSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                bmp.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

            return imgSource;
        }
    }
}
