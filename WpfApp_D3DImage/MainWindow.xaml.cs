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
using Windows.Win32.Media.MediaFoundation;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Storage.FileSystem;

namespace WpfApp_D3DImage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Windows.Win32.PInvoke.MFCreateAttributes(out var attrs, 2);

        }

        unsafe private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Windows.Win32.PInvoke.MFCreateAttributes(out var pAttributes, 1);

            Guid f1;
            Guid f2;
            //Windows.Win32.Media.MediaFoundation.MF_ATTRIBUTES_MATCH_TYPE.
            //pAttributes.SetGUID(&f1, &f2);

            

        //    var hr = pAttributes.SetGUID(
        //MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE,
        //MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_GUID
        //);
        //    if (FAILED(hr))
        //    {
        //        goto done;
        //    }
        }
    }
}