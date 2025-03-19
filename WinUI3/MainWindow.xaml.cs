using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using System.Windows.Controls;
using Windows.Media.MediaProperties;
using QSoft.MediaCapture;
using DirectN;
using QSoft.DevCon;
using System.Runtime.InteropServices;
using System.Collections.Concurrent;
using System.Collections;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUI3
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }

        private async void myButton_Click(object sender, RoutedEventArgs e)
        {
    

        }

        async private void Window_Activated(object sender, WindowActivatedEventArgs args)
        {
            //var aa = QSoft.DevCon.DevConExtension.KSCATEGORY_VIDEO_CAMERA.DevicesFromInterface()
            //    .Select(x => new { path = x.DevicePath(), panel = x.As().Panel() })
            //    .FirstOrDefault();
            //var webcam = WebCam_MF.CreateFromSymbollink(aa.path);
            var webcam = WebCam_MF.GetAllWebCams().FirstOrDefault();
            await webcam.InitCaptureEngine(new WebCam_MF_Setting()
            {
                IsMirror = true
            });
            var aa1 = webcam.GetMediaStreamProperties(MF_CAPTURE_ENGINE_STREAM_CATEGORY.MF_CAPTURE_ENGINE_STREAM_CATEGORY_VIDEO_CAPTURE);
            await webcam.StartPreview(new AA(queue) );
            var encdoing = VideoEncodingProperties.CreateUncompressed(MediaEncodingSubtypes.Nv12, 1280, 720);
            var vd = new VideoStreamDescriptor(encdoing);
            var source = new MediaStreamSource(vd);
            source.IsLive = true;
            source.CanSeek = false;
            source.SampleRequested += Source_SampleRequested;
            preview.Source = MediaSource.CreateFromMediaStreamSource(source);
            preview.AutoPlay = true;
        }
        ConcurrentQueue<byte[]> queue = new ConcurrentQueue<byte[]>();
        private void Source_SampleRequested(MediaStreamSource sender, MediaStreamSourceSampleRequestedEventArgs args)
        {
            if(queue.TryDequeue(out var data))
            {
                var sample = MediaStreamSample.CreateFromBuffer(data.AsBuffer(), TimeSpan.FromMilliseconds(33));
                sample.Duration = TimeSpan.FromMilliseconds(33);
                //secondsPosition += sample.Duration.TotalSeconds;
                sample.KeyFrame = true;
                args.Request.Sample = sample;
            }
        }
    }

    public class AA(ConcurrentQueue<byte[]> queue) :DirectN.IMFCaptureEngineOnSampleCallback
    {
        [DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
        internal static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);

        public HRESULT OnSample(IMFSample pSample)
        {
            pSample.ConvertToContiguousBuffer(out var buf);
            var ptr = buf.Lock(out var max, out var cur);
            byte[] data = new byte[cur];
            Marshal.Copy(ptr, data, 0, (int)cur);
            queue.Enqueue(data);
            buf.Unlock();
            Marshal.ReleaseComObject(buf);
            Marshal.ReleaseComObject(pSample);
            return HRESULTS.S_OK;
        }
    }
}
