using DirectN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace QSoft.MediaCapture
{
    public sealed class WebCam_MF: IMFCaptureEngineOnEventCallback,IDisposable
    {
        public HRESULT OnEvent(IMFMediaEvent pEvent)
        {
            Guid guidType;
            HRESULT hrStatus;
            HRESULT hr = pEvent.GetStatus(out hrStatus);
            if (hr != HRESULTS.S_OK)
            {
                hrStatus = hr;
            }

            hr = pEvent.GetExtendedType(out guidType);
            if (hr == HRESULTS.S_OK)
            {
                if (guidType == MFConstants.MF_CAPTURE_ENGINE_INITIALIZED)
                {
                    m_TaskInitialize?.SetResult(hrStatus);
                }
                else if (guidType == MFConstants.MF_CAPTURE_ENGINE_PREVIEW_STARTED)
                {
                    m_TaskStartPreview?.SetResult(hrStatus);
                }
                else if (guidType == MFConstants.MF_CAPTURE_ENGINE_PREVIEW_STOPPED)
                {
                    m_TaskStopPreview?.SetResult(hrStatus);
                }
                else if (guidType == MFConstants.MF_CAPTURE_ENGINE_RECORD_STARTED)
                {
                   // m_TaskStartRecord.SetResult(hrStatus);
                    //m_pManager->OnRecordStopped(hrStatus);
                    //SetEvent(m_pManager->m_hEvent);
                }
                else if (guidType == MFConstants.MF_CAPTURE_ENGINE_RECORD_STOPPED)
                {
                    //m_TaskStopRecord.SetResult(hrStatus);
                }
                else if (guidType == MFConstants.MF_CAPTURE_ENGINE_PHOTO_TAKEN)
                {
                    //this.m_TakeTakephoto.SetResult(hrStatus);
                }
                else if (guidType == MFConstants.MF_CAPTURE_ENGINE_EFFECT_ADDED)
                {

                }
                else if (guidType == MFConstants.MF_CAPTURE_SOURCE_CURRENT_DEVICE_MEDIA_TYPE_SET)
                {
                    //this.m_TaskSetMediaType?.SetResult(hrStatus);
                }
                else if (guidType == MFConstants.MF_CAPTURE_ENGINE_ERROR)
                {
                    var aa = Marshal.GetExceptionForHR(hrStatus.Value).Message;
                    //if (OnFail != null)
                    //{

                    //    OnFail(this, hrStatus.Value);
                    //}
                }
                else
                {
                    System.Diagnostics.Trace.WriteLine(guidType);
                    // This is an event we don't know about, we don't really care and there's
                    // no clean way to report the error so just set the event and fall through.
                    //SetEvent(m_pManager->m_hEvent);
                }
            }
            return HRESULTS.S_OK;
        }

        public string FriendName { private set; get; } = "";
        public string SymbolLinkName { private set; get; } = "";
        public IComObject<IMFActivate>? CaptureObj { private set; get; }

        public static List<WebCam_MF> GetAllWebCams()
        {
            var attr = MFFunctions.MFCreateAttributes();
            attr.Set(MFConstants.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE, MFConstants.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_GUID);
            return attr.EnumDeviceSources().Select(x =>
            {
                var mf = new WebCam_MF();
                mf.FriendName = x.GetString(MFConstants.MF_DEVSOURCE_ATTRIBUTE_FRIENDLY_NAME);
                mf.SymbolLinkName = x.GetString(MFConstants.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_SYMBOLIC_LINK);
                mf.CaptureObj = x;
                return mf;
            }).ToList();
        }

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
            SafeRelease(m_pPreview);
            SafeRelease(m_pEngine);

            if (g_pDXGIMan != null)
            {
                g_pDXGIMan.ResetDevice(g_pDX11Device, g_ResetToken);
            }
            SafeRelease(g_pDX11Device);
            SafeRelease(g_pDXGIMan);
            m_IsPreviewing = false;
            //m_bRecording = false;
            //m_bPhotoPending = false;
            //m_errorID = 0;
        }

        IMFCaptureEngine? m_pEngine;
        IMFCapturePreviewSink? m_pPreview;

        TaskCompletionSource<HRESULT>? m_TaskInitialize;
        async public Task InitCaptureEngine()
        {
            m_TaskInitialize = new TaskCompletionSource<HRESULT>();
            HRESULT? hr = HRESULTS.S_OK;
            IMFAttributes? pAttributes = null;
            IMFCaptureEngineClassFactory? pFactory = null;

            DestroyCaptureEngine();
            try
            {
                //Create a D3D Manager
                hr = CreateD3DManager();
                if (hr != HRESULTS.S_OK)
                {
                    throw Marshal.GetExceptionForHR((int)hr);
                }
                hr = MFFunctions.MFCreateAttributes(out pAttributes, 1);
                if (hr != HRESULTS.S_OK)
                {
                    throw new MFException();
                }
                hr = pAttributes.SetUnknown(MFConstants.MF_CAPTURE_ENGINE_D3D_MANAGER, g_pDXGIMan);
                if (hr != HRESULTS.S_OK)
                {
                    throw new MFException();
                }

                pFactory = Activator.CreateInstance(Type.GetTypeFromCLSID(DirectN.MFConstants.CLSID_MFCaptureEngineClassFactory)) as IMFCaptureEngineClassFactory;
                object? o = null;
                pFactory?.CreateInstance(DirectN.MFConstants.CLSID_MFCaptureEngine, typeof(IMFCaptureEngine).GUID, out o);
                m_pEngine = o as IMFCaptureEngine;
                hr = m_pEngine?.Initialize(this, pAttributes, null, this.CaptureObj?.Object);
                if (hr != HRESULTS.S_OK)
                {
                    throw new MFException();
                }

                hr = await m_TaskInitialize.Task;
            }
            finally
            {
                SafeRelease(pAttributes);
                SafeRelease(pFactory);
            }
            //return hr;
        }

        void SafeRelease<T>(T? obj) where T : class
        {
            if (obj != null && Marshal.IsComObject(obj))
            {
                Marshal.ReleaseComObject(obj);
            }
        }

        TaskCompletionSource<HRESULT>? m_TaskStopPreview;
        async public Task<HRESULT?> StopPreview()
        {
            HRESULT hr = HRESULTS.S_OK;

            if (m_pEngine == null)
            {
                
                return HRESULTS.MF_E_NOT_INITIALIZED;
            }
            if (!m_IsPreviewing)
            {
                return HRESULTS.S_OK;
            }
            if(m_TaskStopPreview == null)
            {
                this.m_TaskStopPreview = new TaskCompletionSource<HRESULT>(hr);
            }
            
            hr = m_pEngine.StopPreview();
            if (hr.IsError)
            {
                goto done;
            }
        done:
            hr = await this.m_TaskStopPreview.Task;
            m_IsPreviewing = false;
            return hr;
        }

        bool m_IsPreviewing = false;
        TaskCompletionSource<HRESULT>? m_TaskStartPreview;
        async public Task<HRESULT?> StartPreview(IntPtr handle)
        {
            if (m_pEngine == null)
            {
                return HRESULTS.MF_E_NOT_INITIALIZED;
            }

            if(m_TaskStartPreview == null)
            {
                m_TaskStartPreview = new TaskCompletionSource<HRESULT>();
            }

            if (m_IsPreviewing)
            {
                return HRESULTS.S_OK;
            }

            IMFCaptureSink? pSink = null;
            IMFMediaType? pMediaType = null;
            IMFMediaType? pMediaType2 = null;
            IMFCaptureSource? pSource = null;

            HRESULT? hr = HRESULTS.S_OK;
            // Get a pointer to the preview sink.
            if (m_pPreview == null)
            {
                hr = m_pEngine?.GetSink(MF_CAPTURE_ENGINE_SINK_TYPE.MF_CAPTURE_ENGINE_SINK_TYPE_PREVIEW, out pSink);
                if (hr != HRESULTS.S_OK)
                {
                    goto done;
                }
                m_pPreview = pSink as IMFCapturePreviewSink;

                if (handle != IntPtr.Zero)
                {
                    hr = m_pPreview?.SetRenderHandle(handle);
                    if (hr != HRESULTS.S_OK)
                    {
                        goto done;
                    }
                }

                hr = m_pEngine.GetSource(out pSource);
                if (hr != HRESULTS.S_OK)
                {
                    goto done;
                }


                //// Configure the video format for the preview sink.
                hr = pSource.GetCurrentDeviceMediaType((uint)MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM.FOR_VIDEO_PREVIEW, out pMediaType);
                if (hr != HRESULTS.S_OK)
                {
                    goto done;
                }

                hr = CloneVideoMediaType(pMediaType, MFConstants.MFVideoFormat_RGB24, out pMediaType2);
                if (hr != HRESULTS.S_OK)
                {
                    goto done;
                }

                hr = pMediaType2.SetUINT32(MFConstants.MF_MT_ALL_SAMPLES_INDEPENDENT, 1);
                if (hr != HRESULTS.S_OK)
                {
                    goto done;
                }

                // Connect the video stream to the preview sink.
                using (var cm = new ComMemory(Marshal.SizeOf<uint>()))
                {
                    hr = m_pPreview?.AddStream((uint)MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM.FOR_VIDEO_PREVIEW, pMediaType2, null, cm.Pointer);
                    if (hr != HRESULTS.S_OK)
                    {
                        goto done;
                    }
                }

            }


            hr = m_pEngine?.StartPreview();
            hr = await m_TaskStartPreview.Task;
            m_IsPreviewing = true;

        done:
            SafeRelease(pSink);
            SafeRelease(pMediaType);
            SafeRelease(pMediaType2);
            SafeRelease(pSource);

            return hr;
        }

        HRESULT CloneVideoMediaType(IMFMediaType pSrcMediaType, Guid guidSubType, out IMFMediaType ppNewMediaType)
        {
            IMFMediaType pNewMediaType = null;
            ppNewMediaType = null;
            HRESULT hr = MFFunctions.MFCreateMediaType(out pNewMediaType);
            if (hr.IsError)
            {
                goto done;
            }

            hr = pNewMediaType.SetGUID(MFConstants.MF_MT_MAJOR_TYPE, MFConstants.MFMediaType_Video);
            if (hr.IsError)
            {
                goto done;
            }

            hr = pNewMediaType.SetGUID(MFConstants.MF_MT_SUBTYPE, guidSubType);
            if (hr.IsError)
            {
                goto done;
            }

            hr = CopyAttribute(pSrcMediaType, pNewMediaType, MFConstants.MF_MT_FRAME_SIZE);
            if (hr.IsError)
            {
                goto done;
            }

            hr = CopyAttribute(pSrcMediaType, pNewMediaType, MFConstants.MF_MT_FRAME_RATE);
            if (hr.IsError)
            {
                goto done;
            }

            hr = CopyAttribute(pSrcMediaType, pNewMediaType, MFConstants.MF_MT_PIXEL_ASPECT_RATIO);
            if (hr.IsError)
            {
                goto done;
            }

            hr = CopyAttribute(pSrcMediaType, pNewMediaType, MFConstants.MF_MT_INTERLACE_MODE);
            if (hr.IsError)
            {
                goto done;
            }

            ppNewMediaType = pNewMediaType;
        //(ppNewMediaType).AddRef();

        done:
            //SafeRelease(&pNewMediaType);
            return hr;
        }

        HRESULT CopyAttribute(IMFAttributes pSrc, IMFAttributes pDest, Guid key)
        {
            PropVariant var = new PropVariant();
            HRESULT hr = pSrc.GetItem(key, var);
            if (hr == HRESULTS.S_OK)
            {
                hr = pDest.SetItem(key, var);
            }
            return hr;
        }



        public HRESULT? UpdateVideo()
        {
            if (m_IsPreviewing)
            {
                return m_pPreview?.UpdateVideo(IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
            }
            else
            {
                return HRESULTS.S_OK;
            }
        }

        public void Dispose()
        {
            this.DestroyCaptureEngine();
        }
    }

    public enum MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM : uint
    {
        FOR_VIDEO_PREVIEW = 0xfffffffa,
        FOR_VIDEO_RECORD = 0xfffffff9,
        FOR_PHOTO = 0xfffffff8,
        FOR_AUDIO = 0xfffffff7,
        FOR_METADATA = 0xfffffff6,
        MF_CAPTURE_ENGINE_MEDIASOURCE = 0xffffffff
    }

    public class MFException : Exception
    {

    }
}
