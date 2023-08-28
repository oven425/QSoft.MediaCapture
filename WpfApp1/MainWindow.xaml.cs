using DirectN;
using QSoft.MediaCapture;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
        }
        MainUI m_MainUI;
        WebCam_MF mf = new WebCam_MF();
        async private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(this.m_MainUI == null)
            {
                this.DataContext = this.m_MainUI = new MainUI();

                IMFActivate ddd = WebCam_MF.EnumDeviceSources()[0].obj.Object;
                mf.OnFail += (WebCam_MF obj, int error) =>
                {

                };
                await mf.InitializeCaptureManager(ddd, new Setting()
                {
                    Mirror = true,
                });

                
                foreach(var oo in mf.RecordForamts)
                {
                    this.m_MainUI.RecordFormats.Add(new VideoFormat() { Format = oo.format_str, Width = oo.width, Height = oo.height, FPS = oo.fps, Bitrate = oo.bitrate });
                }
                foreach (var oo in mf.PhotoForamts)
                {
                    this.m_MainUI.PhotoFormats.Add(new VideoFormat() { Format = oo.format_str, Width = oo.width, Height = oo.height, FPS = oo.fps, Bitrate = oo.bitrate });
                }
                await mf.StartPreview(x => { this.image_preview.Source = x; });
            }
        }


        int m_JpgIndex = 0;
        async private void button_takephoto_Click(object sender, RoutedEventArgs e)
        {
            var photo = System.IO.Path.Combine(Environment.CurrentDirectory, "aa.jpg");
            
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            //await mf.TakePhoto($"{m_JpgIndex++}.jpg", ()=>(width: 1280, height:720));
            sw.Stop();
            System.Diagnostics.Trace.WriteLine($"takephoto: {sw.ElapsedMilliseconds}ms");
        }

        async private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton toggle = sender as ToggleButton;
            if(toggle.IsChecked == true)
            {
                await mf.StartRecord("aa.mp4");
            }
            else
            {
                await mf.StopRecord();
            }
        }

        private void radiobutton_photo_Click(object sender, RoutedEventArgs e)
        {
            
            var aa = WebCam_MF.EnumDeviceSources()[0];
            WebCam_MF webcam = new WebCam_MF();
            //await webcam.InitializeCaptureManager(aa, true);
            //webcam.StartPreview();
        }

        private void radiobutton_record_Click(object sender, RoutedEventArgs e)
        {

        }
    }

    public enum Modes
    {
        Photo,
        Record
    }
    public class MainUI
    {
        public CollectionViewSource CollectionView { get; set; }
        public Modes Mode { set; get; } = Modes.Photo;
        public ObservableCollection<string> Cameras { set; get; } = new ObservableCollection<string>();
        public ObservableCollection<VideoFormat> PhotoFormats { set; get; } = new ObservableCollection<VideoFormat>();
        public ObservableCollection<VideoFormat> RecordFormats { set; get; } = new ObservableCollection<VideoFormat>();
    }


    public class VideoFormat
    {
        public string Format { set; get; }
        public uint Width { set; get; }
        public uint Height { set; get; }
        public double FPS { set; get; }
        public uint Bitrate { set; get; }
    }
}
