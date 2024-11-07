using DirectN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

//https://learn.microsoft.com/zh-tw/windows/win32/api/mfidl/nn-mfidl-imfcameraconfigurationmanager

namespace QSoft.MediaCapture
{
    public class WebCam_MF_Size
    {
        public uint Width { set; get; }
        public uint Height { set; get; }
    }
    public class WebCam_MF_Setting
    {
        public bool IsMirror { set; get; }
        public uint Rotate { set; get; }
        public WebCam_MF_Size Preview { set; get; }
        public WebCam_MF_Size Photo { set; get; }
        public WebCam_MF_Size Record { set; get; }
    }
    public sealed partial class WebCam_MF: IDisposable
    {
        WebCam_MF_Setting m_Setting = new WebCam_MF_Setting();
        public string FriendName { private set; get; } = "";
        public string SymbolLinkName { private set; get; } = "";
        public IComObject<IMFActivate>? CaptureObj { private set; get; }

        uint g_ResetToken = 0;
        IMFDXGIDeviceManager? g_pDXGIMan;
        HRESULT CreateD3DManager()
        {
            var hr = CreateDX11Device(out g_pDX11Device, out var pDX11DeviceContext, out var FeatureLevel);

            if (hr == HRESULTS.S_OK)
            {
                hr = DirectN.MFFunctions.MFCreateDXGIDeviceManager(out g_ResetToken, out g_pDXGIMan);
            }

            if (hr == HRESULTS.S_OK && g_pDXGIMan != null)
            {
                hr = g_pDXGIMan.ResetDevice(g_pDX11Device, g_ResetToken);
            }
            SafeRelease(pDX11DeviceContext);

            return hr;
        }

        ID3D11Device? g_pDX11Device;
        HRESULT CreateDX11Device(out ID3D11Device ppDevice, out ID3D11DeviceContext ppDeviceContext, out D3D_FEATURE_LEVEL pFeatureLevel)
        {
            D3D_FEATURE_LEVEL[] levels = new[]{
                D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_11_1,
                D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_11_0,
                D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_10_1,
                D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_10_0,
                D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_9_3,
                D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_9_2,
                D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_9_1
            };

            var hr = DirectN.D3D11Functions.D3D11CreateDevice(
                null,
                D3D_DRIVER_TYPE.D3D_DRIVER_TYPE_HARDWARE,
                IntPtr.Zero,
                (uint)DirectN.D3D11_CREATE_DEVICE_FLAG.D3D11_CREATE_DEVICE_VIDEO_SUPPORT,
                levels,
                (uint)levels.Length,
                7,
                out ppDevice,
                out pFeatureLevel,
                out ppDeviceContext
                );

            if (hr == HRESULTS.S_OK)
            {
                ID3D10Multithread? pMultithread;
                pMultithread = ppDevice as ID3D10Multithread;
                if (hr == HRESULTS.S_OK)
                {
                    pMultithread?.SetMultithreadProtected(true);
                }
            }

            return hr;
        }

        void DestroyCaptureEngine()
        {
            SafeRelease(m_VideoProcessor);
            SafeRelease(m_pEngine);

            g_pDXGIMan?.ResetDevice(g_pDX11Device, g_ResetToken);
            SafeRelease(g_pDX11Device);
            SafeRelease(g_pDXGIMan);
            m_IsPreviewing = false;
            
        }

        IMFCaptureEngine? m_pEngine;
        //IMFCapturePreviewSink? m_pPreview;
        //Dictionary<PreviewMediaType, List<IMFMediaType>> m_PreviewTypes = new();
        TaskCompletionSource<HRESULT>? m_TaskInitialize;
        async public Task<HRESULT?> InitCaptureEngine(WebCam_MF_Setting setting)
        {
            m_Setting.IsMirror = setting.IsMirror;
            m_Setting.Rotate = setting.Rotate;
            m_TaskInitialize = new TaskCompletionSource<HRESULT>();
            HRESULT? hr = HRESULTS.S_OK;
            IMFAttributes? pAttributes = null;
            IMFCaptureEngineClassFactory? pFactory = null;

            DestroyCaptureEngine();
            try
            {
                
                if (hr != HRESULTS.S_OK) return hr;
                hr = MFFunctions.MFCreateAttributes(out pAttributes, 1);
                if (hr != HRESULTS.S_OK) return hr;
                //if(used3d)
                //{
                //    hr = CreateD3DManager();
                //    if (hr != HRESULTS.S_OK) return hr;
                //}

                hr = pAttributes.SetUnknown(MFConstants.MF_CAPTURE_ENGINE_D3D_MANAGER, g_pDXGIMan);
                if (hr != HRESULTS.S_OK) return hr;
                var tty = Type.GetTypeFromCLSID(DirectN.MFConstants.CLSID_MFCaptureEngineClassFactory, true);
                if(tty == null) return hr;
                pFactory = Activator.CreateInstance(tty) as IMFCaptureEngineClassFactory;
                object? o = null;
                hr = pFactory?.CreateInstance(DirectN.MFConstants.CLSID_MFCaptureEngine, typeof(IMFCaptureEngine).GUID, out o);
                if (hr != HRESULTS.S_OK) return hr;
                m_pEngine = o as IMFCaptureEngine;
                System.Diagnostics.Trace.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff")}");
                var sw = System.Diagnostics.Stopwatch.StartNew();
                hr = m_pEngine?.Initialize(this, pAttributes, null, this.CaptureObj?.Object);
                if (hr != HRESULTS.S_OK) return hr;

                hr = await m_TaskInitialize.Task;
                sw.Stop();
                this.SupporCategory();
                InitIAMVideoProcAmp();
                InitFlashLight();
                InitTorch();
                InitFaceDection();
                //System.Diagnostics.Trace.WriteLine($"{sw.ElapsedMilliseconds}");
                
            }
            finally
            {
                m_TaskInitialize = null;
                SafeRelease(pAttributes);
                SafeRelease(pFactory);
            }
            return hr;
        }




        public static void SafeRelease<T>(T? obj) where T : class
        {
            if (obj != null && Marshal.IsComObject(obj))
            {
                Marshal.ReleaseComObject(obj);
            }
        }

        


        //TaskCompletionSource<HRESULT>? m_TaskSetMediaType;
        //public async Task<HRESULT> SetPreviewSize(Func<IEnumerable<PreviewMediaType>, PreviewMediaType?>? func)
        //{
        //    if (func == null) return HRESULTS.S_FALSE;
        //    m_TaskSetMediaType = new TaskCompletionSource<HRESULT>();
        //    var ff = func(m_PreviewTypes.Keys);
        //    if (ff == null) return HRESULTS.S_OK;
        //    if(m_PreviewTypes.ContainsKey(ff))
        //    {
        //        var mediatype = m_PreviewTypes[ff].FirstOrDefault();

        //        if (m_pEngine == null) return HRESULTS.MF_E_NOT_INITIALIZED;
        //        IMFCaptureSource? pSource;
        //        var hr = m_pEngine.GetSource(out pSource);
        //        if (hr != HRESULTS.S_OK) return hr;

        //        hr = pSource.SetCurrentDeviceMediaType((uint)MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM.FOR_VIDEO_PREVIEW, mediatype);
        //        if (hr != HRESULTS.S_OK) return hr;
        //        return await m_TaskSetMediaType.Task;
        //    }
        //    return HRESULTS.S_OK;
        //}

        //public HRESULT GetPreviewSize(out uint width, out uint height)
        //{
        //    width = 0; height = 0;
        //    if (m_pEngine == null) return HRESULTS.MF_E_NOT_INITIALIZED;
        //    IMFCaptureSource? pSource;
        //    IMFMediaType? pMediaType;
        //    var hr = m_pEngine.GetSource(out pSource);
        //    if (hr != HRESULTS.S_OK) return hr;


        //    //// Configure the video format for the preview sink.
        //    hr = pSource.GetCurrentDeviceMediaType((uint)MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM.FOR_VIDEO_PREVIEW, out pMediaType);
        //    if (hr != HRESULTS.S_OK) return hr;
        //    pMediaType.MFGetAttributeSize(MFConstants.MF_MT_FRAME_SIZE, out var w, out var h);
        //    var subtype = pMediaType.GetGuid(MFConstants.MF_MT_SUBTYPE);
        //    //pMediaType.GetUINT64(MFConstants.MF_MT_FRAME_SIZE, out var wh);
        //    //width = (int)(wh >> 32);
        //    //height = (int)(wh & 0xffffffff);
        //    width = w;
        //    height = h;
        //    return HRESULTS.S_OK;
        //}


        //async public Task<HRESULT> StartPreview(Func<IMFMediaType, IMFMediaType?>? transform,  IMFCaptureEngineOnSampleCallback samplecallback)
        //{
        //    if (m_pEngine == null) return HRESULTS.MF_E_NOT_INITIALIZED;

        //    m_TaskStartPreview = new TaskCompletionSource<HRESULT>();

        //    if (m_IsPreviewing) return HRESULTS.S_OK;

        //    IMFCaptureSink? pSink = null;
        //    IMFMediaType? pMediaType = null;
        //    IMFMediaType? pMediaType2 = null;
        //    IMFCaptureSource? pSource = null;
        //    int dwSinkStreamIndex = 0;
        //    HRESULT hr = HRESULTS.S_OK;
        //    try
        //    {
        //        // Get a pointer to the preview sink.
        //        if (m_pPreview == null)
        //        {
        //            hr = m_pEngine.GetSink(MF_CAPTURE_ENGINE_SINK_TYPE.MF_CAPTURE_ENGINE_SINK_TYPE_PREVIEW, out pSink);
        //            if (hr != HRESULTS.S_OK) return hr;
        //            m_pPreview = pSink as IMFCapturePreviewSink;
        //            if (m_pPreview == null) return HRESULTS.E_NOTIMPL;


        //            hr = m_pEngine.GetSource(out pSource);
        //            if (hr != HRESULTS.S_OK) return hr;


        //            //// Configure the video format for the preview sink.
        //            hr = pSource.GetCurrentDeviceMediaType((uint)MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM.FOR_VIDEO_PREVIEW, out pMediaType);
        //            if (hr != HRESULTS.S_OK) return hr;

        //            var dst = transform?.Invoke(pMediaType);
        //            if (dst==null)
        //            {
        //                dst = pMediaType;
        //            }
        //            //hr = CloneVideoMediaType(pMediaType, MFConstants.MFVideoFormat_RGB24, out pMediaType2);
        //            //if (hr != HRESULTS.S_OK || pMediaType2 == null) return hr;

        //            //hr = pMediaType2.SetUINT32(MFConstants.MF_MT_ALL_SAMPLES_INDEPENDENT, 1);
        //            //if (hr != HRESULTS.S_OK) return hr;

        //            // Connect the video stream to the preview sink.
        //            using var cm = new ComMemory(Marshal.SizeOf<uint>());
        //            hr = m_pPreview.AddStream((uint)MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM.FOR_VIDEO_PREVIEW, dst, null, cm.Pointer);
        //            if (hr != HRESULTS.S_OK) return hr;
        //            dwSinkStreamIndex = Marshal.ReadInt32(cm.Pointer);
        //        }
        //        if (samplecallback != null)
        //        {
        //            //uint w = 0;
        //            //uint h = 0;
        //            //MFFunctions1.MFGetAttributeSize(pMediaType2, MFConstants.MF_MT_FRAME_SIZE, out w, out h);
        //            //m_PreviewBmp = new WriteableBitmap((int)w, (int)h, 96, 96, PixelFormats.Bgr24, null);
        //            //m_PreviewCallback = new MFCaptureEngineOnSampleCallback(m_PreviewBmp);
        //            hr = m_pPreview.SetSampleCallback((uint)dwSinkStreamIndex, samplecallback);
        //            if (hr != HRESULTS.S_OK) return hr;
        //        }

        //        hr = m_pEngine.StartPreview();
        //        hr = await m_TaskStartPreview.Task;
        //        m_TaskStartPreview = null;
        //        m_IsPreviewing = true;
        //    }
        //    finally
        //    {
        //        SafeRelease(pSink);
        //        SafeRelease(pMediaType);
        //        SafeRelease(pMediaType2);
        //        SafeRelease(pSource);
        //    }


        //    return hr;

        //}

        ////async public Task<HRESULT> StartPreview(IMFCaptureEngineOnSampleCallback samplecallback)
        ////{
        ////    if (m_pEngine == null) return HRESULTS.MF_E_NOT_INITIALIZED;

        ////    m_TaskStartPreview = new TaskCompletionSource<HRESULT>();

        ////    if (m_IsPreviewing) return HRESULTS.S_OK;

        ////    IMFCaptureSink? pSink = null;
        ////    IMFMediaType? pMediaType = null;
        ////    IMFMediaType? pMediaType2 = null;
        ////    IMFCaptureSource? pSource = null;
        ////    int dwSinkStreamIndex = 0;
        ////    HRESULT hr = HRESULTS.S_OK;
        ////    try
        ////    {
        ////        // Get a pointer to the preview sink.
        ////        if (m_pPreview == null)
        ////        {
        ////            hr = m_pEngine.GetSink(MF_CAPTURE_ENGINE_SINK_TYPE.MF_CAPTURE_ENGINE_SINK_TYPE_PREVIEW, out pSink);
        ////            if (hr != HRESULTS.S_OK) return hr;
        ////            m_pPreview = pSink as IMFCapturePreviewSink;
        ////            if (m_pPreview == null) return HRESULTS.E_NOTIMPL;


        ////            hr = m_pEngine.GetSource(out pSource);
        ////            if (hr != HRESULTS.S_OK) return hr;


        ////            //// Configure the video format for the preview sink.
        ////            hr = pSource.GetCurrentDeviceMediaType((uint)MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM.FOR_VIDEO_PREVIEW, out pMediaType);
        ////            if (hr != HRESULTS.S_OK) return hr;

        ////            hr = CloneVideoMediaType(pMediaType, MFConstants.MFVideoFormat_RGB24, out pMediaType2);
        ////            if (hr != HRESULTS.S_OK || pMediaType2 == null) return hr;

        ////            hr = pMediaType2.SetUINT32(MFConstants.MF_MT_ALL_SAMPLES_INDEPENDENT, 1);
        ////            if (hr != HRESULTS.S_OK) return hr;

        ////            // Connect the video stream to the preview sink.
        ////            using var cm = new ComMemory(Marshal.SizeOf<uint>());
        ////            hr = m_pPreview.AddStream((uint)MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM.FOR_VIDEO_PREVIEW, pMediaType, null, cm.Pointer);
        ////            if (hr != HRESULTS.S_OK) return hr;
        ////            dwSinkStreamIndex = Marshal.ReadInt32(cm.Pointer);
        ////        }
        ////        if (samplecallback != null)
        ////        {
        ////            //uint w = 0;
        ////            //uint h = 0;
        ////            //MFFunctions1.MFGetAttributeSize(pMediaType2, MFConstants.MF_MT_FRAME_SIZE, out w, out h);
        ////            //m_PreviewBmp = new WriteableBitmap((int)w, (int)h, 96, 96, PixelFormats.Bgr24, null);
        ////            //m_PreviewCallback = new MFCaptureEngineOnSampleCallback(m_PreviewBmp);
        ////            hr = m_pPreview.SetSampleCallback((uint)dwSinkStreamIndex, samplecallback);
        ////            if (hr != HRESULTS.S_OK) return hr;
        ////        }

        ////        hr = m_pEngine.StartPreview();
        ////        hr = await m_TaskStartPreview.Task;
        ////        m_TaskStartPreview = null;
        ////        m_IsPreviewing = true;
        ////    }
        ////    finally
        ////    {
        ////        SafeRelease(pSink);
        ////        SafeRelease(pMediaType);
        ////        SafeRelease(pMediaType2);
        ////        SafeRelease(pSource);
        ////    }


        ////    return hr;
        ////}

        //async public Task<HRESULT> StartPreview1(IMFCaptureEngineOnSampleCallback samplecallback)
        //{
        //    if (m_pEngine == null) return HRESULTS.MF_E_NOT_INITIALIZED;

        //    m_TaskStartPreview = new TaskCompletionSource<HRESULT>();

        //    if (m_IsPreviewing) return HRESULTS.S_OK;

        //    IMFCaptureSink? pSink = null;
        //    IMFMediaType? pMediaType = null;
        //    IMFMediaType? pMediaType2 = null;
        //    IMFCaptureSource? pSource = null;
        //    int dwSinkStreamIndex = 0;
        //    HRESULT hr = HRESULTS.S_OK;
        //    try
        //    {
        //        // Get a pointer to the preview sink.
        //        if (m_pPreview == null)
        //        {
        //            hr = m_pEngine.GetSink(MF_CAPTURE_ENGINE_SINK_TYPE.MF_CAPTURE_ENGINE_SINK_TYPE_PREVIEW, out pSink);
        //            if (hr != HRESULTS.S_OK) return hr;
        //            m_pPreview = pSink as IMFCapturePreviewSink;
        //            if (m_pPreview == null) return HRESULTS.E_NOTIMPL;


        //            hr = m_pEngine.GetSource(out pSource);
        //            if (hr != HRESULTS.S_OK) return hr;


        //            //// Configure the video format for the preview sink.
        //            hr = pSource.GetCurrentDeviceMediaType((uint)MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM.FOR_VIDEO_PREVIEW, out pMediaType);
        //            if (hr != HRESULTS.S_OK) return hr;

        //            hr = CloneVideoMediaType(pMediaType, MFConstants.MFVideoFormat_RGB24, out pMediaType2);
        //            if (hr != HRESULTS.S_OK || pMediaType2 == null) return hr;

        //            hr = pMediaType2.SetUINT32(MFConstants.MF_MT_ALL_SAMPLES_INDEPENDENT, 1);
        //            if (hr != HRESULTS.S_OK) return hr;

        //            // Connect the video stream to the preview sink.
        //            using var cm = new ComMemory(Marshal.SizeOf<uint>());
        //            hr = m_pPreview.AddStream((uint)MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM.FOR_VIDEO_PREVIEW, pMediaType2, null, cm.Pointer);
        //            if (hr != HRESULTS.S_OK) return hr;
        //            dwSinkStreamIndex = Marshal.ReadInt32(cm.Pointer);
        //        }
        //        if (samplecallback != null)
        //        {
        //            //uint w = 0;
        //            //uint h = 0;
        //            //MFFunctions1.MFGetAttributeSize(pMediaType2, MFConstants.MF_MT_FRAME_SIZE, out w, out h);
        //            //m_PreviewBmp = new WriteableBitmap((int)w, (int)h, 96, 96, PixelFormats.Bgr24, null);
        //            //m_PreviewCallback = new MFCaptureEngineOnSampleCallback(m_PreviewBmp);
        //            hr = m_pPreview.SetSampleCallback((uint)dwSinkStreamIndex, samplecallback);
        //            if (hr != HRESULTS.S_OK) return hr;
        //        }

        //        hr = m_pEngine.StartPreview();
        //        hr = await m_TaskStartPreview.Task;
        //        m_TaskStartPreview = null;
        //        m_IsPreviewing = true;
        //    }
        //    finally
        //    {
        //        SafeRelease(pSink);
        //        SafeRelease(pMediaType);
        //        SafeRelease(pMediaType2);
        //        SafeRelease(pSource);
        //    }


        //    return hr;
        //}


        internal HRESULT CloneVideoMediaType(IMFMediaType pSrcMediaType, Guid guidSubType, out IMFMediaType? ppNewMediaType)
        {
            ppNewMediaType = null;
            var hr = MFFunctions.MFCreateMediaType(out var pNewMediaType);
            if (hr.IsError) return hr;
            hr = pNewMediaType.SetGUID(MFConstants.MF_MT_MAJOR_TYPE, MFConstants.MFMediaType_Video);
            if (hr.IsError) return hr;

            hr = pNewMediaType.SetGUID(MFConstants.MF_MT_SUBTYPE, guidSubType);
            if (hr.IsError) return hr;

            hr = CopyAttribute(pSrcMediaType, pNewMediaType, MFConstants.MF_MT_FRAME_SIZE);
            if (hr.IsError) return hr;

            hr = CopyAttribute(pSrcMediaType, pNewMediaType, MFConstants.MF_MT_FRAME_RATE);
            if (hr.IsError) return hr;

            hr = CopyAttribute(pSrcMediaType, pNewMediaType, MFConstants.MF_MT_PIXEL_ASPECT_RATIO);
            if (hr.IsError) return hr;

            hr = CopyAttribute(pSrcMediaType, pNewMediaType, MFConstants.MF_MT_INTERLACE_MODE);
            if (hr.IsError) return hr;

            ppNewMediaType = pNewMediaType;
            return hr;
        }

        HRESULT CopyAttribute(IMFAttributes pSrc, IMFAttributes pDest, Guid key)
        {
            PROPVARIANT var = new();
            HRESULT hr = pSrc.GetItem(key, var);
            if (hr == HRESULTS.S_OK)
            {
                hr = pDest.SetItem(key, var);
            }
            return hr;
        }



        public void UpdateVideo(int width, int height)
        {
            if (m_pEngine == null) return;
            var hr = m_pEngine.GetSink(MF_CAPTURE_ENGINE_SINK_TYPE.MF_CAPTURE_ENGINE_SINK_TYPE_PREVIEW, out var pSink);
            if (hr != HRESULTS.S_OK) return;
            var previewsink = pSink as IMFCapturePreviewSink;
            IntPtr prc = Marshal.AllocHGlobal(Marshal.SizeOf<RECT>());
            RECT rc;
            rc.left = 0;
            rc.top = 0;
            rc.right = width;
            rc.bottom = height;

            Marshal.StructureToPtr(rc, prc, true);
            if(previewsink != null)
            {
                previewsink.UpdateVideo(IntPtr.Zero, prc, IntPtr.Zero);
            }
            SafeRelease(previewsink);
            Marshal.FreeHGlobal(prc);

        }

        public void Dispose()
        {
            this.DestroyCaptureEngine();
        }
    }

    struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    };

    public enum MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM : uint
    {
        FOR_VIDEO_PREVIEW = 0xfffffffa,
        FOR_VIDEO_RECORD = 0xfffffff9,
        FOR_PHOTO = 0xfffffff8,
        FOR_AUDIO = 0xfffffff7,
        FOR_METADATA = 0xfffffff6,
        MF_CAPTURE_ENGINE_MEDIASOURCE = 0xffffffff
    }
}

