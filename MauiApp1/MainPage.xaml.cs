using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiApp1
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
    }

    public partial class CameraViewVM(ICameraProvider cameraProvider) :ObservableObject
    {
        [ObservableProperty]
        CameraInfo? selectedCamera;

        [ObservableProperty]
        Size selectedResolution;
    }

}
