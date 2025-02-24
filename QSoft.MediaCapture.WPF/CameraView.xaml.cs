using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QSoft.MediaCapture.WPF
{
    /// <summary>
    /// CameraView.xaml 的互動邏輯
    /// </summary>
    public partial class CameraView : System.Windows.Controls.UserControl
    {
        public static readonly DependencyProperty PreviewSourceProperty = DependencyProperty.Register("PreviewSource", typeof(QSoft.MediaCapture.WebCam_MF), typeof(CameraView), new PropertyMetadata(CameraSourcePropertyChanged));
        public static readonly DependencyProperty CameraViewModeProperty = DependencyProperty.Register("CameraViewMode", typeof(CameraViewModes), typeof(CameraView), new PropertyMetadata(null));
        public CameraView()
        {
            InitializeComponent();
        }

        public QSoft.MediaCapture.WebCam_MF PreviewSource
        {
            get { return (QSoft.MediaCapture.WebCam_MF)GetValue(PreviewSourceProperty); }
            set { SetValue(PreviewSourceProperty, value); }
        }
        static async void CameraSourcePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var cameraview = obj as CameraView;
            if (e.NewValue is QSoft.MediaCapture.WebCam_MF camera)
            {
                
                //camera.StartPreview();
            }
            else if(e.NewValue is null && e.OldValue is QSoft.MediaCapture.WebCam_MF oldcamera)
            {
                await oldcamera.StopPreview();
            }
        }

        public CameraViewModes CameraViewMode
        {
            get { return (CameraViewModes)GetValue(CameraViewModeProperty); }
            set { SetValue(CameraViewModeProperty, value); }
        }

        public enum CameraViewModes
        {
            Handle,
            WriteableBitmap
        }
    }
}
