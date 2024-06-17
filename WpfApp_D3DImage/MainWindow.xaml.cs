using DirectN;
using SharpDX.Direct3D9;
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

namespace WpfApp_D3DImage
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        async private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var webcam = QSoft.MediaCapture.WebCam_MF.GetAllWebCams()[0];
            await webcam.InitCaptureEngine();
            await webcam.SetPreviewSize(x => x.FirstOrDefault(y => y.SubType == DirectN.MFConstants.MFVideoFormat_NV12));
            await webcam.StartPreview1(new NV12PreviewSink());
        }
    }

    public class NV12PreviewSink: IMFCaptureEngineOnSampleCallback
    {
        public HRESULT OnSample(IMFSample pSample)
        {
            //pSample.ConvertToContiguousBuffer(out var buffer);
            pSample.GetBufferByIndex(0, out var buffer);
            IDirect3DSurface9 d3d9surface ;
            var hr = MFFunctions.MFGetService(buffer, MFConstants.MR_BUFFER_SERVICE, new Guid("0cfbaf3a-9ff6-429a-99b3-a2796af8b89b"), out var obj);
            
            var len = buffer.GetCurrentLength();
            var ptr = buffer.Lock();
            var surface = Surface.FromPointer<Surface>(ptr);
            var device = surface.Device;
            var d3d = device.Direct3D;
            buffer.Unlock();
            Marshal.ReleaseComObject(pSample);
            return HRESULTS.S_OK;
        }
    }
}
