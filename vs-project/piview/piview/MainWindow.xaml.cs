using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Drawing;

namespace piview
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool m_showable = false;
        private int m_imgWidth = 0;
        private int m_imgHeight = 0;
        private string m_curdir = String.Empty;

        public bool IsShowable()
        {
            return m_showable;
        }
        
        public MainWindow(string fileName)
        {
            InitializeComponent();

            //  fileName ="E:\\Repos\\piview\\vs-project\\testimg\\test-ppm1.ppm";      // test

            if(!File.Exists(fileName))
            {
                return;
            }

            FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            fileStream.Seek(0, SeekOrigin.Begin);

            ImageSource imgSource = LoadPPM(fileStream);
            if(null == imgSource)
            {
                MessageBox.Show("Cannot decode this format's image file.", "Error!");
            }

            imgShow.Source = imgSource;

            m_showable = true;
        }

        public ImageSource LoadPPM(FileStream fileStream)
        {
            ImageSource imgSource = null;

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
            catch(Exception)
            {
                return null;
            }

            if (ppm_type.Equals("P3"))
            {
                string[] imgsize = ppm_wh.Split(' ');
                if(imgsize.Length != 2)
                {
                    return null;
                }

                int width = Convert.ToInt32(imgsize[0]);
                int height = Convert.ToInt32(imgsize[1]);
                int range = Convert.ToInt32(ppm_range);

                imgSource = LoadPPM_P3_DATA(streamReader, width, height, range);
                m_imgWidth = width;
                m_imgHeight = height;
            }
            else if(ppm_type.Equals("P6"))
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
                m_imgWidth = width;
                m_imgHeight = height;
            }

            return imgSource;
        }

        public ImageSource LoadPPM_P3_DATA(StreamReader strReader, int width, int height, int range)
        {
            ImageSource imgSource = null;

            Bitmap bmp = new Bitmap(width, height);

            float fRange = range;
            float fR, fG, fB;
            string line = null;

            int y = 0;
            int x = 0;
            while(null != (line = strReader.ReadLine()))
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
                bmp.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            return imgSource;
        }


        public ImageSource LoadPPM_P6_DATA(StreamReader strReader, int width, int height, int range)
        {
            ImageSource imgSource = null;
            Bitmap bmp = new Bitmap(width, height);

            float fRange = range;
            float fR, fG, fB;

            int y = 0;
            int x = 0;

            byte[] clrbuf = new byte[3];
            BinaryReader binReader = new BinaryReader(strReader.BaseStream);

            while (true)
            {
                byte lf_lable = binReader.ReadByte();
                if (lf_lable != '\n')
                    binReader.BaseStream.Seek(-2, SeekOrigin.Current);
                else
                    break;
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
                bmp.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            return imgSource;
        }
    }
}
