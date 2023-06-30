using DirectN;
using QSoft.MediaCapture;
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
            var sources = attribute.EnumDeviceSources();
            WebCam_MF mf = new WebCam_MF();
            mf.InitializeCaptureManager(IntPtr.Zero, sources.ElementAt(0));
            mf.StartPreview();
            //MFFunctions.MFStartup();
            //var capturefac = Activator.CreateInstance(Type.GetTypeFromCLSID(DirectN.MFConstants.CLSID_MFCaptureEngineClassFactory)) as IMFCaptureEngineClassFactory;
            //object o;
            //capturefac.CreateInstance(DirectN.MFConstants.CLSID_MFCaptureEngine, typeof(IMFCaptureEngine).GUID, out o);
            //var em = o as IMFCaptureEngine;

            //var hr = CreateDX11Device(out g_pDX11Device, out m_pDeviceContext, out var level);

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
