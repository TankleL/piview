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
            StreamReader streamReader = new StreamReader(fileStream, Encoding.Default);
            fileStream.Seek(0, SeekOrigin.Begin);

            ImageSource imgSource = LoadPPM(streamReader);

            imgShow.Source = imgSource;

            m_showable = true;
        }

        public ImageSource LoadPPM(StreamReader strReader)
        {
            ImageSource imgSource = null;

            string ppm_type = null;
            string ppm_wh = null;
            string ppm_range = null;

            try
            {
                ppm_type = strReader.ReadLine();
                ppm_wh = strReader.ReadLine();
                ppm_range = strReader.ReadLine();
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

                imgSource = LoadPPM_P3_DATA(strReader, width, height, range);
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
    }
}
