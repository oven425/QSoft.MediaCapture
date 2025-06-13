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
        public static float Fps(this IMFMediaType src)
        {
            if (src.TryGetRatio(MFConstants.MF_MT_FRAME_RATE, out var m_Numerator, out var m_Denominator))
            {
                return (float)m_Numerator / m_Denominator;
            }
            return 0;
        }
        public static string FormatToString(this Guid src)
        {
            if (src == MFConstants.MFVideoFormat_MJPG) return "MJPG";
            else if (src == MFConstants.MFVideoFormat_NV12) return "NV12";
            else if (src == MFConstants.MFVideoFormat_YV12) return "YV12";
            else if (src == MFConstants.MFVideoFormat_YUY2) return "YUY2";
            else if (src == MFConstants.MFVideoFormat_NV21) return "NV21";
            else if (src == MFConstants.MFVideoFormat_HEVC) return "HEVC";
            else if (src == MFConstants.MFVideoFormat_RGB24) return "RGB24";
            else if (src == MFConstants.MFVideoFormat_RGB8) return "RGB8";
            else if (src == MFConstants.MFVideoFormat_H264) return "H264";
            else if (src == MFConstants.MFVideoFormat_H265) return "H265";
            else if (src == MFConstants.MFAudioFormat_AAC) return "AAC";
            else if (src == MFConstants.MFVideoFormat_L8) return "L8";
            else if (src == MFConstants.MFVideoFormat_L16) return "L16";
            else if (src == MFConstants.MFAudioFormat_Float) return "Float";
            return src.ToString();
        }
    }
}
