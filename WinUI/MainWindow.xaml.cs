using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media;
using Windows.Media.Capture;
using Windows.Media.Capture.Frames;
using Windows.Media.Core;
using Windows.Media.MediaProperties;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUI
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

        //https://www.dynamsoft.com/codepool/winui-barcode-scanner.html
        //https://github.com/microsoft/microsoft-ui-xaml/issues/4710
        bool m_IsLoad = false;
        DeviceInformation _cameraDevice;
        private async void Window_Activated(object sender, WindowActivatedEventArgs args)
        {
            if(m_IsLoad == false)
            {
                m_IsLoad=true;
                return;
            }
            var allVideoDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
            DeviceInformation desiredDevice = allVideoDevices.FirstOrDefault(x => x.EnclosureLocation != null
                && x.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Front);
            _cameraDevice = desiredDevice ?? allVideoDevices.FirstOrDefault();
            var settings = new MediaCaptureInitializationSettings 
            {
                StreamingCaptureMode = StreamingCaptureMode.Video,
                MediaCategory = MediaCategory.Other,
                AudioProcessing = AudioProcessing.Default,
                VideoDeviceId = _cameraDevice.Id 
            };
            var mediaCapture = new MediaCapture();
            await mediaCapture.InitializeAsync(settings);

            mediaPlayerElement.Source = MediaSource.CreateFromMediaFrameSource(mediaCapture.FrameSources.FirstOrDefault().Value);
            try
            {
                var profile = new MediaEncodingProfile
                {
                    Audio = null,
                    Video = VideoEncodingProperties.CreateUncompressed(MediaEncodingSubtypes.Rgb32, 640, 480),
                    Container = null
                };
                IMediaExtension hh = new Sink();
                await mediaCapture.StartPreviewToCustomSinkAsync(profile, hh);
            }
            catch(Exception e)
            {

            }
            
            //


            var video = mediaCapture.VideoDeviceController;
            var props = mediaCapture.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview);
            Guid RotationKey = new Guid("C380465D-2271-428C-9B83-ECEA3B4A85C1");
            props.Properties.Add(RotationKey, 90);
            try
            {
                await mediaCapture.SetEncodingPropertiesAsync(MediaStreamType.VideoPreview, props, null);

            }
            catch (Exception ex)
            {

            }


        }

        private void button_takephoto_Click(object sender, RoutedEventArgs e)
        {

        }
    }

    public class Sink : IMediaExtension
    {
        public void SetProperties(IPropertySet configuration)
        {
            throw new NotImplementedException();
        }
    }
}
