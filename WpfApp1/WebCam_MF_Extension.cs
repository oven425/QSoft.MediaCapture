using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace QSoft.MediaCapture
{
    public static class WebCam_MF_Extension
    {
        public static void StartPreview(this WebCam_MF src, Action<WriteableBitmap> action)
        {
            WriteableBitmap bmp = null;
            
            src.StartPreviewToCustomSinkAsync(new MFCaptureEngineOnSampleCallback(bmp));
            action(bmp);
        }
    }

    
}
