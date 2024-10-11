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
using System.Threading.Tasks;
using System.Threading;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Media.Capture.Frames;
using Windows.Media.MediaProperties;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App1
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

        CaptureElement m_CaptureElement;
        private async void myButton_Click(object sender, RoutedEventArgs e)
        {
            this.m_CaptureElement = new CaptureElement(this.mediaelement);
            await this.m_CaptureElement.InitAsync();
            //CameraProvider cp = new CameraProvider();
            //CancellationTokenSource cts = new CancellationTokenSource();
            //await cp.RefreshAvailableCameras(cts.Token);
            //MediaCapture mm = new MediaCapture();
            //mm.SetPreviewMirroring(true);

        }

        private async void button_startrecord_Click(object sender, RoutedEventArgs e)
        {
            await m_CaptureElement.StartRecordAsync();
        }

        private void button_stoprecord_Click(object sender, RoutedEventArgs e)
        {

        }

        private void mirror_Toggled(object sender, RoutedEventArgs e)
        {
            var ts = sender as ToggleSwitch;
            m_CaptureElement.Mirror(true);
        }
    }

    public class CameraProvider
    {
        public async ValueTask RefreshAvailableCameras(CancellationToken token)
        {
            var deviceInfoCollection = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture).AsTask(token);
            var mediaFrameSourceGroup = await MediaFrameSourceGroup.FindAllAsync().AsTask(token);
            var videoCaptureSourceGroup = mediaFrameSourceGroup.Where(sourceGroup => deviceInfoCollection.Any(deviceInfo => deviceInfo.Id == sourceGroup.Id)).ToList();
            var mediaCapture = new MediaCapture();

            //var availableCameras = new List<CameraInfo>();

            foreach (var sourceGroup in videoCaptureSourceGroup)
            {
                await mediaCapture.InitializeCameraForCameraView(sourceGroup.Id, token);

                //CameraPosition position = CameraPosition.Unknown;
                //var device = deviceInfoCollection.FirstOrDefault(deviceInfo => deviceInfo.Id == sourceGroup.Id);
                //if (device?.EnclosureLocation is not null)
                //{
                //    position = device.EnclosureLocation.Panel switch
                //    {
                //        Panel.Front => CameraPosition.Front,
                //        Panel.Back => CameraPosition.Rear,
                //        _ => CameraPosition.Unknown
                //    };
                //}
                var videorecords = mediaCapture.VideoDeviceController.GetAvailableMediaStreamProperties(MediaStreamType.VideoRecord);
                var mediaEncodingPropertiesList = mediaCapture.VideoDeviceController.GetAvailableMediaStreamProperties(MediaStreamType.Photo)
                    .OfType<ImageEncodingProperties>().OrderByDescending(p => p.Width * p.Height);

                var supportedResolutionsList = new List<Size>();
                var imageEncodingPropertiesList = new List<ImageEncodingProperties>();

                foreach (var mediaEncodingProperties in mediaEncodingPropertiesList)
                {
                    var imageEncodingProperties = mediaEncodingProperties;
                    if (supportedResolutionsList.Contains(new(imageEncodingProperties.Width, imageEncodingProperties.Height)))
                    {
                        continue;
                    }
                    imageEncodingPropertiesList.Add(imageEncodingProperties);
                    supportedResolutionsList.Add(new(imageEncodingProperties.Width, imageEncodingProperties.Height));
                }

                //var cameraInfo = new CameraInfo(
                //    sourceGroup.DisplayName,
                //    sourceGroup.Id,
                //    position,
                //    mediaCapture.VideoDeviceController.FlashControl.Supported,
                //    mediaCapture.VideoDeviceController.ZoomControl.Supported ? mediaCapture.VideoDeviceController.ZoomControl.Min : 1f,
                //    mediaCapture.VideoDeviceController.ZoomControl.Supported ? mediaCapture.VideoDeviceController.ZoomControl.Max : 1f,
                //    supportedResolutionsList,
                //    sourceGroup,
                //    imageEncodingPropertiesList);

                //availableCameras.Add(cameraInfo);
            }

            //AvailableCameras = availableCameras;
        }

    }


    static class CameraViewExtensions
    {
        public static async Task UpdateAvailability(CancellationToken token)
        {
            var videoCaptureDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture).AsTask(token);

            //cameraView.IsAvailable = videoCaptureDevices.Count > 0;
        }

        public static Task InitializeCameraForCameraView(this MediaCapture mediaCapture, string deviceId, CancellationToken token)
        {
            try
            {
                return mediaCapture.InitializeAsync(new MediaCaptureInitializationSettings
                {
                    VideoDeviceId = deviceId,
                    PhotoCaptureSource = PhotoCaptureSource.Auto
                }).AsTask(token);
            }
            catch (System.Runtime.InteropServices.COMException)
            {
                // Camera already initialized
                return Task.CompletedTask;
            }
        }
    }

}
