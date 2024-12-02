using DirectN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QSoft.MediaCapture
{
    public static class WebCam_MFExtension
    {
        public static string FormatToString(this Guid src)
        {
            if (src == MFConstants.MFVideoFormat_MJPG) return "MJPG";
            else if (src == MFConstants.MFVideoFormat_NV12) return "NV12";
            else if (src == MFConstants.MFVideoFormat_YV12) return "YV12";
            else if (src == MFConstants.MFVideoFormat_YUY2) return "YUY2";
            else if (src == MFConstants.MFVideoFormat_RGB24) return "RGB24";
            else if (src == MFConstants.MFVideoFormat_RGB8) return "RGB8";
            else if (src == MFConstants.MFVideoFormat_H264) return "H264";
            else if (src == MFConstants.MFVideoFormat_H265) return "H265";
            else if (src == MFConstants.MFAudioFormat_AAC) return "AAC";
            return src.ToString();
        }
    }
}
