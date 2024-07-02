using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        }

        MainUI m_MainUI;
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //var yuv = new YUVRender();
            //yuv.Init(720, 404);
            //D3DImage d3dimg = new D3DImage();
            //this.image.Source = d3dimg;
            //d3dimg.Lock();
            //var ptr = Marshal.GetIUnknownForObject(yuv.BackBuffer);
            //d3dimg.SetBackBuffer(D3DResourceType.IDirect3DSurface9, ptr);
            //d3dimg.Unlock();

            //var buf = System.IO.File.ReadAllBytes("720-404-nv12.yuv");
            //var src = Marshal.AllocHGlobal(buf.Length);
            //Marshal.Copy(buf, 0, src, buf.Length);
            //yuv.WriteFrame(src, YUVRender.D3DFMT_NV12);
            ////yuv.NV12(src);
            //d3dimg.Lock();
            //d3dimg.AddDirtyRect(new Int32Rect(0, 0, 720, 404));
            //d3dimg.Unlock();

            //return;
            if (m_MainUI == null)
            {
                this.DataContext = this.m_MainUI = new MainUI();
                foreach (var oo in QSoft.MediaCapture.WebCam_MF.GetAllWebCams())
                {
                    System.Diagnostics.Trace.WriteLine(oo.FriendName);
                    System.Diagnostics.Trace.WriteLine(oo.SymbolLinkName);
                    await oo.InitCaptureEngine();
                    //await oo.StartPreview(host.Child.Handle);
                    //await oo.StartPreview(new Action<System.Windows.Interop.D3DImage>((x) => 
                    //{
                    //    //this.image.Source = x;
                    //}), System.Windows.Threading.DispatcherPriority.Render);
                    await oo.StartPreview(new Action<WriteableBitmap>((x) =>
                    {
                        this.image.Source = x;
                    }));


                    //await oo.StartPreview(new Action<D3DImage>((x) =>
                    //{
                    //    this.image.Source = x;
                    //}));
                    //this.m_MainUI.WebCams.Add(oo);
                }
                //var camera = QSoft.MediaCapture.WebCam_MF.GetAllWebCams().Find(x => x.FriendName == "USB2.0 HD UVC WebCam");
                //if(camera != null)
                //{
                //    await camera.InitCaptureEngine();
                //    //await camera.StartPreview(host.Child.Handle);
                //    await camera.SetPreviewSize(x => x.ElementAt(1));
                //    await camera.StartPreview(x => this.image.Source = x,null);
                //}

                //var tasks = QSoft.MediaCapture.WebCam_MF.GetAllWebCams()
                //    .Select((camera, i) => Task.Run(async() =>
                //    {
                //        await camera.InitCaptureEngine();
                //        //await camera.SetPreviewSize(x => x.ElementAt(1));
                //        await camera.StartPreview(x =>
                //        {

                //        });
                //    })).ToArray();
                //await Task.WhenAll(tasks);

                //var aa = QSoft.MediaCapture.WebCam_MF.GetAllWebCams()
                //    .Select(async (camera, i) =>
                //    {
                //        await camera.InitCaptureEngine();
                //        await camera.SetPreviewSize(x => x.FirstOrDefault(y => y.Width >= 640));
                //        await camera.StartPreview(x =>
                //        {
                //            this.image.Source = x;
                //        });
                //    }).ToArray();


                try
                {
                    //var aa1 = QSoft.MediaCapture.WebCam_MF.GetAllWebCams()
                    //    .Select(async (camera, i) =>
                    //    {
                    //        await Task.Run(async () =>
                    //        {
                    //            await camera.InitCaptureEngine();
                    //            await camera.SetPreviewSize(x => x.ElementAt(1));
                    //            await camera.StartPreview(x =>
                    //            {
                    //                this.Dispatcher.Invoke(() =>
                    //                {
                    //                    this.image.Source = x;
                    //                });

                    //            });
                                
                    //        });
                    //        return camera;
                    //        //this.m_MainUI.WebCams.Add(camera);
                    //    }).ToArray();

                    //var io = await Task.WhenAll(aa1);
                    //this.m_MainUI.WebCams.AddRange(io);
                }
                catch (Exception ee)
                {

                }


            }
        }

        private void button_stoppreview_Click(object sender, RoutedEventArgs e)
        {
            foreach(var oo in this.m_MainUI.WebCams)
            {
                oo?.StopPreview();
            }
        }

        private void button_stratpreivew_Click(object sender, RoutedEventArgs e)
        {
            foreach (var oo in this.m_MainUI.WebCams)
            {
                oo?.StartPreview(host.Child.Handle);
            }
        }

        private void picture_SizeChanged(object sender, EventArgs e)
        {
            if (this.m_MainUI == null) return;
            System.Windows.Forms.Control aa = sender as System.Windows.Forms.Control;
            foreach (var oo in this.m_MainUI.WebCams)
            {
                oo.UpdateVideo((int)aa.Width, (int)aa.Height);
            }
        }

        async private void button_takephoto_Click(object sender, RoutedEventArgs e)
        {
            //foreach (var oo in this.m_MainUI.WebCams)
            //{
            //    await oo?.TakePhoto("123.bmp");
            //}
        }
    }

    public class MainUI
    {
        public ObservableCollection<WebCam_MF> WebCams { set; get; } = new ObservableCollection<WebCam_MF>();
    }
}
