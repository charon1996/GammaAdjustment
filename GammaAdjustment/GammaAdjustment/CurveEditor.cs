using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.Immutable;

namespace GammaAdjustment
{
    public class CurveEditor : Control
    {
        public event Action CurveChangedEvent;

        // 定义控制点集合
        private readonly ObservableCollection<Point> _controlPoints = new ObservableCollection<Point>();

        // 当前选中的控制点索引
        private int _selectedPointIndex = -1;

        // 曲线数据（从0到255的映射）
        private byte[] _curveData = new byte[256];

        public CurveEditor()
        {
            // 初始化默认控制点（起点和终点）
            _controlPoints.Add(new Point(0, 0)); // 左下角
            _controlPoints.Add(new Point(255, 255)); // 右上角

            // 计算初始曲线
            CalculateCurve();
        }

        // 计算曲线数据（使用Catmull-Rom样条插值）
        private void CalculateCurve()
        {
            // 排序控制点
            var sortedPoints = _controlPoints.OrderBy(p => p.X).ToList();

            // 计算插值
            for (int i = 0; i < 256; i++)
            {
                _curveData[i] = (byte)CalculateSplineValue(i, sortedPoints);
            }

            // 触发重绘和值变更事件
            InvalidateVisual();
            OnCurveChanged();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            // 绘制背景网格
            DrawGrid(dc);

            // 绘制曲线
            DrawCurve(dc);

            // 绘制控制点
            DrawControlPoints(dc);
        }

      
        // 获取曲线数据
        public byte[] GetCurveData()
        {
            return (byte[])_curveData.Clone();
        }

   
        // 坐标转换方法
        private Point NormalizePoint(Point point)
        {
            double x = point.X * ActualWidth / 255;
            double y = (255 - point.Y) * ActualHeight / 255;
            return new Point(x, y);
        }

        private Point DenormalizePoint(Point point)
        {
            double x = point.X * 255 / ActualWidth;
            double y = 255 - (point.Y * 255 / ActualHeight);
            return new Point(x, y);
        }

        // 样条插值计算（Catmull-Rom实现）
        private double CalculateSplineValue(double x, List<Point> points)
        {
            if (points.Count < 2) return x;
            if (points.Count == 2) // 线性插值
            {
                var p00 = points[0];
                var p01 = points[1];
                return p00.Y + (x - p00.X) * (p01.Y - p00.Y) / (p01.X - p00.X);
            }

            // 找到包含x的区间
            int i = 0;
            while (i < points.Count - 1 && x > points[i + 1].X)
                i++;

            //if (i >= points.Count - 1) return points.Last().Y;
            
            // Catmull-Rom样条参数
            Point p0 = i > 0 ? points[i - 1] : points[i];
            Point p1 = points[i];
            Point p2 = points[i + 1];
            Point p3 = (i + 2 < points.Count) ? points[i + 2] : p2;


            //if (points.Count == 3 && i == 1)
            //{
            //    p0 = points[0];
            //    p1 = points[0];
            //    p2 = points[1];
            //    p3 = points[2];
            //}


            double t = (x - p1.X) / (p2.X - p1.X);
            t = Math.Max(0, Math.Min(1, t));

            double t2 = t * t;
            double t3 = t2 * t;

            return 0.5 * (
                (2 * p1.Y) +
                (-p0.Y + p2.Y) * t +
                (2 * p0.Y - 5 * p1.Y + 4 * p2.Y - p3.Y) * t2 +
                (-p0.Y + 3 * p1.Y - 3 * p2.Y + p3.Y) * t3
            );
        }

        // 绘制网格
        private void DrawGrid(DrawingContext dc)
        {
            Pen gridPen = new Pen(Brushes.LightGray, 0.5);
            double stepX = ActualWidth / 4;
            double stepY = ActualHeight / 4;

            // 垂直线
            for (int i = 1; i < 4; i++)
            {
                double x = i * stepX;
                dc.DrawLine(gridPen, new Point(x, 0), new Point(x, ActualHeight));
            }

            // 水平线
            for (int i = 1; i < 4; i++)
            {
                double y = i * stepY;
                dc.DrawLine(gridPen, new Point(0, y), new Point(ActualWidth, y));
            }

            // 边框
            dc.DrawRectangle(null, new Pen(Brushes.Gray, 1),
                new Rect(0, 0, ActualWidth, ActualHeight));
        }

        // 绘制曲线
        private void DrawCurve(DrawingContext dc)
        {
            if (_curveData == null) return;

            PathGeometry geometry = new PathGeometry();
            PathFigure figure = new PathFigure();
            figure.StartPoint = NormalizePoint(new Point(0, _curveData[0]));

            for (int i = 1; i < 256; i++)
            {
                figure.Segments.Add(new LineSegment(
                    NormalizePoint(new Point(i, _curveData[i])), true));
            }

            geometry.Figures.Add(figure);
            dc.DrawGeometry(null, new Pen(Brushes.Blue, 2), geometry);
        }

        // 绘制控制点
        private void DrawControlPoints(DrawingContext dc)
        {
            Brush normalBrush = Brushes.White;
            Brush selectedBrush = Brushes.Red;
            double pointSize = 8;

            foreach (var point in _controlPoints)
            {
                Point screenPoint = NormalizePoint(point);
                bool isSelected = _controlPoints.IndexOf(point) == _selectedPointIndex;

                dc.DrawEllipse(isSelected ? selectedBrush : normalBrush,
                    new Pen(Brushes.Black, 1),
                    screenPoint,
                    pointSize,
                    pointSize);
            }
        }

        // 触发曲线变化事件
        protected virtual void OnCurveChanged()
        {
            CurveChangedEvent?.Invoke();
        }

        // 完善鼠标交互
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            Point clickPoint = e.GetPosition(this);
            Point normalized = DenormalizePoint(clickPoint);

            // 检查是否点击了现有控制点
            for (int i = 0; i < _controlPoints.Count; i++)
            {
                Point testPoint = NormalizePoint(_controlPoints[i]);
                Rect hitRect = new Rect(
                    testPoint.X - 10,
                    testPoint.Y - 10,
                    20,
                    20);

                if (hitRect.Contains(clickPoint))
                {
                    _selectedPointIndex = i;
                    return;
                }
            }

            // 添加新控制点（确保X坐标不重复）
            if (!_controlPoints.Any(p => Math.Abs(p.X - normalized.X) < 1))
            {
                _controlPoints.Add(new Point(
                    Math.Max(0, Math.Min(255, normalized.X)),
                    Math.Max(0, Math.Min(255, normalized.Y))));

                // 按X坐标排序
                _controlPoints.Sort((a, b) => a.X.CompareTo(b.X));
                _selectedPointIndex = _controlPoints.IndexOf(
                    _controlPoints.First(p => Math.Abs(p.X - normalized.X) < 1));

                CalculateCurve();
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            _selectedPointIndex = -1;
        }


        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_selectedPointIndex >= 0 && e.LeftButton == MouseButtonState.Pressed)
            {
                Point newPos = DenormalizePoint(e.GetPosition(this));

                // 限制移动范围
                newPos.X = Math.Max(0, Math.Min(255, newPos.X));
                newPos.Y = Math.Max(0, Math.Min(255, newPos.Y));

                // 保持X坐标唯一
                if (!_controlPoints.Where((_, i) => i != _selectedPointIndex)
                    .Any(p => Math.Abs(p.X - newPos.X) < 1))
                {
                    _controlPoints[_selectedPointIndex] = newPos;
                    _controlPoints.Sort((a, b) => a.X.CompareTo(b.X));
                    _selectedPointIndex = _controlPoints.IndexOf(newPos);
                    CalculateCurve();
                }
            }
        }
    }
}
