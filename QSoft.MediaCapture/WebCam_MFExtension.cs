//using DirectN;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace QSoft.MediaCapture
//{
//    public static class WebCam_MFExtension
//    {
//        public static HRESULT MFGetAttributeRatio(
//            this IMFAttributes pAttributes,
//            Guid guidKey,
//            out uint punNumerator,
//            out uint punDenominator
//            )
//        {
//            return MFGetAttribute2UINT32asUINT64(pAttributes, guidKey, out punNumerator, out punDenominator);
//        }

//        public static HRESULT MFGetAttributeSize(this IMFAttributes pAttributes, Guid guidKey, out uint punWidth, out uint punHeight)
//        {
//            return MFGetAttribute2UINT32asUINT64(pAttributes, guidKey, out punWidth, out punHeight);
//        }

//        public static HRESULT MFGetAttribute2UINT32asUINT64(this IMFAttributes pAttributes, Guid guidKey, out uint punHigh32, out uint punLow32)
//        {
//            ulong unPacked;
//            HRESULT hr;

//            hr = pAttributes.GetUINT64(guidKey, out unPacked);
//            if (hr.IsError)
//            {
//                punHigh32 = punLow32 = 0;
//                return hr;
//            }
//            Unpack2UINT32AsUINT64(unPacked, out punHigh32, out punLow32);

//            return hr;
//        }

//        public static void Unpack2UINT32AsUINT64(ulong unPacked, out uint punHigh, out uint punLow)
//        {
//            ulong ul = unPacked;
//            punHigh = (uint)(ul >> 32);
//            punLow = (uint)(ul & 0xffffffff);
//        }

//        public static List<IMFMediaType> GetAllMediaType(this IMFCaptureSource src, uint streamindex)
//        {
//            List<IMFMediaType> medias = new List<IMFMediaType>();
//            uint index = 0;
//            while (true)
//            {
//                IMFMediaType mediaType;
//                var hr = src.GetAvailableDeviceMediaType(streamindex, index, out mediaType);
//                if (hr == HRESULTS.MF_E_NO_MORE_TYPES)
//                {
//                    break;
//                }
//                medias.Add(mediaType);
//                index++;
//            }
//            return medias;
//        }

//        public static IEnumerable<(string name, Guid format, uint width, uint height, double fps, uint bitrate, IMFMediaType mediatype)> GetVideoData(this IEnumerable<IMFMediaType> src)
//        {
//            foreach (var oo in src)
//            {
//                oo.GetGUID(MFConstants.MF_MT_SUBTYPE, out var subtype);


//                MFGetAttributeSize(oo, MFConstants.MF_MT_FRAME_SIZE, out var w, out var h);

//                MFGetAttributeRatio(oo, MFConstants.MF_MT_FRAME_RATE, out var numerator, out var denominator);
//                var fps = (double)numerator / (double)denominator;

//                MFGetAttributeRatio(oo, MFConstants.MF_MT_FRAME_RATE_RANGE_MAX, out var numerator_max, out var denominator_max);
//                var fps_max = (double)numerator_max / (double)denominator_max;

//                MFGetAttributeRatio(oo, MFConstants.MF_MT_FRAME_RATE_RANGE_MIN, out var numerator_min, out var denominator_min);
//                var fps_min = (double)numerator_min / (double)denominator_min;

//                //System.Diagnostics.Trace.WriteLine($"fps:{fps} fps_max:{fps_max} fps_min:{fps_min}");

//                var bitrate = oo.GetUInt32(MFConstants.MF_MT_AVG_BITRATE);

//                //MF_MT_AVG_BITRATE
//                var hr = MFGetAttributeRatio(oo, MFConstants.MF_MT_PIXEL_ASPECT_RATIO, out var numerator1, out var denominator2);
//                //System.Diagnostics.Trace.WriteLine($"{numerator1}:{denominator2}");
//                yield return (subtype.FormatToString(), subtype, w, h, fps, bitrate, oo);
//            }
//        }

//        public static Dictionary<MF_CAPTURE_ENGINE_STREAM_CATEGORY, List<IMFMediaType>> GetAllMediaType(this IMFCaptureSource src)
//        {
//            Dictionary<MF_CAPTURE_ENGINE_STREAM_CATEGORY, List<IMFMediaType>> result = new Dictionary<MF_CAPTURE_ENGINE_STREAM_CATEGORY, List<IMFMediaType>>();
//            var hr1 = src.GetDeviceStreamCount(out var count);
//            System.Diagnostics.Trace.WriteLine($"GetDeviceStreamCount :hr1:{hr1} count:{count}");
//            for (uint i = 0; i < count; i++)
//            {
//                var hr = src.GetStreamIndexFromFriendlyName(i, out var streamIndex);
//                src.GetDeviceStreamCategory(i, out var caegory);
//                var medias = src.GetAllMediaType(i);
//                //src.GetStreamIndexFromFriendlyName(i);
//                result[caegory] = medias;
//            }

//            return result;
//        }

//        internal static (uint width, uint height, Guid subtype, double fps)GetVideoTypeInfo(this IMFMediaType src)
//        {
//            src.GetGUID(MFConstants.MF_MT_SUBTYPE, out var subtype);
//            MFGetAttributeSize(src, MFConstants.MF_MT_FRAME_SIZE, out var w, out var h);
//            MFGetAttributeRatio(src, MFConstants.MF_MT_FRAME_RATE, out var numerator, out var denominator);
//            var fps = (double)numerator / (double)denominator;
//            return (w,h,subtype, fps);
//        }

//        public static string FormatToString(this Guid src)
//        {
//            if (src == MFConstants.MFVideoFormat_MJPG) return "MJPG";
//            else if (src == MFConstants.MFVideoFormat_NV12) return "NV12";
//            else if (src == MFConstants.MFVideoFormat_YV12) return "YV12";
//            else if (src == MFConstants.MFVideoFormat_YUY2) return "YUY2";
//            else if (src == MFConstants.MFAudioFormat_AAC) return "AAC";
//            return src.ToString();
//        }
//    }

//    public record PreviewMediaType
//    {
//        public Guid SubType { set; get; }
//        public uint Width { set; get; }
//        public uint Height { set; get; }
//        public double Fps { set; get; }
//    }

    
//}
