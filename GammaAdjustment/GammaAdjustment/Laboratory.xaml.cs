using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace GammaAdjustment
{
    /// <summary>
    /// Laboratory.xaml 的交互逻辑
    /// </summary>
    public partial class Laboratory : Window
    {
        private WriteableBitmap wBitmap;
        private List<Point> controlPoints = new List<Point>();

        public Laboratory()
        {
            InitializeComponent();
            DataContext = this; // 绑定 ControlPoints
                                // 初始化曲线控制点（两个端点）
            controlPoints.Add(new Point(0, 255));
            controlPoints.Add(new Point(255, 0));
            // 将初始点集合复制给可观察集合
            foreach (var p in controlPoints) 
                ControlPoints.Add(p);
            // 加载图像到 WriteableBitmap
            var img = new BitmapImage(new Uri("th.jpeg", UriKind.Relative));
            wBitmap = new WriteableBitmap(img);
            ImageViewer.Source = wBitmap;
            UpdateCurvePath();
            ApplyCurveToImage();
        }

        // 绑定属性（用于 ItemsControl）
        public ObservableCollection<Point> ControlPoints { get; } = new ObservableCollection<Point>();

        // 鼠标点击画布添加新控制点
        private void CurveCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(CurveCanvas);
            // 限制在 [0,255] 内
            p.X = Math.Max(0, Math.Min(255, p.X));
            p.Y = Math.Max(0, Math.Min(255, p.Y));
            ControlPoints.Add(p);
            UpdateCurvePath();
            ApplyCurveToImage();
        }

        // 拖动控制点时触发
        private void ControlPoint_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Thumb thumb = (Thumb)sender;
            Point pt = (Point)thumb.DataContext;
            pt.X += e.HorizontalChange;
            pt.Y += e.VerticalChange;
            pt.X = Math.Max(0, Math.Min(255, pt.X));
            pt.Y = Math.Max(0, Math.Min(255, pt.Y));
            // 更新集合（Point 结构体需替换）
            int idx = ControlPoints.IndexOf((Point)thumb.DataContext);
            if (idx >= 0) ControlPoints[idx] = pt;
            UpdateCurvePath();
            ApplyCurveToImage();
        }

        // 根据控制点绘制路径（简单线性连接示例）
        private void UpdateCurvePath()
        {
            var pts = ControlPoints.OrderBy(p => p.X).ToList();
            var fig = new PathFigure { StartPoint = pts[0] };
            for (int i = 1; i < pts.Count; i++)
            {
                fig.Segments.Add(new LineSegment(pts[i], true));
            }
            CurvePath.Data = new PathGeometry(new[] { fig });
        }

        // 应用曲线映射到图像
        private void ApplyCurveToImage()
        {
            // 生成查找表（假设灰度通道变化同时作用于 RGB）
            int[] lut = new int[256];
            var pts = ControlPoints.OrderBy(p => p.X).ToList();
            for (int i = 0; i < 256; i++)
            {
                // 找插值段
                Point p0 = pts[0], p1 = pts.Last();
                for (int k = 0; k < pts.Count - 1; k++)
                {
                    if (pts[k].X <= i && i <= pts[k + 1].X)
                    {
                        p0 = pts[k]; p1 = pts[k + 1]; break;
                    }
                }
                double t = (i - p0.X) / (p1.X - p0.X);
                double y = p0.Y + (p1.Y - p0.Y) * t;
                int v = (int)Math.Round(y);
                lut[i] = Math.Max(0, Math.Min(255, v));
            }
            // 修改 WriteableBitmap 像素
            wBitmap.Lock();
            unsafe
            {
                byte* buffer = (byte*)wBitmap.BackBuffer;
                int stride = wBitmap.BackBufferStride;
                int height = wBitmap.PixelHeight;
                int width = wBitmap.PixelWidth;
                for (int y = 0; y < height; y++)
                {
                    byte* row = buffer + y * stride;
                    for (int x = 0; x < width; x++)
                    {
                        int idx = x * 4;
                        byte b = row[idx], g = row[idx + 1], r = row[idx + 2];
                        row[idx] = (byte)lut[b]; // 蓝色通道
                        row[idx + 1] = (byte)lut[g]; // 绿色通道
                        row[idx + 2] = (byte)lut[r]; // 红色通道
                    }
                }
            }
            wBitmap.AddDirtyRect(new Int32Rect(0, 0, wBitmap.PixelWidth, wBitmap.PixelHeight));
            wBitmap.Unlock();
        }
    }
}
