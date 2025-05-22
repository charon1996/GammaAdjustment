using System;
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
using System.Windows.Shapes;

namespace GammaAdjustment
{
    /// <summary>
    /// Laboratory1.xaml 的交互逻辑
    /// </summary>
    public partial class Laboratory1 : Window
    {
        public Laboratory1()
        {
            InitializeComponent();
            PreviewImage.Source = GetOriginalImage();
        }

        private void ApplyCurve_Click(object sender, RoutedEventArgs e)
        {
            // 获取原始图像
            BitmapSource original = GetOriginalImage();

            // 应用RGB曲线
            byte[] rgbCurve = RgbCurve.GetCurveData();
            BitmapSource adjusted = ImageProcess.ApplyCurve(original, rgbCurve, CurveChannel.RGB);

            // 应用红色通道曲线
            byte[] redCurve = RedCurve.GetCurveData();
            adjusted = ImageProcess.ApplyCurve(adjusted, redCurve, CurveChannel.Red);

            // 应用绿色通道曲线
            byte[] greenCurve = GreenCurve.GetCurveData();
            adjusted = ImageProcess.ApplyCurve(adjusted, greenCurve, CurveChannel.Green);

            // 应用蓝色通道曲线
            byte[] blueCurve = BlueCurve.GetCurveData();
            adjusted = ImageProcess.ApplyCurve(adjusted, blueCurve, CurveChannel.Blue);

            // 显示调整后的图像
            PreviewImage.Source = adjusted;
        }

        public WriteableBitmap GetOriginalImage()
        {
            var img = new BitmapImage(new Uri("th.jpeg", UriKind.Relative));
            WriteableBitmap wBitmap = new WriteableBitmap(img);
            return wBitmap;
        }
    }
}
