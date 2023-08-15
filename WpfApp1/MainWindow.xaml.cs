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
                var attribute = MFFunctions.MFCreateAttributes();
                attribute.Set(MFConstants.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE, MFConstants.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_GUID);
                var cameras = attribute.EnumDeviceSources().Select(x => new { obj = x, name = x.GetString(MFConstants.MF_DEVSOURCE_ATTRIBUTE_FRIENDLY_NAME) }).ToList();
                foreach (var oo in cameras)
                {
                    System.Diagnostics.Trace.WriteLine(oo);
                    WebCam_MF mf = new WebCam_MF();
                    await mf.InitializeCaptureManager(oo.obj.Object);
                    foreach(var video in mf.VideoFormats)
                    {
                        System.Diagnostics.Trace.WriteLine($"{video.Key}");
                        foreach(var item in video.Value.GroupBy(x=>x.format))
                        {
                            System.Diagnostics.Trace.WriteLine($"{item.Key}");
                            foreach(var format in item)
                            {
                                System.Diagnostics.Trace.WriteLine($"{format}");
                            }
                            
                        }
                    }
                    
                    //await mf.StartPreview(x => mf.Preview = x);
                }
                this.DataContext = this.m_MainUI = new MainUI();

                //var attribute = MFFunctions.MFCreateAttributes();
                //attribute.Set(MFConstants.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE, MFConstants.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_GUID);
                //var sources = attribute.EnumDeviceSources().ToList();
                //IMFActivate ddd = sources[0].Object;
                
                //await mf.InitializeCaptureManager(ddd);
                ////await mf.StartPreview(this.mtbDate.Handle);
                //await mf.StartPreview(x => { this.image_preview.Source = x; });

            }
        }

        int m_JpgIndex = 0;
        async private void button_takephoto_Click(object sender, RoutedEventArgs e)
        {
            var photo = System.IO.Path.Combine(Environment.CurrentDirectory, "aa.jpg");
            
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            await mf.TakePhoto($"{m_JpgIndex++}.jpg", ()=>(1280,720));
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
    }


    public class MainUI
    {
        public ObservableCollection<string> Cameras { set; get; } = new ObservableCollection<string>();
    }

    
    public class SampleRecv: IMFCaptureEngineOnSampleCallback
    {
        int m_Data;
        public SampleRecv(int data)
        {
            this.m_Data = data;
        }
        public HRESULT OnSample(IMFSample pSample)
        {
            System.Diagnostics.Trace.WriteLine($"SampleRecv:{this.m_Data}");
            Marshal.ReleaseComObject(pSample);
            return HRESULTS.S_OK;
        }
    }
}
