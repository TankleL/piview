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
        private List<string> m_imgList = null;
        private int m_imgSeq = 0;

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

            ImageSource imgSource = ImageFileUtils.LoadPPM(out m_imgWidth, out m_imgHeight, fileStream);
            if(null == imgSource)
            {
                MessageBox.Show("Cannot decode this format's image file.", "Error!");
            }
            fileStream.Close();

            imgShow.Source = imgSource;

            string imgRepoPath = fileName.Substring(0, fileName.LastIndexOf('\\'));
            imgRepoPath += '\\';
            m_imgList = ScanImageFiles(imgRepoPath);
            m_imgSeq = m_imgList.IndexOf(fileName);
                        
            m_showable = true;
            Title = "piview - " + fileName;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            switch(e.Key)
            {
                case Key.Right:
                    if (m_imgList.Count == 0 || m_imgList.Count == 1)
                        return;

                    if (m_imgSeq == m_imgList.Count - 1)
                        m_imgSeq = 0;
                    else
                        ++m_imgSeq;

                    using (FileStream fileStream = new FileStream(m_imgList[m_imgSeq], FileMode.Open, FileAccess.Read))
                    {
                        fileStream.Seek(0, SeekOrigin.Begin);
                        ImageSource imgSource = ImageFileUtils.LoadPPM(out m_imgWidth, out m_imgHeight, fileStream);
                        if (null == imgSource)
                        {
                            MessageBox.Show("Cannot decode this format's image file.", "Error!");
                        }
                        fileStream.Close();

                        imgShow.Source = imgSource;
                        Title = "piview - " + m_imgList[m_imgSeq];
                    }
                    break;

                case Key.Left:
                    if (m_imgList.Count == 0 || m_imgList.Count == 1)
                        return;

                    if (m_imgSeq == 0)
                        m_imgSeq = m_imgList.Count - 1;
                    else
                        --m_imgSeq;

                    using (FileStream fileStream = new FileStream(m_imgList[m_imgSeq], FileMode.Open, FileAccess.Read))
                    {
                        fileStream.Seek(0, SeekOrigin.Begin);
                        ImageSource imgSource = ImageFileUtils.LoadPPM(out m_imgWidth, out m_imgHeight, fileStream);
                        if (null == imgSource)
                        {
                            MessageBox.Show("Cannot decode this format's image file.", "Error!");
                        }
                        fileStream.Close();

                        imgShow.Source = imgSource;
                        Title = "piview - " + m_imgList[m_imgSeq];
                    }

                    break;

                default:
                    break;
            }
        }

        private List<string> ScanImageFiles(string path)
        {
            List<string> result = new List<string>();

            DirectoryInfo dirInfo = new DirectoryInfo(path);
            foreach(FileInfo fileInfo in dirInfo.GetFiles("*.ppm"))
            {
                result.Add(fileInfo.FullName);
            }

            if (result.Count <= 0)
                return null;

            return result;
        }
    }
}
