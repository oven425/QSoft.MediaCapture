using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
    public partial class CameraView : UserControl
    {
        readonly public static DependencyProperty PreviewSourceProperty = DependencyProperty.Register("PreviewSource", typeof(QSoft.MediaCapture.WebCam_MF), typeof(CameraView), new PropertyMetadata(null, PreviewSourcePropertyChange));
        [Category("CameraView")]
        public QSoft.MediaCapture.WebCam_MF PreviewSource
        {
            set=>SetValue(PreviewSourceProperty, value);
            get=> (QSoft.MediaCapture.WebCam_MF)GetValue(PreviewSourceProperty);
        }
        static async void PreviewSourcePropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(d is CameraView view)
            {
                if(e.NewValue is null && e.OldValue is QSoft.MediaCapture.WebCam_MF oldcam)
                {
                    
                    await oldcam.StopPreview();
                }
                else if(e.NewValue is QSoft.MediaCapture.WebCam_MF newcam)
                {
                    await newcam.StartPreview(IntPtr.Zero);
                }
            }
        }

        


        public CameraView()
        {
            InitializeComponent();
        }
    }

    public enum RenderMode
    {
        Handle,
        WritableBitmap
    }
}
