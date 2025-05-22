using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GammaAdjustment
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public WriteableBitmap SourceImage  { get; set; }
        public WriteableBitmap AdjustedImage { get; set; }
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[]?)e.Data.GetData(DataFormats.FileDrop);


                SourceImageTxt.Text = files[0];
                ComparisonImageTxt.Text = files[0];

                // 处理拖入的文件
                OuputImage(files[0]);
            }
        }


        //输出图像
        private void OuputImage(string file)
        {
            SourceImage = LoadJpegToWriteableBitmap(file);
            AdjustedImage = LoadJpegToWriteableBitmap(file);

            SourceImageContainer.Source = SourceImage;
            AdjustedImageContainer.Source = AdjustedImage;
        }


        private WriteableBitmap LoadJpegToWriteableBitmap(string filePath)
        {
            // 使用 BitmapImage 加载 JPEG 文件
            BitmapImage bitmapImage = new BitmapImage();
            using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
            }

            // 转换为 WriteableBitmap
            return new WriteableBitmap(bitmapImage);
        }

        private void BrightnessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double gamma = slider_bightness_r.Value;
            ApplyGamma(AdjustedImage, gamma);
        }

        void ApplyGamma(WriteableBitmap bitmap, double gamma)
        {
            bitmap.Lock();
            unsafe
            {
                byte* ptr = (byte*)bitmap.BackBuffer;
                int bytesPerPixel = bitmap.Format.BitsPerPixel / 8;

                for (int y = 0; y < bitmap.PixelHeight; y++)
                {
                    byte* row = ptr + y * bitmap.BackBufferStride;
                    for (int x = 0; x < bitmap.PixelWidth; x++)
                    {
                        int index = x * bytesPerPixel;
                        // Gamma 校正公式
                        row[index + 2] = (byte)MathUtil.Clamp(255 * Math.Pow(row[index + 2] / 255.0, gamma), 0, 255); // R
                        row[index + 1] = (byte)MathUtil.Clamp(255 * Math.Pow(row[index + 1] / 255.0, gamma), 0, 255); // G
                        row[index] = (byte)MathUtil.Clamp(255 * Math.Pow(row[index] / 255.0, gamma), 0, 255);       // B
                    }
                }
            }
            bitmap.AddDirtyRect(new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight));
            bitmap.Unlock();
        }

     

        private void Laboratory0_Click(object sender, RoutedEventArgs e)
        {
            Window win = new Laboratory();
            win.Show();
        }

        private void Laboratory1_Click(object sender, RoutedEventArgs e)
        {
            Window win = new Laboratory1();
            win.Show();
        }
    }
}