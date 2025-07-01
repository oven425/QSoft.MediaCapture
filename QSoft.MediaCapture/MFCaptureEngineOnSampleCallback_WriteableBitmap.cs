using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace QSoft.MediaCapture.WPF
{
    internal class MFCaptureEngineOnSampleCallback_WriteableBitmap : MFCaptureEngineOnSampleCallback
    {
        readonly WriteableBitmap? m_Bmp;
        readonly System.Windows.Threading.DispatcherPriority m_DispatcherPriority;
        public MFCaptureEngineOnSampleCallback_WriteableBitmap(WriteableBitmap? data, System.Windows.Threading.DispatcherPriority dispatcherpriority)
        {
            m_DispatcherPriority = dispatcherpriority;
            this.m_Bmp = data;
        }

        protected override void OnSample(nint data, uint len)
        {
            try
            {
                m_Bmp?.Dispatcher.Invoke(() =>
                {
                    m_Bmp.Lock();
                    WebCam_MFExtension.CopyMemory(m_Bmp.BackBuffer, data, len);
                    m_Bmp.AddDirtyRect(new System.Windows.Int32Rect(0, 0, m_Bmp.PixelWidth, m_Bmp.PixelHeight));
                    m_Bmp.Unlock();
                }, m_DispatcherPriority);
            }
            catch (Exception e)
            {

            }
        }
    }


}
