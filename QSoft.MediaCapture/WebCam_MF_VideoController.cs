using DirectN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace QSoft.MediaCapture
{
    public partial class WebCam_MF
    {
        readonly Dictionary<MF_CAPTURE_ENGINE_STREAM_CATEGORY, IReadOnlyList<ImageEncodingProperties>> m_VideoList = [];
        //public IReadOnlyList<ImageEncodingProperties> GetAvailableMediaStreamProperties(MF_CAPTURE_ENGINE_STREAM_CATEGORY mediastreamtype)
        //{
        //    if (!m_VideoList.ContainsKey(mediastreamtype)) m_VideoList[mediastreamtype] = [];
        //    if (m_VideoList[mediastreamtype].Count > 0) return m_VideoList[mediastreamtype];
        //    if (this.m_pEngine == null) return [];
        //    IMFCaptureSource? source = null;
        //    try
        //    {
        //        m_pEngine.GetSource(out source);
        //        if (source == null) return [];
        //        if (m_StreamGategory.TryGetValue(mediastreamtype, out var streamindex))
        //        {
        //            var lls = new List<ImageEncodingProperties>();
        //            uint index = 0;

        //            while (true)
        //            {
        //                var hr = source.GetAvailableDeviceMediaType(streamindex[0], index, out var mediatype);
        //                if (hr != HRESULTS.S_OK) break;
        //                var mm = new ImageEncodingProperties(mediatype, mediastreamtype, index);
        //                lls.Add(mm);
        //                index++;
        //            }
        //            m_VideoList[mediastreamtype] = [.. lls];
        //        }
        //    }
        //    finally
        //    {
        //        SafeRelease(source);
        //    }
        //    return m_VideoList[mediastreamtype];
        //}

        public IReadOnlyList<ImageEncodingProperties> GetAvailableMediaStreamProperties(MF_CAPTURE_ENGINE_STREAM_CATEGORY streamcategory, int? streamindex=null)
        {
            if(m_Streams.Count ==0)
            {
                this.GetMM();
            }
            if(streamindex is null)
            {
                return [.. m_Streams.Where(x => x.StreamGategory == streamcategory)];
            }
            return [.. m_Streams.Where(x => x.StreamGategory == streamcategory && x.StreamIndex==streamindex)];
        }




        readonly List<ImageEncodingProperties> m_Streams = [];
        public void GetMM()
        {
            m_Streams.Clear();
            m_VideoList.Clear();
            if (m_pEngine == null) return;
            IMFCaptureSource? source = null;
            try
            {
                m_pEngine?.GetSource(out source);
                if (source == null) return;
                var hr = source.GetDeviceStreamCount(out var streamcount);

                for (int i=0; i<streamcount; i++)
                {
                    hr = source.GetDeviceStreamCategory((uint)i, out var category);
                    uint index = 0;
                    while (true)
                    {
                        hr = source.GetAvailableDeviceMediaType((uint)i, index, out var mediatype);
                        if (hr != HRESULTS.S_OK) break;
                        var mm = new ImageEncodingProperties(mediatype, category, (uint)i, index);
                        m_Streams.Add(mm);
                        index++;
                    }

                }

            }
            finally
            {
                SafeRelease(source);
            }
        }

        readonly Dictionary<MF_CAPTURE_ENGINE_STREAM_CATEGORY, List<uint>> m_StreamGategory = [];
        [Obsolete]
        public ImageEncodingProperties? GetMediaStreamProperties(MF_CAPTURE_ENGINE_STREAM_CATEGORY mediastreamtype)
        {
            IMFMediaType? mediatype = null;
            IMFCaptureSource? source = null;
            if (m_pEngine == null) return null;
            var hr = m_pEngine.GetSource(out source);
            if (hr != HRESULTS.S_OK || source == null) return null;
            try
            {
                source.GetCurrentDeviceMediaType(m_StreamGategory[mediastreamtype][0], out mediatype);
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

        public ImageEncodingProperties? GetMediaStreamProperties(MF_CAPTURE_ENGINE_STREAM_CATEGORY streamcatgory, uint? streamindex)
        {
            IMFMediaType? mediatype = null;
            IMFCaptureSource? source = null;
            if (m_pEngine == null) return null;
            var hr = m_pEngine.GetSource(out source);
            if (hr != HRESULTS.S_OK || source == null) return null;
            try
            {
                uint streamindex_ = 0;
                if (streamindex is not null)
                {
                    streamindex_ = streamindex.Value;
                }
                else
                {
                    var ssss = this.GetAvailableMediaStreamProperties(streamcatgory)
                        .GroupBy(x => x.StreamIndex).FirstOrDefault().Key;
                }

                source.GetCurrentDeviceMediaType(streamindex_, out mediatype);

                var mm = new ImageEncodingProperties(mediatype, streamcatgory, streamindex_);
                var ff1 = this.GetAvailableMediaStreamProperties(streamcatgory, (int)streamindex_).FirstOrDefault(x => x.Equals(mm));
                return ff1;
            }
            finally
            {
                SafeRelease(mediatype);
                SafeRelease(source);
            }
        }

        TaskCompletionSource<HRESULT>? m_TaskSetCurrentType;
        [Obsolete]
        public async Task SetMediaStreamPropertiesAsync(MF_CAPTURE_ENGINE_STREAM_CATEGORY streamcategory, ImageEncodingProperties? type)
        {
            if (type == null) return;
            IMFCaptureSource? source = null;
            if (m_pEngine == null) return;
            var hr = m_pEngine.GetSource(out source);
            if (hr != HRESULTS.S_OK || source == null) return;
            try
            {
                m_TaskSetCurrentType = new TaskCompletionSource<HRESULT>();
                var list = this.GetAvailableMediaStreamProperties(streamcategory);
                var sss = list.FirstOrDefault(x => x.Equals(type));
                if (sss != null)
                {
                    hr = source.SetCurrentDeviceMediaType(0, sss.MediaType);
                    if (hr != HRESULTS.S_OK) return;
                    await m_TaskSetCurrentType.Task;

                    //m_TaskSetCurrentType = new TaskCompletionSource<HRESULT>();
                    //hr = source.SetCurrentDeviceMediaType(1, sss.MediaType);
                    //await m_TaskSetCurrentType.Task;
                    //hr = source.GetCurrentDeviceMediaType(0, out var m0);
                    //var mm0 = new ImageEncodingProperties(m0, MF_CAPTURE_ENGINE_STREAM_CATEGORY.MF_CAPTURE_ENGINE_STREAM_CATEGORY_VIDEO_CAPTURE, 0);
                    //hr = source.GetCurrentDeviceMediaType(1, out var m1);
                    //var mm1 = new ImageEncodingProperties(m1, MF_CAPTURE_ENGINE_STREAM_CATEGORY.MF_CAPTURE_ENGINE_STREAM_CATEGORY_VIDEO_CAPTURE, 0);
                    
                }
            }
            finally
            {
                SafeRelease(source);
            }
        }
        TaskCompletionSource<HRESULT>? m_TaskSetCurrentTypeDynamic;
        public async Task SetMediaStreamPropertiesAsync(ImageEncodingProperties? type)
        {
            if (type == null) return;
            IMFCaptureSource? source = null;
            if (m_pEngine == null) return;
            var hr = m_pEngine.GetSource(out source);
            if (hr != HRESULTS.S_OK || source == null) return;
            try
            {
                if (this.IsPreviewing||this.IsRecording)
                {
                    hr = m_pEngine.GetSink(MF_CAPTURE_ENGINE_SINK_TYPE.MF_CAPTURE_ENGINE_SINK_TYPE_PREVIEW, out var pSink);
                    if (hr != HRESULTS.S_OK) return;
                    if (pSink is IMFCaptureSink2 sink2)
                    {
                        m_TaskSetCurrentTypeDynamic = new();
                        hr = sink2.SetOutputMediaType(type.StreamIndex, type.MediaType, null);
                        if (hr == HRESULTS.S_OK)
                        {
                            await m_TaskSetCurrentTypeDynamic.Task;
                        }

                        WebCam_MF.SafeRelease(sink2);
                    }
                }
                //else
                {
                    m_TaskSetCurrentType = new TaskCompletionSource<HRESULT>();
                    hr = source.SetCurrentDeviceMediaType(type.StreamIndex, type.MediaType);
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
        internal void Dispose()
        {
            WebCam_MF.SafeRelease(this.MediaType);
        }
    }

    public partial class ImageEncodingProperties:IEquatable<ImageEncodingProperties>
    {
        public uint StreamIndex { private set; get; }
        public uint MediaTypeIndex { private set; get; }
        public uint Width { set; get; }
        public uint Height { set; get; }
        public Guid SubType { set; get; }
        public Guid MajorType { set; get; } = Guid.Empty;
        public MF_CAPTURE_ENGINE_STREAM_CATEGORY StreamGategory { private set; get; }
        readonly uint m_Numerator;
        readonly uint m_Denominator;
        public float Fps { set; get; }
        public uint ImageSize { get; }   
        public ImageEncodingProperties(IMFMediaType mediaType, MF_CAPTURE_ENGINE_STREAM_CATEGORY categroy, uint streamindex = uint.MaxValue, uint mediatypeindex=uint.MaxValue)
        {
            StreamIndex = streamindex;
            MediaType = mediaType;
            this.MediaTypeIndex = mediatypeindex;
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
            else if (x.StreamGategory != y.StreamGategory) return false;
            else if (x.StreamIndex != y.StreamIndex) return false;
            return true;
        }
    } 

}
