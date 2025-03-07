using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUI3
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }

        private async void myButton_Click(object sender, RoutedEventArgs e)
        {
            var webcams = QSoft.MediaCapture.WebCam_MF.GetAllWebCams();
            var webcam = webcams.FirstOrDefault();
            var hr = await webcam.InitCaptureEngine(new QSoft.MediaCapture.WebCam_MF_Setting());
            webcam.StartPreview(new AA());
            myButton.Content = "Clicked";
            MediaStreamSource streamSource = null;
            //MediaStreamSource
        }
    }

    public class AA:QSoft.MediaCapture.MFCaptureEngineOnSampleCallback
    {
        protected override void OnSample(nint data, uint len)
        {
            
        }
    }
}
