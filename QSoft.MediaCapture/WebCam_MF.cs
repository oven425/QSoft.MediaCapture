using DirectN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace QSoft.MediaCapture
{
    public sealed class WebCam_MF: IMFCaptureEngineOnEventCallback,IDisposable
    {
        public HRESULT OnEvent(IMFMediaEvent pEvent)
        {
            throw new NotImplementedException();
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
            HRESULT hr = DirectN.HRESULTS.S_OK;
            D3D_FEATURE_LEVEL FeatureLevel;
            ID3D11DeviceContext pDX11DeviceContext;
            hr = CreateDX11Device(out g_pDX11Device, out pDX11DeviceContext, out FeatureLevel);

            if (hr == HRESULTS.S_OK)
            {
                hr = DirectN.MFFunctions.MFCreateDXGIDeviceManager(out g_ResetToken, out g_pDXGIMan);
            }

            if (hr == HRESULTS.S_OK && g_pDXGIMan != null)
            {
                hr = g_pDXGIMan.ResetDevice(g_pDX11Device, g_ResetToken);
            }
            Marshal.ReleaseComObject(pDX11DeviceContext);

            return hr;
        }

        ID3D11Device? g_pDX11Device;
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
            //if (NULL != m_hEvent)
            //{
            //    CloseHandle(m_hEvent);
            //    m_hEvent = NULL;
            //}
            SafeRelease(m_pPreview);
            SafeRelease(m_pEngine);
            //SafeRelease(m_pCallback);

            if (g_pDXGIMan != null)
            {
                g_pDXGIMan.ResetDevice(g_pDX11Device, g_ResetToken);
            }
            SafeRelease(g_pDX11Device);
            SafeRelease(g_pDXGIMan);

            //m_bPreviewing = false;
            //m_bRecording = false;
            //m_bPhotoPending = false;
            //m_errorID = 0;
        }

        IMFCaptureEngine? m_pEngine;
        IMFCapturePreviewSink? m_pPreview;

        TaskCompletionSource<HRESULT>? m_Tasknitialize;
        async public Task<HRESULT?> InitCaptureEngine()
        {
            m_Tasknitialize = new TaskCompletionSource<HRESULT>();
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
                    throw new MFException();
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

                hr = await m_Tasknitialize.Task;
            }
            catch(Exception ee)
            {

            }
            finally
            {
                SafeRelease(pAttributes);
                SafeRelease(pFactory);
            }
        //    //Create a D3D Manager
        //    hr = CreateD3DManager();
        //    if (hr != HRESULTS.S_OK)
        //    {
        //        goto Exit;
        //    }
        //    hr = MFFunctions.MFCreateAttributes(out pAttributes, 1);
        //    if (hr != HRESULTS.S_OK)
        //    {
        //        goto Exit;
        //    }
        //    hr = pAttributes.SetUnknown(MFConstants.MF_CAPTURE_ENGINE_D3D_MANAGER, g_pDXGIMan);
        //    if (hr != HRESULTS.S_OK)
        //    {
        //        goto Exit;
        //    }

        //    pFactory = Activator.CreateInstance(Type.GetTypeFromCLSID(DirectN.MFConstants.CLSID_MFCaptureEngineClassFactory)) as IMFCaptureEngineClassFactory;
        //    object? o = null;
        //    pFactory?.CreateInstance(DirectN.MFConstants.CLSID_MFCaptureEngine, typeof(IMFCaptureEngine).GUID, out o);
        //    m_pEngine = o as IMFCaptureEngine;
        //    hr = m_pEngine?.Initialize(this, pAttributes, null, this.CaptureObj?.Object);
        //    if (hr != HRESULTS.S_OK)
        //    {
        //        goto Exit;
        //    }
        //    hr = m_pEngine?.GetSource(out var pSource);
        //    if (hr != HRESULTS.S_OK)
        //    {
        //        goto Exit;
        //    }
        //    hr = await m_Tasknitialize.Task;

        //Exit:
        //    SafeRelease(pAttributes);
        //    SafeRelease(pFactory);
            return hr;
        }

        void SafeRelease<T>(T? obj) where T : class
        {
            if (obj != null && Marshal.IsComObject(obj))
            {
                Marshal.ReleaseComObject(obj);
            }
        }

        public void StartPreview()
        {

        }

        public void StopPreview()
        { 

        }




        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

    public class MFException : Exception
    {

    }
}
