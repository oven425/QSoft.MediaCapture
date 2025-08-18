using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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
        MainUI m_MainUI;
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this.m_MainUI = new MainUI();
        }

        MediaCapture m_MediaCapture;
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.m_MediaCapture = new MediaCapture();
            var videoSource = await GetVideoSource();
            if (videoSource is null) return;
            await this.m_MediaCapture.InitializeAsync(new()
            {
                VideoDeviceId = videoSource.Id,
                SharingMode = MediaCaptureSharingMode.ExclusiveControl,
                StreamingCaptureMode = StreamingCaptureMode.AudioAndVideo,
                MemoryPreference = MediaCaptureMemoryPreference.Auto
            });
            var aaa = m_MediaCapture.VideoDeviceController.GetAvailableMediaStreamProperties(Windows.Media.Capture.MediaStreamType.VideoPreview)
                .OfType<VideoEncodingProperties>()
                //.Where(x => x.Subtype == "NV12")
                .OrderByDescending(x => x.Width * x.Height)
                .ThenByDescending(x => x.FrameRate.Numerator / x.FrameRate.Denominator);
            System.Diagnostics.Trace.WriteLine("Available Video Preview Formats:");
            foreach (var oo in aaa)
            {
                System.Diagnostics.Trace.WriteLine(ToString(oo));
                this.m_MainUI.Previews.Add(oo);
            }

            var rrs = this.m_MediaCapture.VideoDeviceController.GetAvailableMediaStreamProperties(MediaStreamType.VideoRecord)
                .OfType<VideoEncodingProperties>()
                .OrderByDescending(x => x.Width * x.Height)
                .ThenByDescending(x => x.FrameRate.Numerator / x.FrameRate.Denominator);
            System.Diagnostics.Trace.WriteLine("Available Video Record Formats:");
            foreach (var oo in rrs)
            {
                System.Diagnostics.Trace.WriteLine(ToString(oo));
                this.m_MainUI.Records.Add(oo);
            }
            var rr = this.m_MediaCapture.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoRecord) as VideoEncodingProperties;
            this.m_MainUI.Record = this.m_MainUI.Records.FirstOrDefault(x=>x.Subtype == rr.Subtype&&x.Width==rr.Width&&x.Height==rr.Height &&x.FrameRate.Denominator==rr.FrameRate.Denominator&&x.FrameRate.Denominator==rr.FrameRate.Denominator);
        }

        string ToString(VideoEncodingProperties prop)
        {
            var sb = new StringBuilder();
            sb.Append(prop.Subtype);
            sb.Append(" ");
            sb.Append(prop.Width);
            sb.Append("x");
            sb.Append(prop.Height);
            if (prop.FrameRate.Numerator > 0 && prop.FrameRate.Denominator > 0)
            {
                sb.Append(" ");
                sb.Append(prop.FrameRate.Numerator / prop.FrameRate.Denominator);
                sb.Append("fps");
            }
            return sb.ToString();
        }

        readonly SemaphoreSlim m_Lock = new(1, 1);
        private async void M_MediaFrameReader_FrameArrived(MediaFrameReader sender, MediaFrameArrivedEventArgs args)
        {
            if (!await m_Lock.WaitAsync(0)) return;
            try
            {

                
                using var latestFrame = sender.TryAcquireLatestFrame();
                if (latestFrame is null) return;
                if(latestFrame.VideoMediaFrame is null) return;
                using var d3d = latestFrame?.VideoMediaFrame?.Direct3DSurface;

                using var softwareBitmap = d3d == null ? latestFrame?.VideoMediaFrame?.SoftwareBitmap : await SoftwareBitmap.CreateCopyFromSurfaceAsync(d3d);
                if (softwareBitmap is not null)
                {
                    await this.Dispatcher.InvokeAsync(() =>
                    {
                        using var m = softwareBitmap.LockBuffer(BitmapBufferAccessMode.Read);
                        using var reference = m.CreateReference();

                        m_WriteableBitmap.Lock();

                        unsafe
                        {
                            (reference.As<IMemoryBufferByteAccess>()).GetBuffer(out var ptr, out var capacity);

                            NativeMemory.Copy(ptr, (void*)m_WriteableBitmap.BackBuffer, capacity);
                            m_WriteableBitmap.AddDirtyRect(new Int32Rect(0, 0, m_WriteableBitmap.PixelWidth, m_WriteableBitmap.PixelHeight));
                        }
                        m_WriteableBitmap.Unlock();

                    });
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex);
            }
            finally
            {
                m_Lock.Release();
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

        MediaFrameReader? m_MediaFrameReader;

        async Task<MediaFrameSourceGroup?> GetVideoSource()
        {
            var cameras = await MediaFrameSourceGroup.FindAllAsync();
            foreach (var oo in cameras)
            {
                System.Diagnostics.Trace.WriteLine(oo.DisplayName);
            }
            return cameras.Count > 0 ? cameras[0] : null;
        }

        private async void button_startrecord_Click(object sender, RoutedEventArgs e)
        {
            var profile = Windows.Media.MediaProperties.MediaEncodingProfile.CreateMp4(Windows.Media.MediaProperties.VideoEncodingQuality.Auto);
            var myVideos = await Windows.Storage.StorageLibrary.GetLibraryAsync(Windows.Storage.KnownLibraryId.Videos);
            StorageFile file = await myVideos.SaveFolder.CreateFileAsync("video.mp4", CreationCollisionOption.GenerateUniqueName);

            m_LowLagRecord = await this.m_MediaCapture.PrepareLowLagRecordToStorageFileAsync(profile, file);
            await m_LowLagRecord.StartAsync();
        }

        LowLagMediaRecording? m_LowLagRecord;
        async private void button_stoprecord_Click(object sender, RoutedEventArgs e)
        {
            if (m_LowLagRecord != null)
            {
                await m_LowLagRecord.StopAsync();
                await m_LowLagRecord.FinishAsync();
                m_LowLagRecord = null;
            }
        }

        async private void combobox_preview_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.m_MediaFrameReader != null)
            {
                await this.m_MediaFrameReader.StopAsync();
                this.m_MediaFrameReader.Dispose();
                this.m_MediaFrameReader = null;
            }
            if (this.m_LowLagRecord != null)
            {
                await this.m_LowLagRecord.StopAsync();
                await this.m_LowLagRecord.FinishAsync();
                this.m_LowLagRecord = null;
            }
            await  m_MediaCapture.VideoDeviceController.SetMediaStreamPropertiesAsync(MediaStreamType.VideoPreview, this.m_MainUI.Preview);

            var source = this.m_MediaCapture.FrameSources.MaxBy(x => x.Value.SupportedFormats.Count);
            this.m_MediaFrameReader = await this.m_MediaCapture.CreateFrameReaderAsync(source.Value, MediaEncodingSubtypes.Argb32);
            this.m_MediaFrameReader.FrameArrived += M_MediaFrameReader_FrameArrived;
            m_WriteableBitmap = new WriteableBitmap((int)this.m_MainUI.Preview.Width, (int)this.m_MainUI.Preview.Height, 96, 96, PixelFormats.Bgr32, null);
            this.image.Source = m_WriteableBitmap;
            await this.m_MediaFrameReader.StartAsync();

        }

        async private void combobox_record_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(e.AddedItems.Count>0)
            {
                var rr = e.AddedItems[0] as VideoEncodingProperties;
                await this.m_MediaCapture.VideoDeviceController.SetMediaStreamPropertiesAsync(MediaStreamType.VideoRecord, rr);
            }
            
        }
    }


    public class MainUI : INotifyPropertyChanged
    {
        VideoEncodingProperties m_Preview;
        public VideoEncodingProperties Preview
        {
            set
            {
                m_Preview = value;
                this.Update("Preview");
            }
            get => m_Preview;
        }

        VideoEncodingProperties m_Record;
        public VideoEncodingProperties Record
        {
            set
            {
                m_Record = value;
                this.Update("Record");
            }
            get => m_Record;
        }
        public ObservableCollection<VideoEncodingProperties> Previews { set; get; } = new ObservableCollection<VideoEncodingProperties>();

        public ObservableCollection<VideoEncodingProperties> Records { set; get; } = new ObservableCollection<VideoEncodingProperties>();

        public event PropertyChangedEventHandler? PropertyChanged;
        void Update(string name) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}