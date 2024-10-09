using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Runtime.Versioning;

namespace MauiApp1
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
        public CancellationToken Token => CancellationToken.None;
        private async void button_snapshot_Clicked(object sender, EventArgs e)
        {
            await this.cameraview.CaptureImage(Token);
        }

        int m_ImageCount = 1;
        private void cameraview_MediaCaptured(object sender, CommunityToolkit.Maui.Views.MediaCapturedEventArgs e)
        {
            
            var imagePath = Path.Combine(FileSystem.CacheDirectory, $"{m_ImageCount++}.jpg");
            using var localFileStream = File.Create(imagePath);

            e.Media.CopyTo(localFileStream);

        }
    }


}
