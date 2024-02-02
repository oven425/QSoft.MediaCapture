using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace QSoft.MediaCapture.WPF
{
    public static partial class WebCam_MF_Extension
    {
        public static void StartPreview_WPF(this WebCam_MF src, Action<WriteableBitmap> action)
        {
            var sz = src.PreviewSize;
            var bmp = new WriteableBitmap(sz.width,sz.height, 96,96,PixelFormats.Bgr24, null);
            src.StartPreviewToCustomSinkAsync(new MFCaptureEngineOnSampleCallback(bmp));
            action(bmp);
        }
    }

    
}
