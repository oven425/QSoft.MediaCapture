using DirectN;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
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

namespace WpfApp_D3DImage
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //    async Task Start()
        //    {
        //        var hr = CreateVideoDeviceSource(out var pSource);
        //        //if (SUCCEEDED(hr))
        //        {
        //            IMFSourceReader m_pSourceReader;
        //            IMFAttributes pAttributes = null;
        //            hr = DirectN.MFFunctions.MFCreateAttributes(out pAttributes, 5);
        //            pAttributes.SetUINT32(DirectN.MFConstants.MF_READWRITE_ENABLE_HARDWARE_TRANSFORMS, 1);
        //            pAttributes.SetUINT32(DirectN.MFConstants.MF_SOURCE_READER_DISABLE_DXVA, 0);
        //            pAttributes.SetUINT32(DirectN.MFConstants.MF_SOURCE_READER_ENABLE_VIDEO_PROCESSING, 0);
        //            pAttributes.SetUINT32(DirectN.MFConstants.MF_SOURCE_READER_ENABLE_ADVANCED_VIDEO_PROCESSING, 1);
        //            pAttributes.SetUnknown(DirectN.MFConstants.MF_SOURCE_READER_D3D_MANAGER, pDeviceManager);


        //            hr = DirectN.Functions.MFCreateSourceReaderFromMediaSource(pSource, pAttributes, out m_pSourceReader);

        //            hr = m_pSourceReader.GetNativeMediaType(0xFFFFFFFC, 0, out var pMediaType);
        //            //GUID subtype;
        //            hr = pMediaType.GetGUID(DirectN.MFConstants.MF_MT_SUBTYPE, out var subtype);
        //            hr = pMediaType.SetGUID(DirectN.MFConstants.MF_MT_SUBTYPE, DirectN.MFConstants.MFVideoFormat_ARGB32);
        //            hr = DirectN.MFFunctions.MFCreateMediaType(out var tt);
        //            tt.SetGUID(DirectN.MFConstants.MF_MT_MAJOR_TYPE, DirectN.MFConstants.MFMediaType_Video);
        //            tt.SetGUID(DirectN.MFConstants.MF_MT_SUBTYPE, DirectN.MFConstants.MFVideoFormat_RGB32);
        //            hr = m_pSourceReader.SetCurrentMediaType(0xFFFFFFFC, IntPtr.Zero, tt);

        //            while (true)
        //            {

        //                using (var index = new ComMemory(Marshal.SizeOf<uint>()))
        //                using (var timestamp = new ComMemory(Marshal.SizeOf<long>()))
        //                using (var streamflag = new ComMemory(Marshal.SizeOf<uint>()))
        //                {
        //                    hr = m_pSourceReader.ReadSample(0, 0, index.Pointer, streamflag.Pointer, timestamp.Pointer, out var sample);
        //                    if (sample == null)
        //                    {
        //                        continue;
        //                    }

        //                    hr = sample.GetBufferByIndex(0, out var pBuffer);
        //                    //if (SUCCEEDED(hr))
        //                    {
        //                        //Guid("0cfbaf3a-9ff6-429a-99b3-a2796af8b89b")
        //                        hr = DirectN.MFFunctions.MFGetService(pBuffer, DirectN.MFConstants.MR_BUFFER_SERVICE, new Guid("0cfbaf3a-9ff6-429a-99b3-a2796af8b89b"), out var dd);
        //                        IDirect3DSurface9 surface = (IDirect3DSurface9)dd;
        //                        if (this.pRenderTexture == null)
        //                        {

        //                            CreateWPFCompatibleSurface(surface);

        //                            var ptr = Marshal.GetIUnknownForObject(pRenderSurface);
        //                            this.Dispatcher.Invoke(() =>
        //                            {
        //                                if (d3dimage.IsFrontBufferAvailable)
        //                                {
        //                                    this.d3dimage.Lock();
        //                                    this.d3dimage.SetBackBuffer(System.Windows.Interop.D3DResourceType.IDirect3DSurface9, ptr);
        //                                    this.d3dimage.Unlock();
        //                                }
        //                            });

        //                        }
        //                        pDeviceEx.StretchRect(
        //surface,
        //new tagRECT(0, 0, 1280, 720), pRenderSurface,  new tagRECT(0, 0, 1280, 720), DirectN._D3DTEXTUREFILTERTYPE.D3DTEXF_NONE);
        //                        this.Dispatcher.Invoke(() =>
        //                        {
        //                            if (d3dimage.IsFrontBufferAvailable)
        //                            {
        //                                this.d3dimage.Lock();
        //                                this.d3dimage.AddDirtyRect(new Int32Rect(0, 0, 1280, 720));
        //                                this.d3dimage.Unlock();
        //                            }
        //                        });

        //                        Marshal.ReleaseComObject(surface);
        //                        Marshal.ReleaseComObject(pBuffer);
        //                        Marshal.ReleaseComObject(sample);

        //                    }

        //                }




        //                await Task.Delay(30);
        //            }


        //            // Use the media source for capture or preview.
        //            //pSource->Release();
        //        }
        //    }

        //    IDirect3D9 pD3D = null;
        //    IDirect3D9Ex pD3D9Ex = null;
        //    IDirect3DDevice9 pDevice = null;
        //    IDirect3DDevice9Ex pDeviceEx = null;
        //    IDirect3DDeviceManager9 pDeviceManager = null;

        //    IDirect3DTexture9 pRenderTexture = null;
        //    IDirect3DSurface9 pRenderSurface = null;

        //    void CreateWPFCompatibleSurface(IDirect3DSurface9 surface)
        //    {
        //        DirectN._D3DSURFACE_DESC desc = new DirectN._D3DSURFACE_DESC();
        //        surface.GetDesc(ref desc);
        //        // 關鍵參數設定
        //        HRESULT hr = pDeviceEx.CreateTexture(
        //            desc.Width,
        //            desc.Height,
        //            1,
        //            DirectN.Constants.D3DUSAGE_RENDERTARGET,
        //            desc.Format,
        //            DirectN._D3DPOOL.D3DPOOL_DEFAULT,
        //            out pRenderTexture,
        //            IntPtr.Zero
        //        );

        //        pRenderTexture.GetSurfaceLevel(0, out pRenderSurface);
        //    }

        //    WindowInteropHelper helper;
        //    void InitD3D9Ex()
        //    {
        //        HRESULT hr = DirectN.Functions.Direct3DCreate9Ex(DirectN.Constants.D3D_SDK_VERSION, out pD3D9Ex);
        //        DirectN._D3DPRESENT_PARAMETERS_64 d3dpp = new DirectN._D3DPRESENT_PARAMETERS_64
        //        {
        //            Windowed = true,
        //            SwapEffect = _D3DSWAPEFFECT.D3DSWAPEFFECT_DISCARD,
        //            hDeviceWindow = helper.Handle
        //        };
        //        IntPtr unmanagedAddr = Marshal.AllocHGlobal(Marshal.SizeOf(d3dpp));
        //        Marshal.StructureToPtr<DirectN._D3DPRESENT_PARAMETERS_64>(d3dpp, unmanagedAddr, false);
        //        hr = pD3D9Ex.CreateDeviceEx(
        //            DirectN.Constants.D3DADAPTER_DEFAULT,
        //            DirectN._D3DDEVTYPE.D3DDEVTYPE_HAL,
        //            helper.Handle,
        //            DirectN.Constants.D3DCREATE_HARDWARE_VERTEXPROCESSING | DirectN.Constants.D3DCREATE_MULTITHREADED | DirectN.Constants.D3DCREATE_FPU_PRESERVE,
        //            unmanagedAddr,
        //            IntPtr.Zero,
        //            out pDeviceEx
        //        );
        //        uint resetToken = 0;
        //        hr = DXVA2CreateDirect3DDeviceManager9(out resetToken, out pDeviceManager);
        //        //if (FAILED(hr))
        //        //{
        //        //    pDevice->Release();
        //        //    return;
        //        //}
        //        hr = pDeviceManager.ResetDevice(pDeviceEx, resetToken);
        //        //if (FAILED(hr))
        //        //{
        //        //    pDeviceManager->Release();
        //        //    pDevice->Release();
        //        //    return;
        //        //}
        //    }


        //    void InitD3D9()
        //    {
        //        pD3D = DirectN.Functions.Direct3DCreate9(DirectN.Constants.D3D_SDK_VERSION);
        //        DirectN._D3DPRESENT_PARAMETERS_64 d3dpp = new DirectN._D3DPRESENT_PARAMETERS_64
        //        {
        //            Windowed = true,
        //            SwapEffect = _D3DSWAPEFFECT.D3DSWAPEFFECT_DISCARD,
        //            hDeviceWindow = DirectN.WindowsFunctions.GetDesktopWindow()
        //        };
        //        IntPtr unmanagedAddr = Marshal.AllocHGlobal(Marshal.SizeOf(d3dpp));
        //        Marshal.StructureToPtr<DirectN._D3DPRESENT_PARAMETERS_64>(d3dpp, unmanagedAddr, false);
        //        HRESULT hr = pD3D.CreateDevice(
        //            DirectN.Constants.D3DADAPTER_DEFAULT,
        //            DirectN._D3DDEVTYPE.D3DDEVTYPE_HAL,
        //            DirectN.WindowsFunctions.GetDesktopWindow(),
        //            DirectN.Constants.D3DCREATE_HARDWARE_VERTEXPROCESSING| DirectN.Constants.D3DCREATE_FPU_PRESERVE | DirectN.Constants.D3DCREATE_MULTITHREADED,
        //            unmanagedAddr,
        //            out pDevice
        //        );
        //        uint resetToken = 0;
        //        hr = DXVA2CreateDirect3DDeviceManager9(out resetToken, out pDeviceManager);
        //        //if (FAILED(hr))
        //        //{
        //        //    pDevice->Release();
        //        //    return;
        //        //}

        //        hr = pDeviceManager.ResetDevice(pDevice, resetToken);
        //        //if (FAILED(hr))
        //        //{
        //        //    pDeviceManager->Release();
        //        //    pDevice->Release();
        //        //    return;
        //        //}
        //    }

        //    HRESULT CreateVideoDeviceSource(out IMFMediaSource ppSource)
        //    {
        //        ppSource = null;
        //        IMFMediaSource pSource = null;
        //        IMFAttributes pAttributes = null;
        //        //IMFActivate ppDevices = null;
        //        IntPtr ppDevices = IntPtr.Zero;
        //        // Create an attribute store to specify the enumeration parameters.
        //        HRESULT hr = DirectN.MFFunctions.MFCreateAttributes(out pAttributes, 1);

        //        // Source type: video capture devices
        //        hr = pAttributes.SetGUID(
        //            DirectN.MFConstants.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE,
        //            DirectN.MFConstants.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_GUID
        //        );


        //        // Enumerate devices.
        //        hr = DirectN.Functions.MFEnumDeviceSources(pAttributes, out ppDevices, out var count);

        //        if (count > 0)
        //        {
        //            IntPtr[] ptrs = new IntPtr[count];
        //            Marshal.Copy(ppDevices, ptrs, 0, (int)count);

        //            for (int i = 0; i < count; i++)
        //            {
        //                // 將 IntPtr 轉換為受控介面
        //                IMFActivate activate = (IMFActivate)Marshal.GetObjectForIUnknown(ptrs[i]);
        //                ppSource = activate.ActivateObject<IMFMediaSource>().Object;
        //                break;
        //                // 這裡可以讀取設備名稱或啟動設備...

        //                // 釋放單個介面
        //                //Marshal.ReleaseComObject(activate);
        //            }
        //        }


        //        return hr;
        //    }
        //    [ComImport]
        //    [Guid("a0cade0f-06d5-4cf4-a1c7-f3cdd725aa75")]
        //    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        //    public interface IDirect3DDeviceManager9 
        //    { 
        //        int ResetDevice(IDirect3DDevice9 pDevice, uint resetToken); 
        //        int OpenDeviceHandle(out IntPtr phDevice); 
        //        int CloseDeviceHandle(IntPtr hDevice); 
        //        int TestDevice(IntPtr hDevice); 
        //        int LockDevice(IntPtr hDevice, out IntPtr ppDevice, bool fBlock); 
        //        int UnlockDevice(IntPtr hDevice, bool fSaveState); 
        //        int GetVideoService(IntPtr hDevice, ref Guid riid, out IntPtr ppService);
        //    }
        //    [DllImport("dxva2", ExactSpelling = true)]
        //    public static extern int DXVA2CreateDirect3DDeviceManager9(out uint pResetToken, out IDirect3DDeviceManager9 ppDeviceManager);

        WebCamD3D9 m_WebCamD3D9;
        WebCamD3D9Async m_WebCamD3D9Async;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.m_WebCamD3D9Async = new WebCamD3D9Async(this.d3dimage, new WindowInteropHelper(System.Windows.Application.Current.MainWindow));
            m_WebCamD3D9Async.Start();
            //helper = new WindowInteropHelper(System.Windows.Application.Current.MainWindow);
            //InitD3D9Ex();
            //_ = Start();


            //this.Start();
        }
    }
}
