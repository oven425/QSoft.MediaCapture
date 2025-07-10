using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Windows.Devices.Enumeration;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.Capture.Frames;
using Windows.Media.Devices;
using Windows.Media.MediaProperties;
using Windows.Storage;
using WinRT;

namespace WpfApp_MediaCapture
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        MediaCapture m_MediaCapture;
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.m_MediaCapture = new MediaCapture();
            var videoSource = await GetVideoSource();
            await this.m_MediaCapture.InitializeAsync(new()
            {
                VideoDeviceId = videoSource.Id,
                StreamingCaptureMode = StreamingCaptureMode.Video,
                MemoryPreference = MediaCaptureMemoryPreference.Auto
                

            });
            var aaa = m_MediaCapture.VideoDeviceController.GetAvailableMediaStreamProperties(Windows.Media.Capture.MediaStreamType.VideoPreview);
            var mmm = aaa.OfType<VideoEncodingProperties>().Where(x=>x.Subtype=="NV12").MaxBy(x=>x.Width*x.Height);
            await m_MediaCapture.VideoDeviceController.SetMediaStreamPropertiesAsync(MediaStreamType.VideoRecord, mmm);


            var source = this.m_MediaCapture.FrameSources.MaxBy(x => x.Value.SupportedFormats.Count);
            this.m_MediaFrameReader = await this.m_MediaCapture.CreateFrameReaderAsync(source.Value, MediaEncodingSubtypes.Argb32);
            this.m_MediaFrameReader.FrameArrived += M_MediaFrameReader_FrameArrived;
            m_WriteableBitmap = new WriteableBitmap((int)mmm.Width, (int)mmm.Height, 96,96, PixelFormats.Bgr32, null);
            this.image.Source = m_WriteableBitmap;
            await this.m_MediaFrameReader.StartAsync();
        }

        bool isrun = false;
        private async void M_MediaFrameReader_FrameArrived(MediaFrameReader sender, MediaFrameArrivedEventArgs args)
        {
            if (isrun) return;
            isrun = true;
            try
            {
                using var latestFrame = sender.TryAcquireLatestFrame();
                //using var softwareBitmap = latestFrame?.VideoMediaFrame?.SoftwareBitmap;
                using var d3d = latestFrame?.VideoMediaFrame?.Direct3DSurface;
                if (d3d == null) return;
                using var softwareBitmap = await SoftwareBitmap.CreateCopyFromSurfaceAsync(d3d);
                this.Dispatcher.Invoke(() =>
                {
                    

                    using var m = softwareBitmap.LockBuffer(BitmapBufferAccessMode.Read);
                    using var reference = m.CreateReference();

                    m_WriteableBitmap.Lock();

                    unsafe
                    {
                        (reference.As<IMemoryBufferByteAccess>()).GetBuffer(out var ptr, out var capacity);

                        ////way 1:
                        //writeableBitmap.WritePixels(
                        //    react,
                        //    (IntPtr)ptr,
                        //    (int)capacity,
                        //    t.Stride);

                        NativeMemory.Copy(ptr, (void*)m_WriteableBitmap.BackBuffer, capacity);
                        m_WriteableBitmap.AddDirtyRect(new Int32Rect(0, 0, m_WriteableBitmap.PixelWidth, m_WriteableBitmap.PixelHeight));
                        //way 2:
                        /*CopyMemory(writeableBitmap.BackBuffer, (IntPtr)ptr, ImageBufferSize);
                        writeableBitmap.AddDirtyRect(react);*/
                    }
                    m_WriteableBitmap.Unlock();

                });

            }
            finally
            {
                isrun = false;
            }
            
        }

        [ComImport]
        [Guid("5B0D3235-4DBA-4D44-865E-8F1D0E4FD04D")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        unsafe interface IMemoryBufferByteAccess
        {
            void GetBuffer(out byte* buffer, out uint capacity);
        }

        WriteableBitmap? m_WriteableBitmap;

        MediaFrameReader m_MediaFrameReader;

        async Task<MediaFrameSourceGroup> GetVideoSource()
        {
            var cameras = await MediaFrameSourceGroup.FindAllAsync();
            return cameras.Count > 0 ? cameras[0] : null;
        }

        private async void button_startrecord_Click(object sender, RoutedEventArgs e)
        {
            var profile = Windows.Media.MediaProperties.MediaEncodingProfile.CreateMp4(Windows.Media.MediaProperties.VideoEncodingQuality.Auto);
            var myVideos = await Windows.Storage.StorageLibrary.GetLibraryAsync(Windows.Storage.KnownLibraryId.Videos);
            StorageFile file = await myVideos.SaveFolder.CreateFileAsync("video.mp4", CreationCollisionOption.GenerateUniqueName);
            //await this.m_MediaCapture.StartRecordToStorageFileAsync(profile, file);

            m_LowLagRecord  = await this.m_MediaCapture.PrepareLowLagRecordToStorageFileAsync(profile, file);
        }
        LowLagMediaRecording m_LowLagRecord;
        async private void button_stoprecord_Click(object sender, RoutedEventArgs e)
        {
            if(m_LowLagRecord != null)
            {
                await m_LowLagRecord.StopAsync();
                await m_LowLagRecord.FinishAsync();
            }
            //await this.m_MediaCapture.StopRecordAsync();
        }
    }
}