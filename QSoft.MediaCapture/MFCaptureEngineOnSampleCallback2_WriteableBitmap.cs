using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
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

        
        protected override void OnMediaTypeChanged(uint width, uint height)
        {
        }
    }
}
