using DirectN;
using QSoft.MediaCapture.WPF;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp_D3DImage
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

        WebCamD3D9 m_WebCamD3D9Async;
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.m_WebCamD3D9Async = new WebCamD3D9(this.d3dimage, new WindowInteropHelper(System.Windows.Application.Current.MainWindow));
            m_WebCamD3D9Async.Start();
            await Task.Delay(3000);
            this.textblock_info.Text = $"{m_WebCamD3D9Async.m_Rect.right}x{m_WebCamD3D9Async.m_Rect.bottom}";
            //helper = new WindowInteropHelper(System.Windows.Application.Current.MainWindow);
            //InitD3D9Ex();
            //_ = Start();


            //this.Start();
        }
    }
}
