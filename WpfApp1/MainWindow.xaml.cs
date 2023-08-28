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
            //var attribute = MFFunctions.MFCreateAttributes();
            ////hr = pAttributes->SetGUID(MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE,
            ////    MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_GUID);
            //attribute.Set(MFConstants.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE, MFConstants.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_GUID);
            //var sources = attribute.EnumDeviceSources();
            //WebCam_MF mf = new WebCam_MF();
            //mf.InitializeCaptureManager(IntPtr.Zero, sources.ElementAt(0));
            //mf.StartPreview();
            //MFFunctions.MFStartup();
            //var capturefac = Activator.CreateInstance(Type.GetTypeFromCLSID(DirectN.MFConstants.CLSID_MFCaptureEngineClassFactory)) as IMFCaptureEngineClassFactory;
            //object o;
            //capturefac.CreateInstance(DirectN.MFConstants.CLSID_MFCaptureEngine, typeof(IMFCaptureEngine).GUID, out o);
            //var em = o as IMFCaptureEngine;

            //var hr = CreateDX11Device(out g_pDX11Device, out m_pDeviceContext, out var level);

        }
        MainUI m_MainUI;
        WebCam_MF mf = new WebCam_MF();
        async private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(this.m_MainUI == null)
            {
                //var attribute = MFFunctions.MFCreateAttributes();
                //attribute.Set(MFConstants.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE, MFConstants.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_GUID);
                //var cameras = attribute.EnumDeviceSources().Select(x => new { obj = x, name = x.GetString(MFConstants.MF_DEVSOURCE_ATTRIBUTE_FRIENDLY_NAME) }).ToList();
                //foreach (var oo in cameras)
                //{
                //    System.Diagnostics.Trace.WriteLine(oo);
                //    WebCam_MF mf = new WebCam_MF();
                //    await mf.InitializeCaptureManager(oo.obj.Object);
                //    foreach(var video in mf.VideoFormats)
                //    {
                //        System.Diagnostics.Trace.WriteLine($"{video.Key}");
                //        foreach (var item in video.Value.GroupBy(x => x.format))
                //        {
                //            System.Diagnostics.Trace.WriteLine($"{item.Key}");
                //            foreach (var format in item)
                //            {
                //                System.Diagnostics.Trace.WriteLine($"{format}");
                //            }

                //        }
                //    }
                //    mf.Dispose();
                //    //await mf.StartPreview(x => mf.Preview = x);
                //}
                this.DataContext = this.m_MainUI = new MainUI();

                //var attribute = MFFunctions.MFCreateAttributes();
                //attribute.Set(MFConstants.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE, MFConstants.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_GUID);
                //var sources = attribute.EnumDeviceSources().ToList();
                //IMFActivate ddd = sources[0].Object;
                IMFActivate ddd = WebCam_MF.EnumDeviceSources()[0].obj.Object;
                mf.OnFail += (WebCam_MF obj, int error) =>
                {

                };
                await mf.InitializeCaptureManager(ddd, true);
                //var vvsq = mf.VideoFormats[MF_CAPTURE_ENGINE_STREAM_CATEGORY.MF_CAPTURE_ENGINE_STREAM_CATEGORY_VIDEO_CAPTURE]
                //    .OrderByDescending(x => x.width * x.height)
                //    .ThenByDescending(x => x.fps)
                //    .ThenBy(x => x.bitrate)
                //    .GroupBy(x => new { x.width, x.height });

                //foreach (var group in vvsq)
                //{
                //    System.Diagnostics.Trace.WriteLine($"key:{group.Key}");
                //    foreach (var v in group)
                //    {
                //        System.Diagnostics.Trace.WriteLine($"format:{v.format_str} {v.width} {v.height} {v.fps} {v.bitrate}");
                //    }
                //}

                //System.Diagnostics.Trace.WriteLine("-----");
                //foreach(var v in vvsq.Select(x=>x.First()))
                //{
                //    System.Diagnostics.Trace.WriteLine($"format:{v.format_str} {v.width} {v.height} {v.fps} {v.bitrate}");
                //}
                //if (mf.VideoFormats.ContainsKey(MF_CAPTURE_ENGINE_STREAM_CATEGORY.MF_CAPTURE_ENGINE_STREAM_CATEGORY_PHOTO_DEPENDENT))
                //{
                //    var vvs = mf.VideoFormats[MF_CAPTURE_ENGINE_STREAM_CATEGORY.MF_CAPTURE_ENGINE_STREAM_CATEGORY_PHOTO_DEPENDENT]
                //        .GroupBy(x => x.format, (x, y) => new { list=y.OrderByDescending(z => z.width) }) ;
                //    foreach(var oo in vvs)
                //    {
                        
                //    }
                //    foreach (var oo in vvs.SelectMany(x => x.list))
                //    {
                //        var name = oo.format.FormatToString();
                //        this.m_MainUI.PhotoFormats.Add(new VideoFormat() {Format= name, Width =oo.width, Height = oo.height });
                //    }
                    
                //}
                //if(mf.VideoFormats.ContainsKey(MF_CAPTURE_ENGINE_STREAM_CATEGORY.MF_CAPTURE_ENGINE_STREAM_CATEGORY_VIDEO_CAPTURE))
                //{
                //    var vvs = mf.VideoFormats[MF_CAPTURE_ENGINE_STREAM_CATEGORY.MF_CAPTURE_ENGINE_STREAM_CATEGORY_VIDEO_CAPTURE]
                //        .GroupBy(x => x.format, (x, y) => new { list = y.OrderByDescending(z => z.width).ThenBy(z=>z.fps) });
                //    foreach (var oo in vvs.SelectMany(x => x.list))
                //    {
                //        var name = oo.format.FormatToString();

                //        if(mf.VideoFormats.ContainsKey(MF_CAPTURE_ENGINE_STREAM_CATEGORY.MF_CAPTURE_ENGINE_STREAM_CATEGORY_PHOTO_DEPENDENT)==false)
                //        {
                //            this.m_MainUI.PhotoFormats.Add(new VideoFormat() { Format = name, Width = oo.width, Height = oo.height });
                //        }
                        
                //        this.m_MainUI.RecordFormats.Add(new VideoFormat() { Format = name, Width = oo.width, Height = oo.height, FPS = oo.fps, Bitrate=oo.bitrate });

                //    }

                //    //var view = new CollectionViewSource();
                //    //view.GroupDescriptions.Add(new PropertyGroupDescription("Format"));
                //    //view.Source = this.m_MainUI.RecordFormats;
                //    //this.m_MainUI.CollectionView = view;
                //}
                ////await mf.StartPreview(this.mtbDate.Handle);
                
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
