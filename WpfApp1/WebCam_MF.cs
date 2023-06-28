using DirectN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace QSoft.MediaCapture
{
    public class WebCam_MF: IMFCaptureEngineOnEventCallback
    {

        uint g_ResetToken = 0;
        ComObject<IMFDXGIDeviceManager> g_pDXGIMan;
        HRESULT CreateD3DManager()
        {
            HRESULT hr = DirectN.HRESULTS.S_OK;
            D3D_FEATURE_LEVEL FeatureLevel;
            ID3D11DeviceContext pDX11DeviceContext;
            hr = CreateDX11Device(out g_pDX11Device, out pDX11DeviceContext, out FeatureLevel);

            if (hr == HRESULTS.S_OK)
            {
                g_pDXGIMan = DirectN.MFFunctions.MFCreateDXGIDeviceManager(out g_ResetToken);
                //hr = MFCreateDXGIDeviceManager(&g_ResetToken, &g_pDXGIMan);
            }

            if (hr == HRESULTS.S_OK)
            {
                hr = g_pDXGIMan.Object.ResetDevice(g_pDX11Device, g_ResetToken);
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
                Marshal.ReleaseComObject(pMultithread);

            }

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
                
                if (guidType == MFConstants.MF_CAPTURE_ENGINE_PREVIEW_STOPPED)
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
