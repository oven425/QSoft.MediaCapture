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
        readonly Dictionary<MF_CAPTURE_ENGINE_STREAM_CATEGORY, IReadOnlyList<ImageEncodingProperties>> m_VideoList = [];
        public IReadOnlyList<ImageEncodingProperties> GetAvailableMediaStreamProperties(MF_CAPTURE_ENGINE_STREAM_CATEGORY mediastreamtype)
        {
            if (!m_VideoList.ContainsKey(mediastreamtype)) m_VideoList[mediastreamtype] = [];
            if (m_VideoList[mediastreamtype].Count > 0) return m_VideoList[mediastreamtype];
            if (this.m_pEngine == null) return [];
            IMFCaptureSource? source = null;
            try
            {
                m_pEngine.GetSource(out source);
                if (source == null) return [];
                if (m_StreamGategory.TryGetValue(mediastreamtype, out var streamindex))
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
                    m_VideoList[mediastreamtype] = [.. lls];
                }
            }
            finally
            {
                SafeRelease(source);
            }
            return m_VideoList[mediastreamtype];
        }

        readonly Dictionary<MF_CAPTURE_ENGINE_STREAM_CATEGORY, uint> m_StreamGategory = [];

        void SupporCategory()
        {
            IMFCaptureSource? source = null;
            try
            {
                if (m_pEngine == null) return;
                var hr = m_pEngine.GetSource(out source);
                if (hr != HRESULTS.S_OK || source == null) return;
                source.GetDeviceStreamCount(out var streamcount);
                for (uint i = 0; i < streamcount; i++)
                {
                    hr = source.GetDeviceStreamCategory(i, out var category);
                    if (hr == HRESULTS.S_OK)
                    {
                        System.Diagnostics.Trace.WriteLine($"{category}:{i}");
                        m_StreamGategory[category] = i;
                    }
                }
            }
            finally
            {
                SafeRelease(source);
            }
        }

        public ImageEncodingProperties? GetMediaStreamProperties(MF_CAPTURE_ENGINE_STREAM_CATEGORY mediastreamtype)
        {
            IMFMediaType? mediatype = null;
            IMFCaptureSource? source = null;
            if (m_pEngine == null) return null;
            var hr = m_pEngine.GetSource(out source);
            if (hr != HRESULTS.S_OK || source == null) return null;
            try
            {
                source.GetCurrentDeviceMediaType(m_StreamGategory[mediastreamtype], out mediatype);
                var mm = new ImageEncodingProperties(mediatype, mediastreamtype);
                var ff = this.GetAvailableMediaStreamProperties(mediastreamtype).FirstOrDefault(x => x.Equals(mm));
                return ff;
            }
            finally
            { 
                SafeRelease(mediatype);
                SafeRelease(source); 
            }
        }

        TaskCompletionSource<HRESULT>? m_TaskSetCurrentType;
        public async Task SetMediaStreamPropertiesAsync(MF_CAPTURE_ENGINE_STREAM_CATEGORY mediastreamtype, ImageEncodingProperties? type)
        {
            if (type == null) return;
            IMFCaptureSource? source = null;
            if (m_pEngine == null) return;
            var hr = m_pEngine.GetSource(out source);
            if (hr != HRESULTS.S_OK || source == null) return;
            try
            {
                m_TaskSetCurrentType = new TaskCompletionSource<HRESULT>();
                var list = this.GetAvailableMediaStreamProperties(mediastreamtype);
                var sss = list.FirstOrDefault(x => x.Equals(type));
                if (sss != null)
                {
                    hr = source.SetCurrentDeviceMediaType(m_StreamGategory[mediastreamtype], sss.MediaType);
                    if (hr != HRESULTS.S_OK) return;
                    await m_TaskSetCurrentType.Task;
                }
            }
            finally
            {
                SafeRelease(source);
            }
        }

    }


    partial class ImageEncodingProperties
    {
        internal IMFMediaType MediaType { get; set; }
    }

    public partial class ImageEncodingProperties:IEquatable<ImageEncodingProperties>
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
        public uint ImageSize { get; }   
        public ImageEncodingProperties(IMFMediaType mediaType, MF_CAPTURE_ENGINE_STREAM_CATEGORY categroy, uint index=uint.MaxValue)
        {
            MediaType = mediaType;
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
            if(DirectN.Functions.MFCalculateImageSize(this.SubType, this.Width, this.Height, out var imagesize) == HRESULTS.S_OK)
            {
                this.ImageSize = imagesize;
            }
            //MFCalculateImageSize
        }

        public override string ToString()
        {
            return $"{Width}x{Height} {SubType.FormatToString()} {Fps:0} {ImageSize}";
        }

        public bool Equals(ImageEncodingProperties? y)
        {
            if(y==null)return false;
            var x = this;
            if (x.Width != y.Width) return false;
            else if (x.Height != y.Height) return false;
            else if (x.SubType != y.SubType) return false;
            else if (x.m_Denominator != y.m_Denominator) return false;
            else if (x.m_Numerator != y.m_Numerator) return false;
            return true;
        }
    } 

}
