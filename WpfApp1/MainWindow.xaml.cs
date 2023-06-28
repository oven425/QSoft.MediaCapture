using DirectN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var attribute = MFFunctions.MFCreateAttributes();
            //hr = pAttributes->SetGUID(MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE,
            //    MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_GUID);
            attribute.Set(MFConstants.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE, MFConstants.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_GUID);
            var sources = attribute.EnumDeviceSources().Select(x=>x.GetString(MFConstants.MF_DEVSOURCE_ATTRIBUTE_FRIENDLY_NAME));

            //MFFunctions.MFStartup();
            //var capturefac = Activator.CreateInstance(Type.GetTypeFromCLSID(DirectN.MFConstants.CLSID_MFCaptureEngineClassFactory)) as IMFCaptureEngineClassFactory;
            //object o;
            //capturefac.CreateInstance(DirectN.MFConstants.CLSID_MFCaptureEngine, typeof(IMFCaptureEngine).GUID, out o);
            //var em = o as IMFCaptureEngine;

            //var hr = CreateDX11Device(out g_pDX11Device, out m_pDeviceContext, out var level);

        }
        ID3D11Device g_pDX11Device;
        ID3D11DeviceContext m_pDeviceContext;
        HRESULT CreateDX11Device(out ID3D11Device ppDevice, out ID3D11DeviceContext ppDeviceContext, out D3D_FEATURE_LEVEL pFeatureLevel )
        {
            HRESULT hr = HRESULTS.S_OK;


            D3D_FEATURE_LEVEL[] levels = new []{
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

        async private void a_Click(object sender, RoutedEventArgs e)
        {
            TaskCompletionSource<int> t = new TaskCompletionSource<int>();
            await t.Task;
        }

        private void b_Click(object sender, RoutedEventArgs e)
        {

        }

        //HRESULT InitializeCaptureManager(IntPtr hwndPreview, IUnknown* pUnk)
        //{
        //    HRESULT hr = HRESULTS.S_OK;
        //    IMFAttributes pAttributes = null;
        //    IMFCaptureEngineClassFactory pFactory = NULL;

        //    DestroyCaptureEngine();

        //    m_hEvent = CreateEvent(NULL, FALSE, FALSE, NULL);
        //    if (NULL == m_hEvent)
        //    {
        //        hr = HRESULT_FROM_WIN32(GetLastError());
        //        goto Exit;
        //    }

        //    m_pCallback = new(std::nothrow) CaptureEngineCB(m_hwndEvent);
        //    if (m_pCallback == NULL)
        //    {
        //        hr = E_OUTOFMEMORY;
        //        goto Exit;
        //    }

        //    m_pCallback->m_pManager = this;
        //    m_hwndPreview = hwndPreview;

        //    //Create a D3D Manager
        //    hr = CreateD3DManager();
        //    if (FAILED(hr))
        //    {
        //        goto Exit;
        //    }
        //    hr = MFCreateAttributes(&pAttributes, 1);
        //    if (FAILED(hr))
        //    {
        //        goto Exit;
        //    }
        //    hr = pAttributes->SetUnknown(MF_CAPTURE_ENGINE_D3D_MANAGER, g_pDXGIMan);
        //    if (FAILED(hr))
        //    {
        //        goto Exit;
        //    }

        //    // Create the factory object for the capture engine.
        //    hr = CoCreateInstance(CLSID_MFCaptureEngineClassFactory, NULL,
        //        CLSCTX_INPROC_SERVER, IID_PPV_ARGS(&pFactory));
        //    if (FAILED(hr))
        //    {
        //        goto Exit;
        //    }

        //    // Create and initialize the capture engine.
        //    hr = pFactory->CreateInstance(CLSID_MFCaptureEngine, IID_PPV_ARGS(&m_pEngine));
        //    if (FAILED(hr))
        //    {
        //        goto Exit;
        //    }
        //    hr = m_pEngine->Initialize(m_pCallback, pAttributes, NULL, pUnk);
        //    if (FAILED(hr))
        //    {
        //        goto Exit;
        //    }

        //Exit:
        //    if (NULL != pAttributes)
        //    {
        //        pAttributes->Release();
        //        pAttributes = NULL;
        //    }
        //    if (NULL != pFactory)
        //    {
        //        pFactory->Release();
        //        pFactory = NULL;
        //    }
        //    return hr;
        //}
    }

}
