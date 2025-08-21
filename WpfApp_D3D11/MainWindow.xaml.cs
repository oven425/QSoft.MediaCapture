using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.Direct3D9;
using SharpDX.DXGI;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Device = SharpDX.Direct3D11.Device;

namespace WpfApp_D3D11
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Device _device;
        private Texture2D _sharedTexture;
        private RenderTargetView _rtv;
        private D3DImage _d3dImage;
        private int _frameCounter;

        public MainWindow()
        {
            InitializeComponent();
            InitDirectX();
            InitD3DImage();

            // 綁定每幀渲染事件
            CompositionTarget.Rendering += OnRendering;
        }

        private void InitDirectX()
        {
            _device = new Device(DriverType.Hardware, DeviceCreationFlags.BgraSupport);

            var texDesc = new Texture2DDescription
            {
                Width = 800,
                Height = 600,
                MipLevels = 1,
                ArraySize = 1,
                Format = SharpDX.DXGI.Format.B8G8R8A8_UNorm,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                OptionFlags = ResourceOptionFlags.Shared
            };

            _sharedTexture = new Texture2D(_device, texDesc);
            _rtv = new RenderTargetView(_device, _sharedTexture);
        }

        private void InitD3DImage()
        {
            var d3d9Ex = new Direct3DEx();
            //var presentParams = new PresentParameters
            //{
            //    Windowed = true,
            //    SwapEffect = SharpDX.Direct3D9.SwapEffect.Discard,
            //    DeviceWindowHandle = hwnd,
            //    PresentationInterval = PresentInterval.Default
            //};

            //var d3d9DeviceEx = new DeviceEx(d3d9Ex, 0, DeviceType.Hardware, hwnd,
            //    CreateFlags.HardwareVertexProcessing | CreateFlags.Multithreaded | CreateFlags.FpuPreserve,
            //    presentParams);

            //// 開啟共享的 D3D9 Surface
            //var d3d9Surface = d3d9DeviceEx.CreateRenderTarget(800, 600,
            //    SharpDX.Direct3D9.Format.A8R8G8B8, MultisampleType.None, 0, true);

            //// 這裡是關鍵：從 sharedHandle 開啟 D3D9 Surface
            //var sharedSurface = SurfaceEx.OpenShared(d3d9DeviceEx, sharedHandle);


            //using (var dxgiRes = _sharedTexture.QueryInterface<Resource1>())
            //{
            //    IntPtr sharedHandle = dxgiRes.SharedHandle;

            //    // 用這個 handle 開啟另一個裝置的 Texture2D
            //    var sharedTex = _device.OpenSharedResource<Texture2D>(sharedHandle);

            //    //IntPtr sharedHandle = dxgiRes.CreateSharedHandle(null, SharedResourceFlags.None, null);

            //    _d3dImage = new D3DImage();
            //    _d3dImage.Lock();
            //    _d3dImage.SetBackBuffer(D3DResourceType.IDirect3DSurface9, sharedHandle);
            //    _d3dImage.Unlock();

            //    DxImage.Source = _d3dImage;
            //}
        }

        private void OnRendering(object sender, EventArgs e)
        {
            // 每幀改變背景顏色，形成動態效果
            float r = (float)Math.Abs(Math.Sin(_frameCounter * 0.05));
            float g = (float)Math.Abs(Math.Sin(_frameCounter * 0.07));
            float b = (float)Math.Abs(Math.Sin(_frameCounter * 0.09));

            _device.ImmediateContext.ClearRenderTargetView(_rtv, new Color4(r, g, b, 1f));

            // 通知 D3DImage 刷新
            _d3dImage.Lock();
            _d3dImage.AddDirtyRect(new Int32Rect(0, 0, 800, 600));
            _d3dImage.Unlock();

            _frameCounter++;
        }
    }
}