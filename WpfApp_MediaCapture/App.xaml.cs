using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Media;

namespace WpfApp_MediaCapture
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            //RenderOptions.ProcessRenderMode = System.Windows.Interop.RenderMode.SoftwareOnly;
            base.OnStartup(e);
        }
    }

}
