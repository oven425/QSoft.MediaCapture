using DirectN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace QSoft.MediaCapture
{
    public partial class WebCam_MF
    {
        public IReadOnlyList<ImageEncodingProperties> GetAvailableMediaStreamProperties(MF_CAPTURE_ENGINE_STREAM_CATEGORY mediastreamtype)
        {
            if (this.m_pEngine == null) return [];
            IMFCaptureSource? source = null;
            try
            {
                var category = this.SupporCategory();
                m_pEngine.GetSource(out source);
                if (source == null) return [];
                if (category.TryGetValue(mediastreamtype, out var streamindex))
                {
                    var lls = new List<ImageEncodingProperties>();
                    uint index = 0;

                    while (true)
                    {
                        var hr = source.GetAvailableDeviceMediaType(streamindex, index, out var mediatype);
                        if (hr != HRESULTS.S_OK) break;
                        var mm = new ImageEncodingProperties(mediatype, mediastreamtype, index);
                        lls.Add(mm);
                        index++;
                    }
                    return lls.AsReadOnly();
                }
            }
            finally
            {
                SafeRelease(source);
            }
            return [];
        }

        Dictionary<MF_CAPTURE_ENGINE_STREAM_CATEGORY, uint> SupporCategory()
        {
            Dictionary<MF_CAPTURE_ENGINE_STREAM_CATEGORY, uint> stream_category = [];
            IMFCaptureSource? source = null;
            try
            {
                if (m_pEngine == null) return stream_category;
                var hr = m_pEngine.GetSource(out source);
                if (source == null) return stream_category;
                source.GetDeviceStreamCount(out var streamcount);
                for (uint i = 0; i < streamcount; i++)
                {
                    hr = source.GetDeviceStreamCategory(i, out var category);
                    if (hr == HRESULTS.S_OK)
                    {
                        stream_category[category] = i;
                    }
                }
            }
            finally
            {
                SafeRelease(source);
            }

            return stream_category;
        }

        public ImageEncodingProperties? GetMediaStreamProperties(MF_CAPTURE_ENGINE_STREAM_CATEGORY mediastreamtype)
        {
            IMFCaptureSource? source = null;
            if (m_pEngine == null) return null;
            var hr = m_pEngine.GetSource(out source);
            if (source == null) return null;
            var category = this.SupporCategory();
            try
            {
                hr = source.GetCurrentDeviceMediaType(category[mediastreamtype], out var mediatype);
                var mm = new ImageEncodingProperties(mediatype, mediastreamtype);
                return mm;
            }
            finally
            { 
                SafeRelease(source); 
            }
        }

        TaskCompletionSource<HRESULT> m_TaskSetCurrentType;
        public void SetMediaStreamPropertiesAsync(MF_CAPTURE_ENGINE_STREAM_CATEGORY mediastreamtype, ImageEncodingProperties type)
        {
            IMFCaptureSource? source = null;
            if (m_pEngine == null) return;
            var hr = m_pEngine.GetSource(out source);
            if (source == null) return;
            var category = this.SupporCategory();
            try
            {
                var list = this.GetAvailableMediaStreamProperties(mediastreamtype);
                var sss = list.FirstOrDefault(x => x == type);
                if(sss != null)
                {
                    //source.GetAvailableDeviceMediaType()
                    //hr = source.SetCurrentDeviceMediaType(category[mediastreamtype], out var mediatype);

                }
                //var mm = new ImageEncodingProperties(mediatype);
            }
            finally
            {
                SafeRelease(source);
            }
        }

    }

    public class VideoController
    {
        readonly Dictionary<MF_CAPTURE_ENGINE_STREAM_CATEGORY, uint> m_Category = [];
        Dictionary<MF_CAPTURE_ENGINE_STREAM_CATEGORY, List<ImageEncodingProperties>> m_aa;
        public VideoController(IMFCaptureEngine engine)
        {
            this.Init(engine);
        }
        void Init(IMFCaptureEngine engine)
        {
            var hr = engine.GetSource(out var source);
            if (source == null) return;
            this.m_Category.AddRange(this.SupporCategory(source));
            
        }

        public void CheckEvent(HRESULT hr, Guid guid)
        {

        }

        Dictionary<MF_CAPTURE_ENGINE_STREAM_CATEGORY, uint> SupporCategory(IMFCaptureSource source)
        {
            Dictionary<MF_CAPTURE_ENGINE_STREAM_CATEGORY, uint> stream_category = [];
            source.GetDeviceStreamCount(out var streamcount);
            for (uint i = 0; i < streamcount; i++)
            {
                var hr = source.GetDeviceStreamCategory(i, out var category);
                if (hr == HRESULTS.S_OK)
                {
                    stream_category[category] = i;
                }
            }

            return stream_category;
        }

    }

    public class ImageEncodingProperties
    {
        public uint StreamIndex { private set; get; }
        public uint Width { set; get; }
        public uint Height { set; get; }
        public Guid SubType { set; get; }
        public Guid MajorType { set; get; } = Guid.Empty;
        public MF_CAPTURE_ENGINE_STREAM_CATEGORY StreamGategory { private set; get; }
        readonly uint m_Numerator;
        readonly uint m_Denominator;
        public float Fps { set; get; }
        public ImageEncodingProperties(IMFMediaType mediaType, MF_CAPTURE_ENGINE_STREAM_CATEGORY categroy, uint index=uint.MaxValue)
        {
            this.StreamIndex = index;
            this.StreamGategory = categroy;
            if(mediaType.TryGetSize(MFConstants.MF_MT_FRAME_SIZE, out var w, out var h))
            {
                this.Width = w;
                this.Height = h;
            }

            if(mediaType.GetGUID(MFConstants.MF_MT_SUBTYPE, out var subtype) == HRESULTS.S_OK)
            {
                this.SubType = subtype;
            }
            if (mediaType.GetGUID(MFConstants.MF_MT_MAJOR_TYPE, out var major) == HRESULTS.S_OK)
            {
                this.MajorType = major;
            }
            if (mediaType.TryGetRatio(MFConstants.MF_MT_FRAME_RATE, out m_Numerator, out m_Denominator))
            {
                this.Fps = (float)m_Numerator / m_Denominator;
            }

        }

        public static bool operator ==(ImageEncodingProperties x, ImageEncodingProperties y)
        {
            if(x.Width != y.Width) return false;
            else if(x.Height != y.Height) return false;
            else if(x.SubType != y.SubType) return false;
            else if(x.m_Denominator != y.m_Denominator) return false;
            else if(x.m_Numerator != y.m_Numerator)  return false; 
            return true;
        }

        public static bool operator !=(ImageEncodingProperties x, ImageEncodingProperties y)
        {
            return !(x == y);
        }
    }

    
}
