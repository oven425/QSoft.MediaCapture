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
using QSoft.DevCon;
using QSoft.MediaCapture;
using QSoft.MediaCapture.Sensor;
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
            System.Diagnostics.Trace.WriteLine(System.Windows.Forms.SystemInformation.ScreenOrientation);
            
        }
        
        MainUI m_MainUI;
        Dictionary<string, WebCam_MF> m_WebCams = new Dictionary<string, WebCam_MF>();
        [DllImport("mfsensorgroup", ExactSpelling = true)]
        public static extern HRESULT MFCreateSensorGroup([MarshalAs(UnmanagedType.LPWStr)] string SensorGroupSymbolicLink, out IMFSensorGroup ppSensorGroup);
        WebCam_MF m_WebCam;
        
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {


            WebCam_MF.EnumAudioCapture();

            var orientation = System.Windows.Forms.SystemInformation.ScreenOrientation;
            System.Diagnostics.Trace.WriteLine($"orientation:{orientation}");

            if (m_MainUI == null)
            {
                this.DataContext = this.m_MainUI = new MainUI();
                var vvvv = QSoft.MediaCapture.WebCam_MF.GetAllWebCams();
                foreach (var oo in QSoft.MediaCapture.WebCam_MF.GetAllWebCams())
                {
                    System.Diagnostics.Trace.WriteLine($"FriendName:{oo.FriendName}");
                    System.Diagnostics.Trace.WriteLine($"SymbolLinkName:{oo.SymbolLinkName}");
                    m_WebCams[oo.FriendName] = oo;
                }

                var audio = new Guid("{FBF6F530-07B9-11D2-A71E-0000F8004788}").DevicesFromInterface()
                    .Select(x => new
                    {
                        symbollink = x.DevicePath(),
                        friendname = x.As().GetFriendName(),
                        desc = x.As().GetDeviceDesc(),
                    }).ToList();

                var cccs = QSoft.DevCon.DevConExtension.KSCATEGORY_VIDEO_CAMERA.DevicesFromInterface()
                    .Select(x => new
                    {
                        symbollink = x.DevicePath(),
                        friendname = x.As().GetFriendName(),
                        desc = x.As().GetDeviceDesc(),
                        panel = x.As().Panel()
                    });
                foreach(var oo in cccs)
                {
                    SensorGroup sg = new SensorGroup(oo.symbollink);
                    var nn = oo.friendname;
                    if(string.IsNullOrEmpty(nn))
                    {
                        nn = oo.desc;
                    }
                    this.m_MainUI.Cameras.Add(Tuple.Create(nn, oo.symbollink, oo.panel));
                }
                this.combobox_cameras.SelectedIndex = 0;
            }
        }

        async Task OpenCamera(WebCam_MF webcam, QSoft.DevCon.CameraPanel panel)
        {
            m_bb = false;
            this.m_MainUI.VideoCaptureFormats.Clear();
            this.m_MainUI.PhotoDependentFormats.Clear();

            m_WebCam?.Dispose();
            m_WebCam = webcam;

            var or = System.Windows.Forms.SystemInformation.ScreenOrientation;
            CameraRotates rotate = CameraRotates.Rotate0;
            switch (or)
            {
                case System.Windows.Forms.ScreenOrientation.Angle0:
                    rotate = CameraRotates.Rotate0;
                    break;
                case System.Windows.Forms.ScreenOrientation.Angle90:
                    rotate = CameraRotates.Rotate90;
                    break;
                case System.Windows.Forms.ScreenOrientation.Angle180:
                    rotate = CameraRotates.Rotate180;
                    break;
                case System.Windows.Forms.ScreenOrientation.Angle270:
                    rotate = CameraRotates.Rotate270;
                    break;
            }
            await m_WebCam.InitCaptureEngine(new WebCam_MF_Setting()
            {
                Shared = false,
                Rotate = rotate,
                IsMirror = panel == CameraPanel.Front,
                UseD3D = this.m_MainUI.UseD3D
            });

            this.m_MainUI.EyeGazeCorrectionStates.Clear();
            if(this.m_WebCam.EyeGazeCorrection.IsSupported)
            {
                foreach(var oo in this.m_WebCam.EyeGazeCorrection.SupportStates)
                {
                    this.m_MainUI.EyeGazeCorrectionStates.Add(oo);
                }
                this.m_MainUI.EyeGazeCorrectionState = this.m_WebCam.EyeGazeCorrection.GetState().First();
            }

            this.m_MainUI.DigitalWindowStates.Clear();
            if(this.m_WebCam.DigitalWindow.IsSupported)
            {
                foreach(var oo in this.m_WebCam.DigitalWindow.SupportStates)
                {
                    this.m_MainUI.DigitalWindowStates.Add(oo);
                }
                this.m_MainUI.DigitalWindowState = this.m_WebCam.DigitalWindow.GetState();
            }

            this.m_MainUI.BackgroundSegmentations.Clear();
            if(this.m_WebCam.BackgroundSegmentation.IsSupported)
            {
                foreach(var oo in this.m_WebCam.BackgroundSegmentation.SupportStates)
                {
                    this.m_MainUI.BackgroundSegmentations.Add(oo);
                }
                this.m_MainUI.BackgroundSegmentation = this.m_WebCam.BackgroundSegmentation.GetState().First();
            }


            this.m_MainUI.IsSupportTorch = this.m_WebCam.TorchLight?.IsSupported == true;
            this.m_MainUI.Torchs.Clear();
            if (this.m_MainUI.IsSupportTorch)
            {
                foreach (var oo in this.m_WebCam.TorchLight.SupportStates)
                {
                    this.m_MainUI.Torchs.Add(oo);
                }
                this.m_MainUI.Torch = this.m_WebCam.TorchLight.GetState();
            }

            this.m_MainUI.IsSupportFlash = this.m_WebCam.FlashLight?.IsSupported == true;
            this.m_MainUI.FlashLights.Clear();
            if (this.m_MainUI.IsSupportFlash)
            {
                foreach (var oo in this.m_WebCam.FlashLight.SupportStates)
                {
                    this.m_MainUI.FlashLights.Add(oo);
                }
                this.m_MainUI.FlashLight = this.m_WebCam.FlashLight.GetState();
            }
            if(this.m_WebCam.FaceDetectionControl.IsSupported)
            {
                var oi = this.m_WebCam.FaceDetectionControl.SupportStates;
                
                m_WebCam.FaceDetectionControl.SetState();
                var ss = m_WebCam.FaceDetectionControl.GetState();
                m_WebCam.FaceDetectionControl.FaceDetectionEvent += (sender, args) =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        canvas.Children.Clear();
                        foreach (var oo in args.FaceRects)
                        {
                            var rc = new Rect(new Point(1-oo.right, oo.bottom), new Point(1-oo.left, oo.top));
                            var rect = new Rectangle();
                            rect.Stroke = Brushes.Red;
                            rect.StrokeThickness = 1;
                            rect.Width = rc.Width * canvas.ActualWidth;
                            rect.Height = rc.Height * canvas.ActualHeight;
                            Canvas.SetLeft(rect, rc.Left * canvas.ActualWidth);
                            Canvas.SetTop(rect, rc.Top * canvas.ActualHeight);
                            canvas.Children.Add(rect);
                            
                        }
                        
                        
                    });

                    //System.Diagnostics.Trace.WriteLine($"FaceDetectionEvent:{args.FaceRects.Count}");
                };
            }

            this.m_MainUI.ColorTemperaturePresets.Clear();
            //if (this.m_WebCam.WhiteBalanceControl.IsSupport)
            //{
            //    foreach (var oo in typeof(ColorTemperaturePreset).GetEnumValues().Cast<ColorTemperaturePreset>())
            //    {
            //        this.m_MainUI.ColorTemperaturePresets.Add(oo);
            //    }
            //    this.m_MainUI.ColorTemperaturePreset = this.m_WebCam.WhiteBalanceControl.Preset;
            //    this.m_MainUI.WhiteBalance.Value = (int)m_WebCam.WhiteBalanceControl.Value;
            //    this.m_MainUI.WhiteBalance.IsAuto = m_WebCam.WhiteBalanceControl.IsAuto;
            //    this.slider_whitebalance.SmallChange = this.m_WebCam.WhiteBalanceControl.Step;
            //    this.slider_whitebalance.Minimum = this.m_WebCam.WhiteBalanceControl.Min;

            //    this.slider_whitebalance.Maximum = this.m_WebCam.WhiteBalanceControl.Max;
            //}
            System.Diagnostics.Trace.WriteLine($"{m_WebCam.FriendName}");
            //m_WebCam.GetMM();
            var capturess = m_WebCam.GetAvailableMediaStreamProperties(MF_CAPTURE_ENGINE_STREAM_CATEGORY.MF_CAPTURE_ENGINE_STREAM_CATEGORY_VIDEO_CAPTURE);
            //System.Diagnostics.Trace.WriteLine($"record types");
            var gg = capturess.GroupBy(x => x.SubType);
            foreach (var oo in capturess.Where(x => x.SubType != DirectN.MFConstants.MFVideoFormat_YUY2)
                .OrderByDescending(x => x.ImageSize)
                .ThenByDescending(x => x.Fps))
                
            {
                this.m_MainUI.VideoCaptureFormats.Add(oo);
            }
            this.m_MainUI.RecordFormat = this.m_MainUI.VideoCaptureFormats[0];
            //await m_WebCam.SetMediaStreamPropertiesAsync(this.m_MainUI.VideoCaptureFormats[0]);

            var photoss = m_WebCam.GetAvailableMediaStreamProperties(MF_CAPTURE_ENGINE_STREAM_CATEGORY.MF_CAPTURE_ENGINE_STREAM_CATEGORY_PHOTO_DEPENDENT);
            //System.Diagnostics.Trace.WriteLine($"photo types");

            foreach (var oo in photoss.Where(x => x.SubType == DirectN.MFConstants.MFVideoFormat_NV12)
                .OrderBy(x => x.Width * x.Height))
            {
                this.m_MainUI.PhotoDependentFormats.Add(oo);
            }
            if (photoss.Count > 0)
            {
                await m_WebCam.SetMediaStreamPropertiesAsync(this.m_MainUI.PhotoDependentFormats.LastOrDefault());
            }
            //var v1 = m_MainUI.VideoCaptureFormats.FirstOrDefault(x => x.Width == 1920 && x.Fps == 30);
            //this.m_MainUI.RecordFormat = m_MainUI.VideoCaptureFormats.FirstOrDefault();
            this.m_MainUI.VideoProcAmps.Clear();
            foreach (var oo in this.m_WebCam.VideoProcAmps)
            {
                this.m_MainUI.VideoProcAmps.Add(oo.Value);
                System.Diagnostics.Trace.WriteLine($"VideoProcAmp:{oo.Key} IsSupport:{oo.Value.IsSupport} IsAuto:{oo.Value.IsAuto} Value:{oo.Value.Value} Min:{oo.Value.Min} Max:{oo.Value.Max} Step:{oo.Value.Step}");
            }

            this.m_MainUI.CameraControls.Clear();
            foreach (var oo in this.m_WebCam.CameraControls)
            {
                this.m_MainUI.CameraControls.Add(oo.Value);
                System.Diagnostics.Trace.WriteLine($"CameraControl:{oo.Key} IsSupport:{oo.Value.IsSupport} IsAuto:{oo.Value.IsAuto} Value:{oo.Value.Value} Min:{oo.Value.Min} Max:{oo.Value.Max} Step:{oo.Value.Step}");
            }
        }

        async Task StartPreviewAsync()
        {
            //this.host.Visibility = Visibility.Visible;
            //await m_WebCam.StartPreview(this.host.Child.Handle);

            this.host.Visibility = Visibility.Collapsed;
            await m_WebCam.StartPreview2(() => this.image);

            //this.host.Visibility = Visibility.Collapsed;
            //await m_WebCam.StartPreviewL8(() => this.image);

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
            await StartPreviewAsync();
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
            //var hr = await m_WebCam.StartRecord($"{DateTime.Now:yyyyMMdd_HHmmss}.mp4");
            var hr = await m_WebCam.StartRecord1($"aa.mp4");
        }

        private async void button_stoprecord_Click(object sender, RoutedEventArgs e)
        {
            await m_WebCam.StopRecord();
        }

        private void slider_whitebalance_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            
            //m_WebCam.WhiteBalanceControl.Value = (int)e.NewValue;
        }

        private void checkbox_wh_auto_Click(object sender, RoutedEventArgs e)
        {

        }
        bool m_bb = false;
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(!m_bb) return;
            var combobox = sender as ComboBox;
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var state = combobox.SelectedItem as FlashState?;
            System.Diagnostics.Trace.WriteLine($"FlashLight:{state}");
            m_WebCam.FlashLight.SetState(state ?? FlashState.OFF);

            sw.Stop();
            System.Diagnostics.Trace.WriteLine($"Set FlashLight:{sw.ElapsedMilliseconds}");
            System.Diagnostics.Trace.WriteLine($"get FlashLight:{m_WebCam.FlashLight.GetState()}");
        }

        private void combobox_torch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!m_bb) return;
            var combobox = sender as ComboBox;
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var state = combobox.SelectedItem as TorchLightState?;
            System.Diagnostics.Trace.WriteLine($"TorchLight:{state}");
            m_WebCam.TorchLight.SetState(state ?? TorchLightState.OFF);
            sw.Stop();
            System.Diagnostics.Trace.WriteLine($"Set TorchLight:{sw.ElapsedMilliseconds}");
            System.Diagnostics.Trace.WriteLine($"get TorchLight:{m_WebCam.TorchLight.GetState()}");

        }

        async private void combobox_recordformat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await this.m_WebCam.SetMediaStreamPropertiesAsync(this.m_MainUI.RecordFormat);
        }



        private void combobox_colortempature_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(e.RemovedItems.Count >0 && e.AddedItems.Count>0)
            {
                this.m_WebCam.WhiteBalanceControl.Preset = this.m_MainUI.ColorTemperaturePreset;

            }
        }

        async private void combobox_cameras_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var combobox = sender as ComboBox;
            if(combobox.SelectedItem is Tuple<string, string, CameraPanel> camera)
            {
                var webcam = WebCam_MF.CreateFromSymbollink(camera.Item2);
                await this.OpenCamera(webcam, camera.Item3);
            }
        }

        private void combobox_eyegazecorrection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(m_WebCam.EyeGazeCorrection.IsSupported)
            {
                m_WebCam.EyeGazeCorrection.SetState(this.m_MainUI.EyeGazeCorrectionState);
            }
        }

        private void combobox_digitalwindow_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (m_WebCam.DigitalWindow.IsSupported)
            {
                m_WebCam.DigitalWindow.SetState(this.m_MainUI.DigitalWindowState);
            }
        }

        private void combobox_backgroundsegmentation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(this.m_WebCam.BackgroundSegmentation.IsSupported)
            {
                this.m_WebCam.BackgroundSegmentation.SetState(this.m_MainUI.BackgroundSegmentation);
            }
        }
    }



    public class MainUI:INotifyPropertyChanged
    {
        public VideoAmpVM WhiteBalance { set; get; } = new VideoAmpVM();
        bool m_IsSupportFlash;
        bool m_IsSupportTorch;
        ImageEncodingProperties m_RecordFormat;
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
        public ImageEncodingProperties RecordFormat
        {
            set { m_RecordFormat = value; this.Update("RecordFormat"); }
            get => m_RecordFormat;
        }
        FlashState m_FlashState;
        public FlashState FlashLight
        {
            set { m_FlashState = value; this.Update("FlashLight"); }
            get => m_FlashState;
        }
        public ObservableCollection<FlashState> FlashLights { set; get; } = new ObservableCollection<FlashState>();
        TorchLightState m_Torch;
        public TorchLightState Torch
        {
            set { m_Torch = value; this.Update("Torch"); }
            get => m_Torch;
        }
        public ObservableCollection<TorchLightState> Torchs { set; get; } = new ObservableCollection<TorchLightState>();

        ColorTemperaturePreset m_ColorTemperaturePreset;
        public ColorTemperaturePreset ColorTemperaturePreset
        {
            set { m_ColorTemperaturePreset = value; this.Update("ColorTemperaturePreset"); }
            get => m_ColorTemperaturePreset;
        }
        public bool UseD3D { set; get; }
        public ObservableCollection<ColorTemperaturePreset> ColorTemperaturePresets { set; get; } = new ObservableCollection<ColorTemperaturePreset>();

        public ObservableCollection<ImageEncodingProperties> VideoCaptureFormats { set; get; }=new ObservableCollection<ImageEncodingProperties>();
        public ObservableCollection<ImageEncodingProperties> PhotoDependentFormats { set; get; } = new ObservableCollection<ImageEncodingProperties>();
        public ObservableCollection<Tuple<string , string, QSoft.DevCon.CameraPanel>> Cameras { set; get; } = new ObservableCollection<Tuple<string, string, CameraPanel>>();

        public ObservableCollection<EyeGazeCorrectionState> EyeGazeCorrectionStates { set; get; } = new ObservableCollection<EyeGazeCorrectionState>();
        EyeGazeCorrectionState m_EyeGazeCorrectionState;
        public EyeGazeCorrectionState EyeGazeCorrectionState
        {
            set { m_EyeGazeCorrectionState = value; this.Update("EyeGazeCorrectionState"); }
            get => m_EyeGazeCorrectionState;
        }

        public ObservableCollection<DigitalWindowState> DigitalWindowStates { set; get; } = new ObservableCollection<DigitalWindowState>();
        DigitalWindowState m_DigitalWindowState;
        public DigitalWindowState DigitalWindowState
        {
            set { m_DigitalWindowState = value; this.Update("DigitalWindowState"); }
            get => m_DigitalWindowState;
        }

        public ObservableCollection<BackgroundSegmentationState> BackgroundSegmentations { set; get; } = new ObservableCollection<BackgroundSegmentationState>();
        BackgroundSegmentationState m_BackgroundSegmentation;
        public BackgroundSegmentationState BackgroundSegmentation
        {
            set { m_BackgroundSegmentation = value; this.Update("BackgroundSegmentation"); }
            get => m_BackgroundSegmentation;
        }

        public ObservableCollection<QSoft.MediaCapture.Legacy.AMCameraControl> CameraControls { set; get; } =  new ObservableCollection<QSoft.MediaCapture.Legacy.AMCameraControl>();
        public ObservableCollection<QSoft.MediaCapture.Legacy.AMVideoProcAmp> VideoProcAmps { set; get; } = new ObservableCollection<QSoft.MediaCapture.Legacy.AMVideoProcAmp>();

        public event PropertyChangedEventHandler PropertyChanged;
        void Update(string name)=> this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        //public ObservableCollection<WebCam_MF> WebCams { set; get; } = new ObservableCollection<WebCam_MF>();
    }

    public enum VideoAmpState
    {
        Auto,
        Manual
    }

    public class VideoAmpVM: INotifyPropertyChanged
    {
        int m_Max;
        int m_Min;
        int m_Step;
        int m_Value;
        bool m_IsAuto;
        public int Max
        {
            get => m_Max;
            set { m_Max = value; this.Update("Max"); }
        }
        public int Min
        {
            get => m_Min;
            set { m_Min = value; this.Update("Min"); }
        }
        public int Step
        {
            get => m_Step;
            set { m_Step = value; this.Update("Step"); }
        }
        public int Value
        {
            get => m_Value;
            set { m_Value = value; this.Update("Value"); }
        }
        public bool IsAuto
        {
            get => m_IsAuto;
            set { m_IsAuto = value; this.Update("IsAuto"); }
        }

        
        public event PropertyChangedEventHandler PropertyChanged;
        void Update(string name) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    }
}
