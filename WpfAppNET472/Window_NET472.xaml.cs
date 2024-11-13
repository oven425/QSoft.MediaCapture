using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using DirectN;
using QSoft.MediaCapture;
using QSoft.MediaCapture.WPF;

namespace WpfAppNET472
{
    /// <summary>
    /// Window_NET472.xaml 的互動邏輯
    /// </summary>
    public partial class Window_NET472 : Window
    {
        public Window_NET472()
        {
            InitializeComponent();
            Microsoft.Win32.SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
        }

        private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {

        }

        MainUI m_MainUI;
        Dictionary<string, WebCam_MF> m_WebCams = new Dictionary<string, WebCam_MF>();
        [DllImport("mfsensorgroup", ExactSpelling = true)]
        public static extern HRESULT MFCreateSensorGroup([MarshalAs(UnmanagedType.LPWStr)] string SensorGroupSymbolicLink, out IMFSensorGroup ppSensorGroup);
        WebCam_MF m_WebCam;
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {

            var orientation = System.Windows.Forms.SystemInformation.ScreenOrientation;
            System.Diagnostics.Trace.WriteLine($"orientation:{orientation}");

            if (m_MainUI == null)
            {
                this.DataContext = this.m_MainUI = new MainUI();
                foreach (var oo in QSoft.MediaCapture.WebCam_MF.GetAllWebCams())
                {
                    System.Diagnostics.Trace.WriteLine($"FriendName:{oo.FriendName}");
                    System.Diagnostics.Trace.WriteLine($"SymbolLinkName:{oo.SymbolLinkName}");
                    //try
                    //{
                    //    var hr = DirectN.Functions.MFCreateSensorGroup("SymbolLink", out var group);
                    //}
                    //catch(Exception ee)
                    //{
                    //    System.Diagnostics.Trace.WriteLine(ee.Message);
                    //}
                    
                    m_WebCams[oo.FriendName] = oo;
                }
                
                //foreach (var oo in QSoft.MediaCapture.WebCam_MF.GetAllWebCams().Skip(0).Take(1))
                //{
                //    oo.MediaCaptureFailedEventHandler += Oo_MediaCaptureFailedEventHandler;
                //    await oo.InitCaptureEngine(new WebCam_MF_Setting()
                //    {
                //        IsMirror = true,
                //        Rotate = 0
                //    });
                //    if(oo.WhiteBalance.IsSupported)
                //    {
                //        this.slider_whitebalance.Maximum = oo.WhiteBalance.Max;
                //        this.slider_whitebalance.Minimum = oo.WhiteBalance.Min;
                //        this.slider_whitebalance.Value = oo.WhiteBalance.Value;
                //        this.slider_whitebalance.SmallChange = oo.WhiteBalance.Step;
                //        this.slider_whitebalance.LargeChange = oo.WhiteBalance.Step;
                //    }
                //    System.Diagnostics.Trace.WriteLine($"FlashLight IsSupported:{oo.FlashLight.IsSupported}");
                //    System.Diagnostics.Trace.WriteLine($"TorchLight IsSupported:{oo.TorchLight.IsSupported}");
                //    //var currentmm = oo.GetMediaStreamProperties(MF_CAPTURE_ENGINE_STREAM_CATEGORY.MF_CAPTURE_ENGINE_STREAM_CATEGORY_VIDEO_CAPTURE);
                //    var photos = oo.GetAvailableMediaStreamProperties(MF_CAPTURE_ENGINE_STREAM_CATEGORY.MF_CAPTURE_ENGINE_STREAM_CATEGORY_PHOTO_DEPENDENT);
                //    //foreach (var mm in photos)
                //    //{
                //    //    System.Diagnostics.Trace.WriteLine($"{mm.Width}x{mm.Height} {mm.Fps} {WebCam_MFExtension.FormatToString(mm.SubType)}");
                //    //}
                //    var mms = oo.GetAvailableMediaStreamProperties(MF_CAPTURE_ENGINE_STREAM_CATEGORY.MF_CAPTURE_ENGINE_STREAM_CATEGORY_VIDEO_CAPTURE);
                //    //foreach (var mm in mms)
                //    //{
                //    //    System.Diagnostics.Trace.WriteLine($"{mm.Width}x{mm.Height} {mm.Fps} {WebCam_MFExtension.FormatToString(mm.SubType)}");
                //    //}
                //    //await oo.SetMediaStreamPropertiesAsync(MF_CAPTURE_ENGINE_STREAM_CATEGORY.MF_CAPTURE_ENGINE_STREAM_CATEGORY_VIDEO_CAPTURE, mms[6]);
                //    host.Visibility = Visibility.Collapsed;
                //    host.Visibility = Visibility.Visible;
                //    await oo.StartPreview(host.Child.Handle);
                //    //await oo.StartPreview(new Action<System.Windows.Interop.D3DImage>((x) => 
                //    //{
                //    //    //this.image.Source = x;
                //    //}), System.Windows.Threading.DispatcherPriority.Render);
                //    //await oo.StartPreview(x =>
                //    //{
                //    //    this.image.Source = x;
                //    //});
                //    this.m_WebCam = oo;
                //    //this.m_MainUI.WebCams.Add(oo);
                //}





            }
        }
        async Task OpenCamera(int index)
        {
            this.m_MainUI.RecordTypes.Clear();
            this.m_MainUI.PhotoTypes.Clear();
            foreach (var oo in this.m_WebCams)
            {
                oo.Value.Dispose();
            }
            m_WebCam = this.m_WebCams.ElementAt(index).Value;
            await m_WebCam.InitCaptureEngine(new WebCam_MF_Setting()
            {
                IsMirror = true,
            });
            this.m_MainUI.IsSupportTorch = this.m_WebCam.TorchLight.IsSupported;
            this.m_MainUI.Torchs.Clear();
            if (this.m_MainUI.IsSupportTorch)
            {
                foreach(var oo in this.m_WebCam.TorchLight.SupportStates)
                {
                    this.m_MainUI.Torchs.Add(oo);
                }
            }
            
            this.m_MainUI.IsSupportFlash = this.m_WebCam.FlashLight.IsSupported;
            
            System.Diagnostics.Trace.WriteLine($"{m_WebCam.FriendName}");
            var capturess = m_WebCam.GetAvailableMediaStreamProperties(MF_CAPTURE_ENGINE_STREAM_CATEGORY.MF_CAPTURE_ENGINE_STREAM_CATEGORY_VIDEO_CAPTURE);
            //System.Diagnostics.Trace.WriteLine($"record types");
            
            foreach (var oo in capturess.Where(x => x.SubType == DirectN.MFConstants.MFVideoFormat_NV12)
                .OrderBy(x => x.Width * x.Height))
            {
                this.m_MainUI.RecordTypes.Add(oo);
                //System.Diagnostics.Trace.WriteLine($"{oo.Width}x{oo.Height} {oo.Fps} {oo.SubType.FormatToString()}");
            }
            var photoss = m_WebCam.GetAvailableMediaStreamProperties(MF_CAPTURE_ENGINE_STREAM_CATEGORY.MF_CAPTURE_ENGINE_STREAM_CATEGORY_PHOTO_DEPENDENT);
            //System.Diagnostics.Trace.WriteLine($"photo types");

            foreach (var oo in photoss.Where(x => x.SubType == DirectN.MFConstants.MFVideoFormat_NV12)
                .OrderBy(x => x.Width * x.Height))
            {
                this.m_MainUI.PhotoTypes.Add(oo);
                //System.Diagnostics.Trace.WriteLine($"{oo.Width}x{oo.Height} {oo.Fps} {oo.SubType.FormatToString()}");
            }
            if(photoss.Count > 0)
            {
                await m_WebCam.SetMediaStreamPropertiesAsync( MF_CAPTURE_ENGINE_STREAM_CATEGORY.MF_CAPTURE_ENGINE_STREAM_CATEGORY_PHOTO_DEPENDENT, this.m_MainUI.PhotoTypes.Last());
            }
            m_WebCam.SetMediaStreamPropertiesAsync(MF_CAPTURE_ENGINE_STREAM_CATEGORY.MF_CAPTURE_ENGINE_STREAM_CATEGORY_VIDEO_CAPTURE, m_MainUI.RecordTypes.LastOrDefault());

            this.host.Visibility = Visibility.Visible;
            await m_WebCam.StartPreview(this.host.Child.Handle);

            //this.host.Visibility = Visibility.Collapsed;
            //await m_WebCam.StartPreview(bmp=>this.image.Source = bmp);
        }

        private void Oo_MediaCaptureFailedEventHandler(object sender, MediaCaptureFailedEventArgs e)
        {
        }

        private async void button_stoppreview_Click(object sender, RoutedEventArgs e)
        {
            var hr = await m_WebCam?.StopPreview();
        }

        private async void button_stratpreivew_Click(object sender, RoutedEventArgs e)
        {
            this.host.Visibility = Visibility.Visible;
            await m_WebCam.StartPreview(this.host.Child.Handle);

            //this.host.Visibility = Visibility.Collapsed;
            //await m_WebCam.StartPreview(bmp=>this.image.Source = bmp);
        }

        private void picture_SizeChanged(object sender, EventArgs e)
        {
            //if (this.m_MainUI == null) return;
            //System.Windows.Forms.Control aa = sender as System.Windows.Forms.Control;
            //foreach (var oo in this.m_MainUI.WebCams)
            //{
            //    oo.UpdateVideo((int)aa.Width, (int)aa.Height);
            //}
        }

        async private void button_takephoto_Click(object sender, RoutedEventArgs e)
        {
            var hr = await m_WebCam.TakePhoto($"{DateTime.Now:yyyyMMdd_HHmmss}.jpg");
        }

        private async void button_startrecord_Click(object sender, RoutedEventArgs e)
        {
            var hr = await m_WebCam.StartRecord($"{DateTime.Now:yyyyMMdd_HHmmss}.mp4");

        }

        private async void button_stoprecord_Click(object sender, RoutedEventArgs e)
        {
            await m_WebCam.StopRecord();
        }

        private void slider_whitebalance_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            m_WebCam?.WhiteBalance.SetValue((int)e.NewValue, false);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var combobox = sender as ComboBox;
            var sw = System.Diagnostics.Stopwatch.StartNew();
            switch (combobox.SelectedIndex)
            {
                case 0:
                    {
                        m_WebCam.FlashLight.Set(0);
                    }
                    break;
                case 1:
                    {
                        m_WebCam.FlashLight.Set(1);
                    }
                    break;
                case 2:
                    {
                        m_WebCam.FlashLight.Set(4);
                    }
                    break;
            }
            sw.Stop();
            System.Diagnostics.Trace.WriteLine($"FlashLight:{sw.ElapsedMilliseconds}");
        }

        private void combobox_torch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var combobox = sender as ComboBox;
            var sw = System.Diagnostics.Stopwatch.StartNew();
            switch (combobox.SelectedIndex)
            {
                case 0:
                    {
                        m_WebCam.TorchLight.SetState(TorchLightState.OFF);
                    }
                    break;
                case 1:
                    {
                        m_WebCam.TorchLight.SetState(TorchLightState.ON);
                    }
                    break;
            }
            sw.Stop();
            System.Diagnostics.Trace.WriteLine($"TorchLight:{sw.ElapsedMilliseconds}");

        }
        int m_Index = -1;
        async private void button_nextcamera_Click(object sender, RoutedEventArgs e)
        {
            m_Index++;
            if(this.m_Index >= this.m_WebCams.Count)
            {
                m_Index = 0;
            }
            await this.OpenCamera(m_Index);
        }
    }



    public class MainUI:INotifyPropertyChanged
    {
        bool m_IsSupportFlash;
        bool m_IsSupportTorch;
        public bool IsSupportTorch
        {
            set { m_IsSupportTorch = value;this.Update("IsSupportTorch"); }
            get => m_IsSupportTorch;
        }
        public bool IsSupportFlash
        {
            set { m_IsSupportFlash = value; this.Update("IsSupportFlash"); }
            get => m_IsSupportFlash;
        }

        public ObservableCollection<TorchLightState> Torchs { set; get; } = new ObservableCollection<TorchLightState>();
        public ObservableCollection<ImageEncodingProperties> RecordTypes { set; get; }=new ObservableCollection<ImageEncodingProperties>();
        public ObservableCollection<ImageEncodingProperties> PhotoTypes { set; get; }  =new ObservableCollection<ImageEncodingProperties>();

        public event PropertyChangedEventHandler PropertyChanged;
        void Update(string name)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)); 
        }
        //public ObservableCollection<WebCam_MF> WebCams { set; get; } = new ObservableCollection<WebCam_MF>();
    }
}
