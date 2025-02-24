using DirectN;
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
    /// BarcodeView.xaml 的互動邏輯
    /// </summary>
    public partial class BarcodeView : System.Windows.Controls.UserControl
    {
        public static readonly DependencyProperty CameraSourceProperty = DependencyProperty.Register("CameraSource", typeof(QSoft.MediaCapture.WebCam_MF), typeof(CameraView), new PropertyMetadata(CameraSourcePropertyChanged));

        public BarcodeView()
        {
            InitializeComponent();
        }

        public QSoft.MediaCapture.WebCam_MF CameraSource
        {
            get { return (QSoft.MediaCapture.WebCam_MF)GetValue(CameraSourceProperty); }
            set { SetValue(CameraSourceProperty, value); }
        }
        static async void CameraSourcePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var view = obj as BarcodeView;
            if (view is null) return;
            if (e.NewValue is QSoft.MediaCapture.WebCam_MF camera)
            {
                await camera.StartPreview(x=> view.img.Source = x);
            }
            else if (e.NewValue is null && e.OldValue is QSoft.MediaCapture.WebCam_MF oldcamera)
            {
                await oldcamera.StopPreview();
            }
        }
    }
}
