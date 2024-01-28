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
        WebCam_MF mf = null;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(this.m_MainUI == null)
            {
                this.DataContext = this.m_MainUI = new MainUI();
                var webcams = WebCam_MF.EnumDeviceSources();
                this.m_MainUI.WebCams = new ObservableCollection<WebCam_MF>(webcams);
                //IMFActivate ddd = WebCam_MF.EnumDeviceSources()[0].obj.Object;
                //mf.OnFail += (WebCam_MF obj, int error) =>
                //{

                //};
                //await mf.InitializeCaptureManager(ddd, new Setting()
                //{
                //    Mirror = true,
                //});

                
                //foreach(var oo in mf.RecordForamts)
                //{
                //    this.m_MainUI.RecordFormats.Add(new VideoFormat() { Format = oo.format_str, Width = oo.width, Height = oo.height, FPS = oo.fps, Bitrate = oo.bitrate });
                //}
                //foreach (var oo in mf.PhotoForamts)
                //{
                //    this.m_MainUI.PhotoFormats.Add(new VideoFormat() { Format = oo.format_str, Width = oo.width, Height = oo.height, FPS = oo.fps, Bitrate = oo.bitrate });
                //}
                //await mf.StartPreview(x => { this.image_preview.Source = x; });
            }
        }


        int m_JpgIndex = 0;
        async private void button_takephoto_Click(object sender, RoutedEventArgs e)
        {
            var photo = System.IO.Path.Combine(Environment.CurrentDirectory, "aa.jpg");

            var vd = combobox_photos.SelectedItem as VideoFormat;
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            if(vd == null)
            {
                await mf.TakePhoto($"{m_JpgIndex++}.jpg");
            }
            else
            {
                await mf.TakePhoto($"{m_JpgIndex++}.jpg", vd.Width, vd.Height);
            }
           
            sw.Stop();
            System.Diagnostics.Trace.WriteLine($"takephoto: {sw.ElapsedMilliseconds}ms");
        }




        private async void combobox_webcams_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.m_MainUI.RecordFormats.Clear();
            this.m_MainUI.PhotoFormats.Clear();
            var combobox = sender as ComboBox;
            this.mf = combobox.SelectedItem as WebCam_MF;
            await this.mf.InitializeCaptureManager(new Setting() { Mirror = this.m_MainUI.IsMirror});
            foreach (var oo in mf.RecordForamts)
            {
                this.m_MainUI.RecordFormats.Add(new VideoFormat() { Format = oo.format_str, Width = oo.width, Height = oo.height, FPS = oo.fps, Bitrate = oo.bitrate });
            }
            foreach (var oo in mf.PhotoForamts)
            {
                this.m_MainUI.PhotoFormats.Add(new VideoFormat() { Format = oo.format_str, Width = oo.width, Height = oo.height, FPS = oo.fps, Bitrate = oo.bitrate });
            }
            //slider_brightness.Maximum = mf.Brigtness.Max;
            //slider_brightness.Minimum = mf.Brigtness.Min;
            //slider_brightness.Value = mf.Brigtness.Value;
        }

        async private void button_startpreview_Click(object sender, RoutedEventArgs e)
        {
            //await mf?.StartPreview(x => this.image_preview.Source = x);
            await mf?.StartPreview(mtbDate.Handle);
            mf.Mirror();
            await Task.Delay(5000);
            
            //mf.SetSource(0, 0, 2560, 1440);
            
        }

        private async void button_stoppreview_Click(object sender, RoutedEventArgs e)
        {
            await mf?.StopPreview();
        }

        private async void button_startrecord_Click(object sender, RoutedEventArgs e)
        {
            var filename = $"{mf?.Name}_{DateTime.Now.ToString("HHmmss")}.mp4";
            await mf?.StartRecord(filename);
        }

        async private void button_stoprecord_Click(object sender, RoutedEventArgs e)
        {
            await mf?.StopRecord();
        }

        private void WindowsFormsHost_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //mf?.UpdateVideo((int)e.NewSize.Width, (int)e.NewSize.Height);
        }

        private void mtbDate_SizeChanged(object sender, EventArgs e)
        {
            System.Windows.Forms.Control aa = sender as System.Windows.Forms.Control;
            mf?.UpdateVideo((int)aa.Width, (int)aa.Height);
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double w = 1280;
            double h = 720;
            var dw = w / e.NewValue;
            var dh = h / e.NewValue;
            var dx = (w - dw) / 2;
            var dy = (h - dh) / 2;
            mf?.SetDestination((int)dx, (int)dy, (int)dw, (int)dh);
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
        public ObservableCollection<WebCam_MF> WebCams { set; get; } = new ObservableCollection<WebCam_MF>();
        public ObservableCollection<VideoFormat> PhotoFormats { set; get; } = new ObservableCollection<VideoFormat>();
        public ObservableCollection<VideoFormat> RecordFormats { set; get; } = new ObservableCollection<VideoFormat>();
        public bool IsMirror { set; get; } = true;
        public RECT Source{set;get;} = new RECT() { left=0, top=0, right=100, bottom=100};
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
