using DirectN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace QSoft.MediaCapture
{
    public enum MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM : uint
    {
        MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM_FOR_VIDEO_PREVIEW = 0xfffffffa,
        MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM_FOR_VIDEO_RECORD = 0xfffffff9,
        MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM_FOR_PHOTO = 0xfffffff8,
        MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM_FOR_AUDIO = 0xfffffff7,
        MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM_FOR_METADATA = 0xfffffff6,
        MF_CAPTURE_ENGINE_MEDIASOURCE = 0xffffffff
    }
    public class WebCam_MF: IMFCaptureEngineOnEventCallback
    {

        uint g_ResetToken = 0;
        IMFDXGIDeviceManager g_pDXGIMan;
        HRESULT CreateD3DManager()
        {
            HRESULT hr = DirectN.HRESULTS.S_OK;
            D3D_FEATURE_LEVEL FeatureLevel;
            ID3D11DeviceContext pDX11DeviceContext;
            hr = CreateDX11Device(out g_pDX11Device, out pDX11DeviceContext, out FeatureLevel);

            if (hr == HRESULTS.S_OK)
            {
                //g_pDXGIMan = DirectN.MFFunctions.MFCreateDXGIDeviceManager(out g_ResetToken);
                hr = DirectN.MFFunctions.MFCreateDXGIDeviceManager(out g_ResetToken, out g_pDXGIMan);
                //hr = MFCreateDXGIDeviceManager(&g_ResetToken, &g_pDXGIMan);
            }

            if (hr == HRESULTS.S_OK)
            {
                hr = g_pDXGIMan.ResetDevice(g_pDX11Device, g_ResetToken);
            }
            Marshal.ReleaseComObject(pDX11DeviceContext);

            return hr;
        }

        ID3D11Device g_pDX11Device;
        ID3D11DeviceContext m_pDeviceContext;
        HRESULT CreateDX11Device(out ID3D11Device ppDevice, out ID3D11DeviceContext ppDeviceContext, out D3D_FEATURE_LEVEL pFeatureLevel)
        {
            HRESULT hr = HRESULTS.S_OK;


            D3D_FEATURE_LEVEL[] levels = new[]{
                D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_11_1,
                D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_11_0,
                D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_10_1,
                D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_10_0,
                D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_9_3,
                D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_9_2,
                D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_9_1
            };

            hr = DirectN.D3D11Functions.D3D11CreateDevice(
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
                ID3D10Multithread pMultithread;
                pMultithread = ppDevice as ID3D10Multithread;
                //hr = ppDevice.QueryInterface(IID_PPV_ARGS(&pMultithread)));
                if (hr == HRESULTS.S_OK)
                {
                    var bb = pMultithread.SetMultithreadProtected(true);
                }
            }

            return hr;
        }
        IntPtr m_hwndPreview;
        public HRESULT InitializeCaptureManager(IntPtr hwndPreview, object videosource)
        {
            HRESULT hr = HRESULTS.S_OK;
            IMFAttributes pAttributes;
            IMFCaptureEngineClassFactory pFactory;
            
            DestroyCaptureEngine();

            //m_hEvent = CreateEvent(NULL, FALSE, FALSE, NULL);
            //if (NULL == m_hEvent)
            //{
            //    hr = HRESULT_FROM_WIN32(GetLastError());
            //    goto Exit;
            //}

            //m_pCallback = new(std::nothrow) CaptureEngineCB(m_hwndEvent);
            //if (m_pCallback == NULL)
            //{
            //    hr = E_OUTOFMEMORY;
            //    goto Exit;
            //}

            //m_pCallback->m_pManager = this;
            m_hwndPreview = hwndPreview;

            //Create a D3D Manager
            hr = CreateD3DManager();
            if (hr != HRESULTS.S_OK)
            {
                goto Exit;
            }
            hr = MFFunctions.MFCreateAttributes(out pAttributes, 1);
            if (hr != HRESULTS.S_OK)
            {
                goto Exit;
            }
            hr = pAttributes.SetUnknown(MFConstants.MF_CAPTURE_ENGINE_D3D_MANAGER, g_pDXGIMan);
            if (hr != HRESULTS.S_OK)
            {
                goto Exit;
            }

            pFactory = Activator.CreateInstance(Type.GetTypeFromCLSID(DirectN.MFConstants.CLSID_MFCaptureEngineClassFactory)) as IMFCaptureEngineClassFactory;
            object o;
            pFactory.CreateInstance(DirectN.MFConstants.CLSID_MFCaptureEngine, typeof(IMFCaptureEngine).GUID, out o);
            m_pEngine = o as IMFCaptureEngine;
            hr = m_pEngine.Initialize(this, pAttributes, null, videosource);
            if (hr != HRESULTS.S_OK)
            {
                goto Exit;
            }
        //// Create the factory object for the capture engine.
        //hr = CoCreateInstance(CLSID_MFCaptureEngineClassFactory, NULL,
        //    CLSCTX_INPROC_SERVER, IID_PPV_ARGS(&pFactory));
        //if (hr != HRESULTS.S_OK)
        //{
        //    goto Exit;
        //}

        //// Create and initialize the capture engine.
        //hr = pFactory->CreateInstance(CLSID_MFCaptureEngine, IID_PPV_ARGS(&m_pEngine));
        //if (hr != HRESULTS.S_OK)
        //{
        //    goto Exit;
        //}
        //hr = m_pEngine->Initialize(m_pCallback, pAttributes, NULL, pUnk);
        //if (hr != HRESULTS.S_OK)
        //{
        //    goto Exit;
        //}

        Exit:
            //if (NULL != pAttributes)
            //{
            //    pAttributes->Release();
            //    pAttributes = NULL;
            //}
            //if (NULL != pFactory)
            //{
            //    pFactory->Release();
            //    pFactory = NULL;
            //}
            return hr;
        }

        void DestroyCaptureEngine()
        {
            //if (NULL != m_hEvent)
            //{
            //    CloseHandle(m_hEvent);
            //    m_hEvent = NULL;
            //}
            //SafeRelease(&m_pPreview);
            //SafeRelease(&m_pEngine);
            //SafeRelease(&m_pCallback);

            //if (g_pDXGIMan)
            //{
            //    g_pDXGIMan->ResetDevice(g_pDX11Device, g_ResetToken);
            //}
            //SafeRelease(&g_pDX11Device);
            //SafeRelease(&g_pDXGIMan);

            //m_bPreviewing = false;
            //m_bRecording = false;
            //m_bPhotoPending = false;
            //m_errorID = 0;
        }

        IMFCaptureEngine m_pEngine;
        IMFCapturePreviewSink m_pPreview;

        //CaptureEngineCB* m_pCallback;

        bool m_bPreviewing;
        bool m_bRecording;
        bool m_bPhotoPending;

        uint m_errorID;
        //HANDLE m_hEvent;
        //HANDLE m_hpwrRequest;
        bool m_fPowerRequestSet;

        public HRESULT StartPreview()
        {
            if (m_pEngine == null)
            {
                return HRESULTS.MF_E_NOT_INITIALIZED;
            }

            if (m_bPreviewing == true)
            {
                return HRESULTS.S_OK;
            }

            IMFCaptureSink pSink = null;
            IMFMediaType pMediaType = null;
            IMFMediaType pMediaType2 = null;
            IMFCaptureSource pSource = null;
            IntPtr dwSinkStreamIndex = IntPtr.Zero;
            HRESULT hr = HRESULTS.S_OK;
            // Get a pointer to the preview sink.
            if (m_pPreview == null)
            {
                hr = m_pEngine.GetSink(MF_CAPTURE_ENGINE_SINK_TYPE.MF_CAPTURE_ENGINE_SINK_TYPE_PREVIEW, out pSink);
                if (hr != HRESULTS.S_OK)
                {
                    goto done;
                }
                m_pPreview = pSink as IMFCapturePreviewSink;
                //hr = pSink.QueryInterface(IID_PPV_ARGS(&m_pPreview));
                ////if (hr != HRESULTS.S_OK)
                ////{
                ////    goto done;
                ////}

                hr = m_pPreview.SetRenderHandle(m_hwndPreview);
                if (hr != HRESULTS.S_OK)
                {
                    goto done;
                }

                hr = m_pEngine.GetSource(out pSource);
                if (hr != HRESULTS.S_OK)
                {
                    goto done;
                }

                //// Configure the video format for the preview sink.
                ////MF_CAPTURE_ENGINE_FIRST_SOURCE_VIDEO_STREAM
                ////MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM_FOR_VIDEO_PREVIEW

                //MF



                hr = pSource.GetCurrentDeviceMediaType((uint)MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM.MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM_FOR_VIDEO_PREVIEW, out pMediaType);
                if (hr != HRESULTS.S_OK)
                {
                    goto done;
                }

                hr = CloneVideoMediaType(pMediaType,MFConstants.MFVideoFormat_RGB32, out pMediaType2);
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
                hr = m_pPreview.AddStream((uint)MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM.MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM_FOR_VIDEO_PREVIEW, pMediaType2, null, dwSinkStreamIndex);
                if (hr != HRESULTS.S_OK)
                {
                    goto done;
                }
            }


            hr = m_pEngine.StartPreview();
            //if (!m_fPowerRequestSet && m_hpwrRequest != INVALID_HANDLE_VALUE)
            //{
            //    // NOTE:  By calling this, on SOC systems (AOAC enabled), we're asking the system to not go
            //    // into sleep/connected standby while we're streaming.  However, since we don't want to block
            //    // the device from ever entering connected standby/sleep, we're going to latch ourselves to
            //    // the monitor on/off notification (RegisterPowerSettingNotification(GUID_MONITOR_POWER_ON)).
            //    // On SOC systems, this notification will fire when the user decides to put the device in
            //    // connected standby mode--we can trap this, turn off our media streams and clear this
            //    // power set request to allow the device to go into the lower power state.
            //    m_fPowerRequestSet = (TRUE == PowerSetRequest(m_hpwrRequest, PowerRequestExecutionRequired));
            //}
        done:
            Marshal.ReleaseComObject(pSink);
            Marshal.ReleaseComObject(pMediaType);
            Marshal.ReleaseComObject(pMediaType2);
            Marshal.ReleaseComObject(pSource);


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
                //PropVariantInit( &var );
                HRESULT hr = pSrc.GetItem(key, var);
            if (hr == HRESULTS.S_OK)
            {
                hr = pDest.SetItem(key, var);
                //PropVariantClear(&var);
            }
            return hr;
        }

public HRESULT OnEvent(IMFMediaEvent pEvent)
        {
            // We're about to fall asleep, that means we've just asked the CE to stop the preview
            // and record.  We need to handle it here since our message pump may be gone.
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
                if(guidType == MFConstants.MF_CAPTURE_ENGINE_INITIALIZED)
                {

                }
                else if (guidType == MFConstants.MF_CAPTURE_ENGINE_PREVIEW_STOPPED)
                {
                    //m_pManager->OnPreviewStopped(hrStatus);
                    //SetEvent(m_pManager->m_hEvent);
                }
                else if (guidType == MFConstants.MF_CAPTURE_ENGINE_RECORD_STARTED)
                {
                    //m_pManager->OnRecordStopped(hrStatus);
                    //SetEvent(m_pManager->m_hEvent);
                }
                else
                {
                    // This is an event we don't know about, we don't really care and there's
                    // no clean way to report the error so just set the event and fall through.
                    //SetEvent(m_pManager->m_hEvent);
                }
            }

            return HRESULTS.S_OK;
        }

        
    }

    
}
