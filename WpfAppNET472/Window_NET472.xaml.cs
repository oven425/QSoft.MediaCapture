﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
            if (m_MainUI == null)
            {
                this.DataContext = this.m_MainUI = new MainUI();
                //foreach(var oo in QSoft.MediaCapture.WebCam_MF.GetAllWebCams())
                //{
                //    System.Diagnostics.Trace.WriteLine(oo.FriendName);
                //    System.Diagnostics.Trace.WriteLine(oo.SymbolLinkName);
                //    //await oo.InitCaptureEngine();
                //    ////await oo.StartPreview(host.Child.Handle);
                //    //await oo.StartPreview(x => this.image.Source = x);
                //    //this.m_MainUI.WebCams.Add(oo);
                //}
                var camera = QSoft.MediaCapture.WebCam_MF.GetAllWebCams().Find(x => x.FriendName == "USB2.0 HD UVC WebCam");
                if(camera != null)
                {
                    await camera.InitCaptureEngine();
                    //await camera.StartPreview(host.Child.Handle);
                    await camera.StartPreview(x => this.image.Source = x);
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
    }

    public class MainUI
    {
        public ObservableCollection<WebCam_MF> WebCams { set; get; } = new ObservableCollection<WebCam_MF>();
    }
}