using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace GammaAdjustment
{
    public enum CurveChannel
    { 
        RGB,
        Red,
        Green,
        Blue
    }


    public static class ImageProcess
    {

        public static BitmapSource ApplyCurve(BitmapSource source, byte[] curve, CurveChannel channel)
        {
            // 将BitmapSource转换为可编辑的格式
            var writable = new WriteableBitmap(source);

            // 锁定位图
            writable.Lock();

            // 获取像素数据
            int width = writable.PixelWidth;
            int height = writable.PixelHeight;
            int stride = writable.BackBufferStride;
            IntPtr buffer = writable.BackBuffer;

            // 处理每个像素
            unsafe
            {
                byte* ptr = (byte*)buffer.ToPointer();

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int index = y * stride + x * 4;

                        // 根据通道应用曲线
                        switch (channel)
                        {
                            case CurveChannel.RGB:
                                ptr[index + 2] = curve[ptr[index + 2]]; // R
                                ptr[index + 1] = curve[ptr[index + 1]]; // G
                                ptr[index] = curve[ptr[index]];         // B
                                break;
                            case CurveChannel.Red:
                                ptr[index + 2] = curve[ptr[index + 2]]; // R
                                break;
                            case CurveChannel.Green:
                                ptr[index + 1] = curve[ptr[index + 1]]; // G
                                break;
                            case CurveChannel.Blue:
                                ptr[index] = curve[ptr[index]];         // B
                                break;
                        }
                    }
                }
            }

            // 解锁并返回
            writable.Unlock();
            writable.Freeze();
            return writable;
        }
    }
}
