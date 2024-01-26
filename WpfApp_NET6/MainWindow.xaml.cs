using QSoft.MediaCapture;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp_NET6
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var webcams = WebCam_MF.EnumDeviceSources();
            await webcams[0].InitializeCaptureManager(webcams[0].VideoDevice.Object, new Setting() {  });
            await webcams[0].StartPreview(x => { this.image.Source = x; });
        }
    }
}