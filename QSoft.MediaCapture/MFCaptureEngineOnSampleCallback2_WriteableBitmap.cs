using QSoft.MediaCapture.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace QSoft.MediaCapture
{
    internal class MFCaptureEngineOnSampleCallback2_WriteableBitmap(WriteableBitmap bmp, Func<System.Windows.Controls.Image> func, System.Windows.Threading.DispatcherPriority dispatcherpriority)
        :MFCaptureEngineOnSampleCallback2
    {

        protected override void OnSample(IntPtr data, uint len)
        {
            try
            {
                
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    if(bmp.PixelWidth != m_Width || bmp.PixelHeight != m_Height)
                    {
                        bmp = WebCam_MFExtension_WPF.CreateWriteableBitmap((uint)m_Width, (uint)m_Height);
                        func().Source = bmp;
                    }
                    bmp.Lock();
                    WebCam_MFExtension.CopyMemory(bmp.BackBuffer, data, len);
                    bmp.AddDirtyRect(new System.Windows.Int32Rect(0, 0, bmp.PixelWidth, bmp.PixelHeight));
                    bmp.Unlock();
                }, dispatcherpriority);
            }
            catch (Exception e)
            {

            }

        }
        int m_Width = bmp.PixelWidth;
        int m_Height = bmp.PixelHeight;
        protected override void OnMediaTypeChanged(uint width, uint height)
        {
            m_Width = (int)width;
            m_Height = (int)height;
        }
    }
}
