using DirectN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

//https://learn.microsoft.com/zh-tw/windows/win32/api/mfidl/nn-mfidl-imfcameraconfigurationmanager

namespace QSoft.MediaCapture
{
    //public class WebCam_MF_Size
    //{
    //    public uint Width { set; get; }
    //    public uint Height { set; get; }
    //}
    public class WebCam_MF_Setting
    {
        public bool Shared { set; get; }
        public bool IsMirror { set; get; }
        public uint Rotate { set; get; }
        //public WebCam_MF_Size Preview { set; get; } = new WebCam_MF_Size();
        //public WebCam_MF_Size Photo { set; get; } = new WebCam_MF_Size();
        //public WebCam_MF_Size Record { set; get; } = new WebCam_MF_Size();
    }
    public sealed partial class WebCam_MF: IDisposable
    {
        WebCam_MF_Setting m_Setting = new WebCam_MF_Setting();
        public string FriendName { private set; get; } = "";
        public string SymbolLinkName { private set; get; } = "";
        public IComObject<IMFActivate>? CaptureObj { private set; get; }


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
            m_Setting.Shared = setting.Shared;
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
                hr = CreateD3DManager();
                if (hr != HRESULTS.S_OK) return hr;
                //}

                hr = pAttributes.SetUnknown(MFConstants.MF_CAPTURE_ENGINE_D3D_MANAGER, g_pDXGIMan);
                if(m_Setting.Shared)
                {
                    pAttributes.SetUINT32(MFConstants.MF_DEVSOURCE_ATTRIBUTE_FRAMESERVER_SHARE_MODE, 1);
                }
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
                //InitIAMVideoProcAmp();
                //InitFlashLight();
                //InitTorch();
                //InitFaceDection();

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

        




        internal static HRESULT CloneVideoMediaType(IMFMediaType pSrcMediaType, Guid guidSubType, out IMFMediaType? ppNewMediaType)
        {
            ppNewMediaType = null;
            var hr = MFFunctions.MFCreateMediaType(out var pNewMediaType);
            if (hr.IsError) return hr;
            hr = pNewMediaType.SetGUID(MFConstants.MF_MT_MAJOR_TYPE, MFConstants.MFMediaType_Video);
            if (hr.IsError) return hr;

            hr = pNewMediaType.SetGUID(MFConstants.MF_MT_SUBTYPE, guidSubType);
            if (hr.IsError) return hr;

            hr = WebCam_MF.CopyAttribute(pSrcMediaType, pNewMediaType, MFConstants.MF_MT_FRAME_SIZE);
            if (hr.IsError) return hr;

            hr = WebCam_MF.CopyAttribute(pSrcMediaType, pNewMediaType, MFConstants.MF_MT_FRAME_RATE);
            if (hr.IsError) return hr;

            hr = WebCam_MF.CopyAttribute(pSrcMediaType, pNewMediaType, MFConstants.MF_MT_PIXEL_ASPECT_RATIO);
            if (hr.IsError) return hr;

            hr = WebCam_MF.CopyAttribute(pSrcMediaType, pNewMediaType, MFConstants.MF_MT_INTERLACE_MODE);
            if (hr.IsError) return hr;

            ppNewMediaType = pNewMediaType;
            return hr;
        }

        static HRESULT CopyAttribute(IMFAttributes pSrc, IMFAttributes pDest, Guid key)
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

