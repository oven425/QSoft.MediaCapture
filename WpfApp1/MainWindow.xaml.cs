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
        WebCam_MF mf;
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
            await this.mf.InitializeCaptureManager(this.mf.VideoDevice.Object, new Setting() { Mirror = this.m_MainUI.IsMirror});
            foreach (var oo in mf.RecordForamts)
            {
                this.m_MainUI.RecordFormats.Add(new VideoFormat() { Format = oo.format_str, Width = oo.width, Height = oo.height, FPS = oo.fps, Bitrate = oo.bitrate });
            }
            foreach (var oo in mf.PhotoForamts)
            {
                this.m_MainUI.PhotoFormats.Add(new VideoFormat() { Format = oo.format_str, Width = oo.width, Height = oo.height, FPS = oo.fps, Bitrate = oo.bitrate });
            }
        }

        async private void button_startpreview_Click(object sender, RoutedEventArgs e)
        {
            await mf?.StartPreview(x =>
            {
                this.image_preview.Source = x;
            });
        }

        private void button_stoppreview_Click(object sender, RoutedEventArgs e)
        {
            mf?.StopPreview();
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
        public bool IsMirror { set; get; }
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
