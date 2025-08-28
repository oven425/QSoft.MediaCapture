using DirectN;
using System.CodeDom;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
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
using Windows.Devices.Enumeration;
using Windows.Graphics.DirectX.Direct3D11;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.Capture.Frames;
using Windows.Media.Devices;
using Windows.Media.MediaProperties;
using Windows.Storage;
using WinRT;
using static System.Net.Mime.MediaTypeNames;

namespace WpfApp_MediaCapture
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainUI m_MainUI;
        public MainWindow()
        {
            InitializeComponent();
            
            this.DataContext = this.m_MainUI = new MainUI();
        }

        MediaCapture m_MediaCapture;
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await this.InitMediaCapture();
           
        }

        string ToString(VideoEncodingProperties prop)
        {
            var sb = new StringBuilder();
            sb.Append(prop.Subtype);
            sb.Append(" ");
            sb.Append(prop.Width);
            sb.Append("x");
            sb.Append(prop.Height);
            if (prop.FrameRate.Numerator > 0 && prop.FrameRate.Denominator > 0)
            {
                sb.Append(" ");
                sb.Append(prop.FrameRate.Numerator / prop.FrameRate.Denominator);
                sb.Append("fps");
            }
            return sb.ToString();
        }
        ID3D11Resource outputD3dResource;
        bool m_IsFirst = true;
        D3DImage m_D3dImage;
        ID3D11DeviceContext inputD3dContext;
        void CopyToSharedTexture(IDirect3DSurface surface, ID3D11Device device)
        {
            try
            {
                IDirect3DDxgiInterfaceAccess access = surface.As<IDirect3DDxgiInterfaceAccess>();
                
                Guid iid = typeof(ID3D11Texture2D).GUID;
                access.GetInterface(ref iid, out IntPtr texturePtr);
                var unknow = Marshal.GetObjectForIUnknown(texturePtr);
                var sourceTexture = unknow as ID3D11Texture2D;

                
                DirectN.D3D11_TEXTURE2D_DESC desc1;
                sourceTexture.GetDesc(out desc1);

                
                if (this.m_IsFirst)
                {
                    sourceTexture.GetDevice(out m_D3D11Device);
                    this.m_IsFirst = false;
                    this.InitD3D9(desc1.Width, desc1.Height);
                    m_D3dImage = new D3DImage();
                    m_D3dImage.Lock();
                    var sp = Marshal.GetIUnknownForObject(this.m_d3d9SharedSurface);
                    m_D3dImage.SetBackBuffer(D3DResourceType.IDirect3DSurface9, sp);
                    m_D3dImage.Unlock();
                    this.image.Source = m_D3dImage;

                    var hr = m_D3D11Device.OpenSharedResource(this.m_d3d9SharedTextureHandle, typeof(ID3D11Resource).GUID, out var dr);
                    outputD3dResource = dr as ID3D11Resource;
                    m_D3D11Device.GetImmediateContext(out inputD3dContext);
                }




                inputD3dContext.CopyResource(outputD3dResource, sourceTexture);
                m_D3dImage.Lock();

                //inputD3dContext.CopySubresourceRegion(outputD3dResource, 0, 0, 0, 0, sourceTexture, inputSubresource, IntPtr.Zero);
                m_D3dImage.AddDirtyRect(new Int32Rect(0, 0, (int)desc1.Width, (int)desc1.Height));
                m_D3dImage.Unlock();

                //Marshal.ReleaseComObject(inputD3dContext);
                //Marshal.ReleaseComObject(outputD3dResource);
                //Marshal.ReleaseComObject(srcdev);

                Marshal.ReleaseComObject(sourceTexture);
                Marshal.Release(texturePtr);
                Marshal.ReleaseComObject(access);
            }
            catch(Exception ee)
            {
                System.Diagnostics.Trace.WriteLine(ee.Message);
            }
        }



        TT.IDirect3D9Ex d3d9;
        [DllImport("d3d9", ExactSpelling = true)]
        public static extern HRESULT Direct3DCreate9Ex(uint SDKVersion, out TT.IDirect3D9Ex unnamed__1);

        TT.IDirect3DDevice9Ex d3d9Device;
        IDirect3DSurface9 m_d3d9SharedSurface;
        IDirect3DTexture9 m_d3d9SharedTexture;
        IntPtr m_d3d9SharedTextureHandle = IntPtr.Zero;
        public void InitD3D9(uint width, uint height)
        {
            WindowInteropHelper helper = new WindowInteropHelper(System.Windows.Application.Current.MainWindow);
            var hwnd = helper.Handle;
            Direct3DCreate9Ex(Constants.D3D_SDK_VERSION, out d3d9);
            var parameters = new _D3DPRESENT_PARAMETERS_64();
            parameters.Windowed = true;
            parameters.SwapEffect = _D3DSWAPEFFECT.D3DSWAPEFFECT_DISCARD;
            parameters.hDeviceWindow = hwnd;
            parameters.PresentationInterval = 0x80000000;

            

            var ptr = Marshal.AllocHGlobal(Marshal.SizeOf(parameters));
            Marshal.StructureToPtr(parameters, ptr, false);



            var hr = d3d9.CreateDeviceEx(Constants.D3DADAPTER_DEFAULT, _D3DDEVTYPE.D3DDEVTYPE_HAL, hwnd,
                Constants.D3DCREATE_HARDWARE_VERTEXPROCESSING | Constants.D3DCREATE_MULTITHREADED | Constants.D3DCREATE_FPU_PRESERVE,
                ptr, IntPtr.Zero, out d3d9Device);
            Marshal.FreeHGlobal(ptr);
            hr = d3d9Device.CreateTexture(width, height, 1, DirectN.Constants.D3DUSAGE_RENDERTARGET, _D3DFORMAT.D3DFMT_A8R8G8B8, _D3DPOOL.D3DPOOL_DEFAULT,
                out m_d3d9SharedTexture, out m_d3d9SharedTextureHandle);
            hr = m_d3d9SharedTexture.GetSurfaceLevel(0, out m_d3d9SharedSurface);
        }

        bool d3dimage = true;

        ID3D11Device m_D3D11Device;
        ID3D11DeviceContext m_D3D11DeviceContext;
        ID3D11Texture2D sharedTexture;
        HRESULT InitD3D11Device()
        {
            var creationFlags = D3D11_CREATE_DEVICE_FLAG.D3D11_CREATE_DEVICE_BGRA_SUPPORT; // WPF interop 需要 BGRA
            creationFlags |= D3D11_CREATE_DEVICE_FLAG.D3D11_CREATE_DEVICE_DEBUG;

            var featureLevels = new D3D_FEATURE_LEVEL[] 
            {
                D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_11_1, 
                D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_11_0,
                D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_10_1,
                D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_10_0
            };

            D3D_FEATURE_LEVEL selectedLevel;
            IDXGIAdapter adapter = null;
            
            var ptr = Marshal.AllocHGlobal(Marshal.SizeOf<int>());
            var ddd = DirectN.Functions.D3D11CreateDevice(
                adapter,                    // 使用預設 adapter（第一張 GPU）
                D3D_DRIVER_TYPE.D3D_DRIVER_TYPE_HARDWARE,   // 使用硬體加速
                IntPtr.Zero,                    // 無需 software rasterizer
                (uint)creationFlags,
                featureLevels,
                featureLevels.Length,
                DirectN.Constants.D3D11_SDK_VERSION,
                out m_D3D11Device,
                ptr,
                out m_D3D11DeviceContext
            );
            var selectlevel = Marshal.ReadInt32(ptr);
            Marshal.FreeHGlobal(ptr);

            D3D11_TEXTURE2D_DESC desc = new D3D11_TEXTURE2D_DESC();
            desc.Width = 1280;
            desc.Height = 720;
            desc.Format = DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM;
            desc.Usage = D3D11_USAGE.D3D11_USAGE_DEFAULT;
            desc.BindFlags = (uint)D3D11_BIND_FLAG.D3D11_BIND_SHADER_RESOURCE;
            desc.MiscFlags = (uint)D3D11_RESOURCE_MISC_FLAG.D3D11_RESOURCE_MISC_SHARED;
            desc.CPUAccessFlags = 0;
            desc.ArraySize = 1;
            desc.MipLevels = 1;
            desc.SampleDesc.Count = 1;
            desc.SampleDesc.Quality = 0;

            // 建立共享紋理
           
            HRESULT hr = m_D3D11Device.CreateTexture2D(ref desc, null, out sharedTexture);

            //Guid iid1 = typeof(ID3D11Resource).GUID; // IDXGISurface
            //IntPtr ptrr;
            //hr = Marshal.QueryInterface(Marshal.GetIUnknownForObject(sharedTexture), ref iid1, out ptrr);

            return ddd;
        }

        readonly SemaphoreSlim m_Lock = new(1, 1);
        private async void M_MediaFrameReader_FrameArrived(MediaFrameReader sender, MediaFrameArrivedEventArgs args)
        {
            if (!await m_Lock.WaitAsync(0)) return;
            try
            {
                using var latestFrame = sender.TryAcquireLatestFrame();
                if (latestFrame is null) return;
                if(latestFrame.VideoMediaFrame is null) return;
                using var d3d = latestFrame?.VideoMediaFrame?.Direct3DSurface;

                if(d3dimage)
                {
                    await this.Dispatcher.InvokeAsync(() =>
                    {
                        CopyToSharedTexture(d3d, this.m_D3D11Device);
                    });
                }
                else
                {
                    using var softwareBitmap = d3d == null ? latestFrame?.VideoMediaFrame?.SoftwareBitmap : await SoftwareBitmap.CreateCopyFromSurfaceAsync(d3d);
                    if (softwareBitmap is not null)
                    {
                        await this.Dispatcher.InvokeAsync(() =>
                        {
                            using var m = softwareBitmap.LockBuffer(BitmapBufferAccessMode.Read);
                            using var reference = m.CreateReference();
                            if(this.m_IsFirst)
                            {
                                this.m_IsFirst = false;
                                m_WriteableBitmap = new WriteableBitmap((int)softwareBitmap.PixelWidth, (int)softwareBitmap.PixelHeight, 96, 96, PixelFormats.Bgr32, null);
                                this.image.Source = m_WriteableBitmap;

                            }
                            m_WriteableBitmap.Lock();

                            unsafe
                            {
                                (reference.As<IMemoryBufferByteAccess>()).GetBuffer(out var ptr, out var capacity);

                                NativeMemory.Copy(ptr, (void*)m_WriteableBitmap.BackBuffer, capacity);
                                m_WriteableBitmap.AddDirtyRect(new Int32Rect(0, 0, m_WriteableBitmap.PixelWidth, m_WriteableBitmap.PixelHeight));
                            }
                            m_WriteableBitmap.Unlock();

                        });
                    }
                }



                
            }
            catch(Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex);
            }
            finally
            {
                m_Lock.Release();
            }

        }

        [ComImport]
        [Guid("5B0D3235-4DBA-4D44-865E-8F1D0E4FD04D")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        unsafe interface IMemoryBufferByteAccess
        {
            void GetBuffer(out byte* buffer, out uint capacity);
        }

        [ComImport]
        [Guid("A9B3D012-3DF2-4EE3-B8D1-8695F457D3C1")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IDirect3DDxgiInterfaceAccess
        {
            void GetInterface([In] ref Guid iid, [Out] out IntPtr p);
        }


        WriteableBitmap? m_WriteableBitmap;

        MediaFrameReader? m_MediaFrameReader;

        async Task<MediaFrameSourceGroup?> GetVideoSource()
        {
            var cameras = await MediaFrameSourceGroup.FindAllAsync();
            foreach (var oo in cameras)
            {
                System.Diagnostics.Trace.WriteLine(oo.DisplayName);
            }
            return cameras.Count > 0 ? cameras[0] : null;
        }

        async Task<DeviceInformation?> GetAudioSource()
        {
            var audioDevices = await DeviceInformation.FindAllAsync(DeviceClass.AudioCapture);
            foreach (var oo in audioDevices)
            {
                System.Diagnostics.Trace.WriteLine($"Name:{oo.Name} IsEnabled:{oo.IsEnabled} EnclosureLocation:{oo.EnclosureLocation}");
            }
            var a = audioDevices.Where(x => x.IsEnabled);
            return a.FirstOrDefault();
        }

        private async void button_startrecord_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var profile = Windows.Media.MediaProperties.MediaEncodingProfile.CreateMp4(Windows.Media.MediaProperties.VideoEncodingQuality.Auto);
                var folder = await StorageFolder.GetFolderFromPathAsync(AppContext.BaseDirectory);
                var file = await folder.CreateFileAsync("video.mp4", CreationCollisionOption.GenerateUniqueName);

                m_LowLagRecord = await this.m_MediaCapture.PrepareLowLagRecordToStorageFileAsync(profile, file);
                await m_LowLagRecord.StartAsync();

            }
            catch (Exception ee)
            {
                System.Diagnostics.Trace.WriteLine(ee.Message);
                System.Diagnostics.Trace.WriteLine(ee.StackTrace);
            }

        }

        LowLagMediaRecording? m_LowLagRecord;
        async private void button_stoprecord_Click(object sender, RoutedEventArgs e)
        {
            if (m_LowLagRecord != null)
            {
                await m_LowLagRecord.StopAsync();
                await m_LowLagRecord.FinishAsync();
                m_LowLagRecord = null;
            }
        }

        async private void combobox_preview_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.m_MediaFrameReader != null)
            {
                await this.m_MediaFrameReader.StopAsync();
                this.m_MediaFrameReader.Dispose();
                this.m_MediaFrameReader = null;
            }
            if (this.m_LowLagRecord != null)
            {
                await this.m_LowLagRecord.StopAsync();
                await this.m_LowLagRecord.FinishAsync();
                this.m_LowLagRecord = null;
            }
            await m_MediaCapture.VideoDeviceController.SetMediaStreamPropertiesAsync(MediaStreamType.VideoPreview, this.m_MainUI.Preview);
            if(this.m_MainUI.Preview.Width > 1920)
            {
                double ddd = 1920.0 / this.m_MainUI.Preview.Width;
                double h = 1920.0 * this.m_MainUI.Preview.Height / this.m_MainUI.Preview.Width;
                var bs = new BitmapSize(1920, (uint)h);
            }

            var source = this.m_MediaCapture.FrameSources.MaxBy(x => x.Value.SupportedFormats.Count);
            
            this.m_MediaFrameReader = await this.m_MediaCapture.CreateFrameReaderAsync(source.Value, MediaEncodingSubtypes.Argb32);
            this.m_MediaFrameReader.FrameArrived += M_MediaFrameReader_FrameArrived;
            await this.m_MediaFrameReader.StartAsync();

        }

        async private void combobox_record_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(e.AddedItems.Count>0)
            {
                try
                {
                    var rr = e.AddedItems[0] as VideoEncodingProperties;
                    await this.m_MediaCapture.VideoDeviceController.SetMediaStreamPropertiesAsync(MediaStreamType.VideoRecord, rr);

                }
                catch (Exception ee)
                {
                    System.Diagnostics.Trace.WriteLine(ee.Message);
                }
            }
            
        }


        async Task InitMediaCapture()
        {
            try
            {
                var videoSource = await GetVideoSource();
                if (videoSource is null) return;
                var audiosource = await GetAudioSource();
                var mediacapturesetting = new MediaCaptureInitializationSettings()
                {

                    VideoDeviceId = videoSource.Id,
                    SharingMode = MediaCaptureSharingMode.ExclusiveControl,
                    StreamingCaptureMode = StreamingCaptureMode.Video,
                    MemoryPreference = MediaCaptureMemoryPreference.Auto,
                };
                if (audiosource is not null)
                {
                    mediacapturesetting.StreamingCaptureMode = StreamingCaptureMode.AudioAndVideo;
                    mediacapturesetting.AudioDeviceId = audiosource.Id;
                }
                this.m_MediaCapture = new MediaCapture();
                this.m_MainUI.Previews.Clear();
                await this.m_MediaCapture.InitializeAsync(mediacapturesetting);
                var aaa = m_MediaCapture.VideoDeviceController.GetAvailableMediaStreamProperties(Windows.Media.Capture.MediaStreamType.VideoPreview)
                    .OfType<VideoEncodingProperties>()
                    //.Where(x => x.Subtype == "NV12")
                    .OrderByDescending(x => x.Width * x.Height)
                    .ThenByDescending(x => x.FrameRate.Numerator / x.FrameRate.Denominator);
                System.Diagnostics.Trace.WriteLine("Available Video Preview Formats:");
                foreach (var oo in aaa)
                {
                    System.Diagnostics.Trace.WriteLine(ToString(oo));
                    this.m_MainUI.Previews.Add(oo);
                }
                this.m_MainUI.Records.Clear();
                var rrs = this.m_MediaCapture.VideoDeviceController.GetAvailableMediaStreamProperties(MediaStreamType.VideoRecord)
                    .OfType<VideoEncodingProperties>()
                    .OrderByDescending(x => x.Width * x.Height)
                    .ThenByDescending(x => x.FrameRate.Numerator / x.FrameRate.Denominator);
                System.Diagnostics.Trace.WriteLine("Available Video Record Formats:");
                foreach (var oo in rrs)
                {
                    System.Diagnostics.Trace.WriteLine(ToString(oo));
                    this.m_MainUI.Records.Add(oo);
                }
                var rr = this.m_MediaCapture.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoRecord) as VideoEncodingProperties;
                this.m_MainUI.Record = this.m_MainUI.Records.FirstOrDefault(x => x.Subtype == rr.Subtype && x.Width == rr.Width && x.Height == rr.Height && x.FrameRate.Denominator == rr.FrameRate.Denominator && x.FrameRate.Denominator == rr.FrameRate.Denominator);


                var audio_preview = m_MediaCapture.AudioDeviceController.GetAvailableMediaStreamProperties(Windows.Media.Capture.MediaStreamType.Audio)
                    .OfType<AudioEncodingProperties>();
                System.Diagnostics.Trace.WriteLine("Available Audio Preview Formats:");
                foreach (var oo in audio_preview)
                {
                    System.Diagnostics.Trace.WriteLine($"{oo.Subtype} {oo.Bitrate} {oo.SampleRate}");
                }
            }
            catch (Exception ee)
            {
                System.Diagnostics.Debug.WriteLine(ee.Message);
                System.Diagnostics.Trace.WriteLine(ee.StackTrace);
            }
        }

        async Task UnInitMediaCapture()
        {
            if (this.m_MediaFrameReader != null)
            {
                await this.m_MediaFrameReader.StopAsync();
                this.m_MediaFrameReader.Dispose();
                this.m_MediaFrameReader = null;
            }
            if (this.m_LowLagRecord != null)
            {
                await this.m_LowLagRecord.StopAsync();
                await this.m_LowLagRecord.FinishAsync();
                this.m_LowLagRecord = null;
            }
            if (this.m_MediaCapture is not null)
            {
                this.m_MediaCapture.Dispose();
            }
        }
    }


    public class TestDevice
    {
        async public Task UnInit()
        {
            await Task.Delay(5000);
        }

        public async Task Init()
        {
            await Task.Delay(5000);
        }
    }

    public class MainUI : INotifyPropertyChanged
    {
        VideoEncodingProperties m_Preview;
        public VideoEncodingProperties Preview
        {
            set
            {
                m_Preview = value;
                this.Update("Preview");
            }
            get => m_Preview;
        }

        VideoEncodingProperties m_Record;
        public VideoEncodingProperties Record
        {
            set
            {
                m_Record = value;
                this.Update("Record");
            }
            get => m_Record;
        }
        public ObservableCollection<VideoEncodingProperties> Previews { set; get; } = new ObservableCollection<VideoEncodingProperties>();

        public ObservableCollection<VideoEncodingProperties> Records { set; get; } = new ObservableCollection<VideoEncodingProperties>();

        public ObservableCollection<MediaFrameSourceGroup> VideoSources { set; get; } = new ObservableCollection<MediaFrameSourceGroup>();
        MediaFrameSourceGroup m_VideoSource;
        public MediaFrameSourceGroup VideoSource
        {
            set
            {
                m_VideoSource = value;
                this.Update("VideoSource");
            }
            get => m_VideoSource;
        }
        public ObservableCollection<DeviceInformation> AudioSources { set; get; } = new ObservableCollection<DeviceInformation>();
        DeviceInformation m_AudioSource;
        public DeviceInformation AudioSource
        {
            set
            {
                m_AudioSource = value;
                this.Update("AudioSource");
            }
            get => m_AudioSource;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        void Update(string name) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

namespace TT
{
    [ComImport]
    [Guid("02177241-69fc-400c-8ff1-93a44df6861d")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDirect3D9Ex : IDirect3D9
    {
        [PreserveSig]
        new HRESULT RegisterSoftwareDevice(IntPtr pInitializeFunction);

        [PreserveSig]
        new uint GetAdapterCount();

        [PreserveSig]
        new HRESULT GetAdapterIdentifier(uint Adapter, uint Flags, IntPtr pIdentifier);

        [PreserveSig]
        new uint GetAdapterModeCount(uint Adapter, _D3DFORMAT Format);

        [PreserveSig]
        new HRESULT EnumAdapterModes(uint Adapter, _D3DFORMAT Format, uint Mode, ref _D3DDISPLAYMODE pMode);

        [PreserveSig]
        new HRESULT GetAdapterDisplayMode(uint Adapter, ref _D3DDISPLAYMODE pMode);

        [PreserveSig]
        new HRESULT CheckDeviceType(uint Adapter, _D3DDEVTYPE DevType, _D3DFORMAT AdapterFormat, _D3DFORMAT BackBufferFormat, bool bWindowed);

        [PreserveSig]
        new HRESULT CheckDeviceFormat(uint Adapter, _D3DDEVTYPE DeviceType, _D3DFORMAT AdapterFormat, uint Usage, _D3DRESOURCETYPE RType, _D3DFORMAT CheckFormat);

        [PreserveSig]
        new HRESULT CheckDeviceMultiSampleType(uint Adapter, _D3DDEVTYPE DeviceType, _D3DFORMAT SurfaceFormat, bool Windowed, _D3DMULTISAMPLE_TYPE MultiSampleType, ref uint pQualityLevels);

        [PreserveSig]
        new HRESULT CheckDepthStencilMatch(uint Adapter, _D3DDEVTYPE DeviceType, _D3DFORMAT AdapterFormat, _D3DFORMAT RenderTargetFormat, _D3DFORMAT DepthStencilFormat);

        [PreserveSig]
        new HRESULT CheckDeviceFormatConversion(uint Adapter, _D3DDEVTYPE DeviceType, _D3DFORMAT SourceFormat, _D3DFORMAT TargetFormat);

        [PreserveSig]
        new HRESULT GetDeviceCaps(uint Adapter, _D3DDEVTYPE DeviceType, ref _D3DCAPS9 pCaps);

        [PreserveSig]
        new IntPtr GetAdapterMonitor(uint Adapter);

        [PreserveSig]
        new HRESULT CreateDevice(uint Adapter, _D3DDEVTYPE DeviceType, IntPtr hFocusWindow, uint BehaviorFlags, IntPtr pPresentationParameters, out IDirect3DDevice9 ppReturnedDeviceInterface);

        [PreserveSig]
        uint GetAdapterModeCountEx(uint Adapter, ref D3DDISPLAYMODEFILTER pFilter);

        [PreserveSig]
        HRESULT EnumAdapterModesEx(uint Adapter, ref D3DDISPLAYMODEFILTER pFilter, uint Mode, ref D3DDISPLAYMODEEX pMode);

        [PreserveSig]
        HRESULT GetAdapterDisplayModeEx(uint Adapter, ref D3DDISPLAYMODEEX pMode, ref D3DDISPLAYROTATION pRotation);

        [PreserveSig]
        HRESULT CreateDeviceEx(uint Adapter, _D3DDEVTYPE DeviceType, IntPtr hFocusWindow, uint BehaviorFlags, IntPtr pPresentationParameters, IntPtr pFullscreenDisplayMode, out IDirect3DDevice9Ex ppReturnedDeviceInterface);

        [PreserveSig]
        HRESULT GetAdapterLUID(uint Adapter, ref _LUID pLUID);
    }


    [ComImport]
    [Guid("d0223b96-bf7a-43fd-92bd-a43b0d82b9eb")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDirect3DDevice9
    {
        [PreserveSig]
        HRESULT TestCooperativeLevel();

        [PreserveSig]
        uint GetAvailableTextureMem();

        [PreserveSig]
        HRESULT EvictManagedResources();

        [PreserveSig]
        HRESULT GetDirect3D(out IDirect3D9 ppD3D9);

        [PreserveSig]
        HRESULT GetDeviceCaps(ref _D3DCAPS9 pCaps);

        [PreserveSig]
        HRESULT GetDisplayMode(uint iSwapChain, ref _D3DDISPLAYMODE pMode);

        [PreserveSig]
        HRESULT GetCreationParameters(IntPtr pParameters);

        [PreserveSig]
        HRESULT SetCursorProperties(uint XHotSpot, uint YHotSpot, IDirect3DSurface9 pCursorBitmap);

        [PreserveSig]
        void SetCursorPosition(int X, int Y, uint Flags);

        [PreserveSig]
        bool ShowCursor(bool bShow);

        [PreserveSig]
        HRESULT CreateAdditionalSwapChain(IntPtr pPresentationParameters, out IDirect3DSwapChain9 pSwapChain);

        [PreserveSig]
        HRESULT GetSwapChain(uint iSwapChain, out IDirect3DSwapChain9 pSwapChain);

        [PreserveSig]
        uint GetNumberOfSwapChains();

        [PreserveSig]
        HRESULT Reset(IntPtr pPresentationParameters);

        [PreserveSig]
        HRESULT Present(ref tagRECT pSourceRect, ref tagRECT pDestRect, IntPtr hDestWindowOverride, ref _RGNDATA pDirtyRegion);

        [PreserveSig]
        HRESULT GetBackBuffer(uint iSwapChain, uint iBackBuffer, _D3DBACKBUFFER_TYPE Type, out IDirect3DSurface9 ppBackBuffer);

        [PreserveSig]
        HRESULT GetRasterStatus(uint iSwapChain, ref _D3DRASTER_STATUS pRasterStatus);

        [PreserveSig]
        HRESULT SetDialogBoxMode(bool bEnableDialogs);

        [PreserveSig]
        void SetGammaRamp(uint iSwapChain, uint Flags, ref _D3DGAMMARAMP pRamp);

        [PreserveSig]
        void GetGammaRamp(uint iSwapChain, ref _D3DGAMMARAMP pRamp);

        [PreserveSig]
        HRESULT CreateTexture(uint Width, uint Height, uint Levels, uint Usage, _D3DFORMAT Format, _D3DPOOL Pool, out IDirect3DTexture9 ppTexture, IntPtr pSharedHandle);

        [PreserveSig]
        HRESULT CreateVolumeTexture(uint Width, uint Height, uint Depth, uint Levels, uint Usage, _D3DFORMAT Format, _D3DPOOL Pool, out IDirect3DVolumeTexture9 ppVolumeTexture, IntPtr pSharedHandle);

        [PreserveSig]
        HRESULT CreateCubeTexture(uint EdgeLength, uint Levels, uint Usage, _D3DFORMAT Format, _D3DPOOL Pool, out IDirect3DCubeTexture9 ppCubeTexture, IntPtr pSharedHandle);

        [PreserveSig]
        HRESULT CreateVertexBuffer(uint Length, uint Usage, uint FVF, _D3DPOOL Pool, out IDirect3DVertexBuffer9 ppVertexBuffer, IntPtr pSharedHandle);

        [PreserveSig]
        HRESULT CreateIndexBuffer(uint Length, uint Usage, _D3DFORMAT Format, _D3DPOOL Pool, out IDirect3DIndexBuffer9 ppIndexBuffer, IntPtr pSharedHandle);

        [PreserveSig]
        HRESULT CreateRenderTarget(uint Width, uint Height, _D3DFORMAT Format, _D3DMULTISAMPLE_TYPE MultiSample, uint MultisampleQuality, bool Lockable, out IDirect3DSurface9 ppSurface, IntPtr pSharedHandle);

        [PreserveSig]
        HRESULT CreateDepthStencilSurface(uint Width, uint Height, _D3DFORMAT Format, _D3DMULTISAMPLE_TYPE MultiSample, uint MultisampleQuality, bool Discard, out IDirect3DSurface9 ppSurface, IntPtr pSharedHandle);

        [PreserveSig]
        HRESULT UpdateSurface(IDirect3DSurface9 pSourceSurface, ref tagRECT pSourceRect, IDirect3DSurface9 pDestinationSurface, ref tagPOINT pDestPoint);

        [PreserveSig]
        HRESULT UpdateTexture(IDirect3DBaseTexture9 pSourceTexture, IDirect3DBaseTexture9 pDestinationTexture);

        [PreserveSig]
        HRESULT GetRenderTargetData(IDirect3DSurface9 pRenderTarget, IDirect3DSurface9 pDestSurface);

        [PreserveSig]
        HRESULT GetFrontBufferData(uint iSwapChain, IDirect3DSurface9 pDestSurface);

        [PreserveSig]
        HRESULT StretchRect(IDirect3DSurface9 pSourceSurface, ref tagRECT pSourceRect, IDirect3DSurface9 pDestSurface, ref tagRECT pDestRect, _D3DTEXTUREFILTERTYPE Filter);

        [PreserveSig]
        HRESULT ColorFill(IDirect3DSurface9 pSurface, ref tagRECT pRect, uint color);

        [PreserveSig]
        HRESULT CreateOffscreenPlainSurface(uint Width, uint Height, _D3DFORMAT Format, _D3DPOOL Pool, out IDirect3DSurface9 ppSurface, IntPtr pSharedHandle);

        [PreserveSig]
        HRESULT SetRenderTarget(uint RenderTargetIndex, IDirect3DSurface9 pRenderTarget);

        [PreserveSig]
        HRESULT GetRenderTarget(uint RenderTargetIndex, out IDirect3DSurface9 ppRenderTarget);

        [PreserveSig]
        HRESULT SetDepthStencilSurface(IDirect3DSurface9 pNewZStencil);

        [PreserveSig]
        HRESULT GetDepthStencilSurface(out IDirect3DSurface9 ppZStencilSurface);

        [PreserveSig]
        HRESULT BeginScene();

        [PreserveSig]
        HRESULT EndScene();

        [PreserveSig]
        HRESULT Clear(uint Count, ref _D3DRECT pRects, uint Flags, uint Color, float Z, uint Stencil);

        [PreserveSig]
        HRESULT SetTransform(_D3DTRANSFORMSTATETYPE State, ref _D3DMATRIX pMatrix);

        [PreserveSig]
        HRESULT GetTransform(_D3DTRANSFORMSTATETYPE State, ref _D3DMATRIX pMatrix);

        [PreserveSig]
        HRESULT MultiplyTransform(_D3DTRANSFORMSTATETYPE unnamed__0, ref _D3DMATRIX unnamed__1);

        [PreserveSig]
        HRESULT SetViewport(ref _D3DVIEWPORT9 pViewport);

        [PreserveSig]
        HRESULT GetViewport(ref _D3DVIEWPORT9 pViewport);

        [PreserveSig]
        HRESULT SetMaterial(ref _D3DMATERIAL9 pMaterial);

        [PreserveSig]
        HRESULT GetMaterial(ref _D3DMATERIAL9 pMaterial);

        [PreserveSig]
        HRESULT SetLight(uint Index, ref _D3DLIGHT9 unnamed__1);

        [PreserveSig]
        HRESULT GetLight(uint Index, ref _D3DLIGHT9 unnamed__1);

        [PreserveSig]
        HRESULT LightEnable(uint Index, bool Enable);

        [PreserveSig]
        HRESULT GetLightEnable(uint Index, ref bool pEnable);

        [PreserveSig]
        HRESULT SetClipPlane(uint Index, ref float pPlane);

        [PreserveSig]
        HRESULT GetClipPlane(uint Index, ref float pPlane);

        [PreserveSig]
        HRESULT SetRenderState(_D3DRENDERSTATETYPE State, uint Value);

        [PreserveSig]
        HRESULT GetRenderState(_D3DRENDERSTATETYPE State, ref uint pValue);

        [PreserveSig]
        HRESULT CreateStateBlock(_D3DSTATEBLOCKTYPE Type, out IDirect3DStateBlock9 ppSB);

        [PreserveSig]
        HRESULT BeginStateBlock();

        [PreserveSig]
        HRESULT EndStateBlock(out IDirect3DStateBlock9 ppSB);

        [PreserveSig]
        HRESULT SetClipStatus(ref _D3DCLIPSTATUS9 pClipStatus);

        [PreserveSig]
        HRESULT GetClipStatus(ref _D3DCLIPSTATUS9 pClipStatus);

        [PreserveSig]
        HRESULT GetTexture(uint Stage, out IDirect3DBaseTexture9 ppTexture);

        [PreserveSig]
        HRESULT SetTexture(uint Stage, IDirect3DBaseTexture9 pTexture);

        [PreserveSig]
        HRESULT GetTextureStageState(uint Stage, _D3DTEXTURESTAGESTATETYPE Type, ref uint pValue);

        [PreserveSig]
        HRESULT SetTextureStageState(uint Stage, _D3DTEXTURESTAGESTATETYPE Type, uint Value);

        [PreserveSig]
        HRESULT GetSamplerState(uint Sampler, _D3DSAMPLERSTATETYPE Type, ref uint pValue);

        [PreserveSig]
        HRESULT SetSamplerState(uint Sampler, _D3DSAMPLERSTATETYPE Type, uint Value);

        [PreserveSig]
        HRESULT ValidateDevice(ref uint pNumPasses);

        [PreserveSig]
        HRESULT SetPaletteEntries(uint PaletteNumber, ref tagPALETTEENTRY pEntries);

        [PreserveSig]
        HRESULT GetPaletteEntries(uint PaletteNumber, ref tagPALETTEENTRY pEntries);

        [PreserveSig]
        HRESULT SetCurrentTexturePalette(uint PaletteNumber);

        [PreserveSig]
        HRESULT GetCurrentTexturePalette(ref uint PaletteNumber);

        [PreserveSig]
        HRESULT SetScissorRect(ref tagRECT pRect);

        [PreserveSig]
        HRESULT GetScissorRect(ref tagRECT pRect);

        [PreserveSig]
        HRESULT SetSoftwareVertexProcessing(bool bSoftware);

        [PreserveSig]
        bool GetSoftwareVertexProcessing();

        [PreserveSig]
        HRESULT SetNPatchMode(float nSegments);

        [PreserveSig]
        void GetNPatchMode();

        [PreserveSig]
        HRESULT DrawPrimitive(_D3DPRIMITIVETYPE PrimitiveType, uint StartVertex, uint PrimitiveCount);

        [PreserveSig]
        HRESULT DrawIndexedPrimitive(_D3DPRIMITIVETYPE unnamed__0, int BaseVertexIndex, uint MinVertexIndex, uint NumVertices, uint startIndex, uint primCount);

        [PreserveSig]
        HRESULT DrawPrimitiveUP(_D3DPRIMITIVETYPE PrimitiveType, uint PrimitiveCount, IntPtr pVertexStreamZeroData, uint VertexStreamZeroStride);

        [PreserveSig]
        HRESULT DrawIndexedPrimitiveUP(_D3DPRIMITIVETYPE PrimitiveType, uint MinVertexIndex, uint NumVertices, uint PrimitiveCount, IntPtr pIndexData, _D3DFORMAT IndexDataFormat, IntPtr pVertexStreamZeroData, uint VertexStreamZeroStride);

        [PreserveSig]
        HRESULT ProcessVertices(uint SrcStartIndex, uint DestIndex, uint VertexCount, IDirect3DVertexBuffer9 pDestBuffer, IDirect3DVertexDeclaration9 pVertexDecl, uint Flags);

        [PreserveSig]
        HRESULT CreateVertexDeclaration(ref _D3DVERTEXELEMENT9 pVertexElements, out IDirect3DVertexDeclaration9 ppDecl);

        [PreserveSig]
        HRESULT SetVertexDeclaration(IDirect3DVertexDeclaration9 pDecl);

        [PreserveSig]
        HRESULT GetVertexDeclaration(out IDirect3DVertexDeclaration9 ppDecl);

        [PreserveSig]
        HRESULT SetFVF(uint FVF);

        [PreserveSig]
        HRESULT GetFVF(ref uint pFVF);

        [PreserveSig]
        HRESULT CreateVertexShader(IntPtr pFunction, out IDirect3DVertexShader9 ppShader);

        [PreserveSig]
        HRESULT SetVertexShader(IDirect3DVertexShader9 pShader);

        [PreserveSig]
        HRESULT GetVertexShader(out IDirect3DVertexShader9 ppShader);

        [PreserveSig]
        HRESULT SetVertexShaderConstantF(uint StartRegister, IntPtr pConstantData, uint Vector4fCount);

        [PreserveSig]
        HRESULT GetVertexShaderConstantF(uint StartRegister, IntPtr pConstantData, uint Vector4fCount);

        [PreserveSig]
        HRESULT SetVertexShaderConstantI(uint StartRegister, IntPtr pConstantData, uint Vector4iCount);

        [PreserveSig]
        HRESULT GetVertexShaderConstantI(uint StartRegister, IntPtr pConstantData, uint Vector4iCount);

        [PreserveSig]
        HRESULT SetVertexShaderConstantB(uint StartRegister, IntPtr pConstantData, uint BoolCount);

        [PreserveSig]
        HRESULT GetVertexShaderConstantB(uint StartRegister, IntPtr pConstantData, uint BoolCount);

        [PreserveSig]
        HRESULT SetStreamSource(uint StreamNumber, IDirect3DVertexBuffer9 pStreamData, uint OffsetInBytes, uint Stride);

        [PreserveSig]
        HRESULT GetStreamSource(uint StreamNumber, out IDirect3DVertexBuffer9 ppStreamData, ref uint pOffsetInBytes, ref uint pStride);

        [PreserveSig]
        HRESULT SetStreamSourceFreq(uint StreamNumber, uint Setting);

        [PreserveSig]
        HRESULT GetStreamSourceFreq(uint StreamNumber, ref uint pSetting);

        [PreserveSig]
        HRESULT SetIndices(IDirect3DIndexBuffer9 pIndexData);

        [PreserveSig]
        HRESULT GetIndices(out IDirect3DIndexBuffer9 ppIndexData);

        [PreserveSig]
        HRESULT CreatePixelShader(IntPtr pFunction, out IDirect3DPixelShader9 ppShader);

        [PreserveSig]
        HRESULT SetPixelShader(IDirect3DPixelShader9 pShader);

        [PreserveSig]
        HRESULT GetPixelShader(out IDirect3DPixelShader9 ppShader);

        [PreserveSig]
        HRESULT SetPixelShaderConstantF(uint StartRegister, IntPtr pConstantData, uint Vector4fCount);

        [PreserveSig]
        HRESULT GetPixelShaderConstantF(uint StartRegister, IntPtr pConstantData, uint Vector4fCount);

        [PreserveSig]
        HRESULT SetPixelShaderConstantI(uint StartRegister, IntPtr pConstantData, uint Vector4iCount);

        [PreserveSig]
        HRESULT GetPixelShaderConstantI(uint StartRegister, IntPtr pConstantData, uint Vector4iCount);

        [PreserveSig]
        HRESULT SetPixelShaderConstantB(uint StartRegister, IntPtr pConstantData, uint BoolCount);

        [PreserveSig]
        HRESULT GetPixelShaderConstantB(uint StartRegister, IntPtr pConstantData, uint BoolCount);

        [PreserveSig]
        HRESULT DrawRectPatch(uint Handle, ref float pNumSegs, ref _D3DRECTPATCH_INFO pRectPatchInfo);

        [PreserveSig]
        HRESULT DrawTriPatch(uint Handle, ref float pNumSegs, ref _D3DTRIPATCH_INFO pTriPatchInfo);

        [PreserveSig]
        HRESULT DeletePatch(uint Handle);

        [PreserveSig]
        HRESULT CreateQuery(_D3DQUERYTYPE Type, out IDirect3DQuery9 ppQuery);
    }

    [ComImport]
    [Guid("b18b10ce-2649-405a-870f-95f777d4313a")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDirect3DDevice9Ex : IDirect3DDevice9
    {
        [PreserveSig]
        new HRESULT TestCooperativeLevel();

        [PreserveSig]
        new uint GetAvailableTextureMem();

        [PreserveSig]
        new HRESULT EvictManagedResources();

        [PreserveSig]
        new HRESULT GetDirect3D(out IDirect3D9 ppD3D9);

        [PreserveSig]
        new HRESULT GetDeviceCaps(ref _D3DCAPS9 pCaps);

        [PreserveSig]
        new HRESULT GetDisplayMode(uint iSwapChain, ref _D3DDISPLAYMODE pMode);

        [PreserveSig]
        new HRESULT GetCreationParameters(IntPtr pParameters);

        [PreserveSig]
        new HRESULT SetCursorProperties(uint XHotSpot, uint YHotSpot, IDirect3DSurface9 pCursorBitmap);

        [PreserveSig]
        new void SetCursorPosition(int X, int Y, uint Flags);

        [PreserveSig]
        new bool ShowCursor(bool bShow);

        [PreserveSig]
        new HRESULT CreateAdditionalSwapChain(IntPtr pPresentationParameters, out IDirect3DSwapChain9 pSwapChain);

        [PreserveSig]
        new HRESULT GetSwapChain(uint iSwapChain, out IDirect3DSwapChain9 pSwapChain);

        [PreserveSig]
        new uint GetNumberOfSwapChains();

        [PreserveSig]
        new HRESULT Reset(IntPtr pPresentationParameters);

        [PreserveSig]
        new HRESULT Present(ref tagRECT pSourceRect, ref tagRECT pDestRect, IntPtr hDestWindowOverride, ref _RGNDATA pDirtyRegion);

        [PreserveSig]
        new HRESULT GetBackBuffer(uint iSwapChain, uint iBackBuffer, _D3DBACKBUFFER_TYPE Type, out IDirect3DSurface9 ppBackBuffer);

        [PreserveSig]
        new HRESULT GetRasterStatus(uint iSwapChain, ref _D3DRASTER_STATUS pRasterStatus);

        [PreserveSig]
        new HRESULT SetDialogBoxMode(bool bEnableDialogs);

        [PreserveSig]
        new void SetGammaRamp(uint iSwapChain, uint Flags, ref _D3DGAMMARAMP pRamp);

        [PreserveSig]
        new void GetGammaRamp(uint iSwapChain, ref _D3DGAMMARAMP pRamp);

        [PreserveSig]
        new HRESULT CreateTexture(uint Width, uint Height, uint Levels, uint Usage, _D3DFORMAT Format, _D3DPOOL Pool, out IDirect3DTexture9 ppTexture, out IntPtr pSharedHandle);

        [PreserveSig]
        new HRESULT CreateVolumeTexture(uint Width, uint Height, uint Depth, uint Levels, uint Usage, _D3DFORMAT Format, _D3DPOOL Pool, out IDirect3DVolumeTexture9 ppVolumeTexture, IntPtr pSharedHandle);

        [PreserveSig]
        new HRESULT CreateCubeTexture(uint EdgeLength, uint Levels, uint Usage, _D3DFORMAT Format, _D3DPOOL Pool, out IDirect3DCubeTexture9 ppCubeTexture, IntPtr pSharedHandle);

        [PreserveSig]
        new HRESULT CreateVertexBuffer(uint Length, uint Usage, uint FVF, _D3DPOOL Pool, out IDirect3DVertexBuffer9 ppVertexBuffer, IntPtr pSharedHandle);

        [PreserveSig]
        new HRESULT CreateIndexBuffer(uint Length, uint Usage, _D3DFORMAT Format, _D3DPOOL Pool, out IDirect3DIndexBuffer9 ppIndexBuffer, IntPtr pSharedHandle);

        [PreserveSig]
        new HRESULT CreateRenderTarget(uint Width, uint Height, _D3DFORMAT Format, _D3DMULTISAMPLE_TYPE MultiSample, uint MultisampleQuality, bool Lockable, out IDirect3DSurface9 ppSurface, IntPtr pSharedHandle);

        [PreserveSig]
        new HRESULT CreateDepthStencilSurface(uint Width, uint Height, _D3DFORMAT Format, _D3DMULTISAMPLE_TYPE MultiSample, uint MultisampleQuality, bool Discard, out IDirect3DSurface9 ppSurface, IntPtr pSharedHandle);

        [PreserveSig]
        new HRESULT UpdateSurface(IDirect3DSurface9 pSourceSurface, ref tagRECT pSourceRect, IDirect3DSurface9 pDestinationSurface, ref tagPOINT pDestPoint);

        [PreserveSig]
        new HRESULT UpdateTexture(IDirect3DBaseTexture9 pSourceTexture, IDirect3DBaseTexture9 pDestinationTexture);

        [PreserveSig]
        new HRESULT GetRenderTargetData(IDirect3DSurface9 pRenderTarget, IDirect3DSurface9 pDestSurface);

        [PreserveSig]
        new HRESULT GetFrontBufferData(uint iSwapChain, IDirect3DSurface9 pDestSurface);

        [PreserveSig]
        new HRESULT StretchRect(IDirect3DSurface9 pSourceSurface, ref tagRECT pSourceRect, IDirect3DSurface9 pDestSurface, ref tagRECT pDestRect, _D3DTEXTUREFILTERTYPE Filter);

        [PreserveSig]
        new HRESULT ColorFill(IDirect3DSurface9 pSurface, ref tagRECT pRect, uint color);

        [PreserveSig]
        new HRESULT CreateOffscreenPlainSurface(uint Width, uint Height, _D3DFORMAT Format, _D3DPOOL Pool, out IDirect3DSurface9 ppSurface, IntPtr pSharedHandle);

        [PreserveSig]
        new HRESULT SetRenderTarget(uint RenderTargetIndex, IDirect3DSurface9 pRenderTarget);

        [PreserveSig]
        new HRESULT GetRenderTarget(uint RenderTargetIndex, out IDirect3DSurface9 ppRenderTarget);

        [PreserveSig]
        new HRESULT SetDepthStencilSurface(IDirect3DSurface9 pNewZStencil);

        [PreserveSig]
        new HRESULT GetDepthStencilSurface(out IDirect3DSurface9 ppZStencilSurface);

        [PreserveSig]
        new HRESULT BeginScene();

        [PreserveSig]
        new HRESULT EndScene();

        [PreserveSig]
        new HRESULT Clear(uint Count, ref _D3DRECT pRects, uint Flags, uint Color, float Z, uint Stencil);

        [PreserveSig]
        new HRESULT SetTransform(_D3DTRANSFORMSTATETYPE State, ref _D3DMATRIX pMatrix);

        [PreserveSig]
        new HRESULT GetTransform(_D3DTRANSFORMSTATETYPE State, ref _D3DMATRIX pMatrix);

        [PreserveSig]
        new HRESULT MultiplyTransform(_D3DTRANSFORMSTATETYPE unnamed__0, ref _D3DMATRIX unnamed__1);

        [PreserveSig]
        new HRESULT SetViewport(ref _D3DVIEWPORT9 pViewport);

        [PreserveSig]
        new HRESULT GetViewport(ref _D3DVIEWPORT9 pViewport);

        [PreserveSig]
        new HRESULT SetMaterial(ref _D3DMATERIAL9 pMaterial);

        [PreserveSig]
        new HRESULT GetMaterial(ref _D3DMATERIAL9 pMaterial);

        [PreserveSig]
        new HRESULT SetLight(uint Index, ref _D3DLIGHT9 unnamed__1);

        [PreserveSig]
        new HRESULT GetLight(uint Index, ref _D3DLIGHT9 unnamed__1);

        [PreserveSig]
        new HRESULT LightEnable(uint Index, bool Enable);

        [PreserveSig]
        new HRESULT GetLightEnable(uint Index, ref bool pEnable);

        [PreserveSig]
        new HRESULT SetClipPlane(uint Index, ref float pPlane);

        [PreserveSig]
        new HRESULT GetClipPlane(uint Index, ref float pPlane);

        [PreserveSig]
        new HRESULT SetRenderState(_D3DRENDERSTATETYPE State, uint Value);

        [PreserveSig]
        new HRESULT GetRenderState(_D3DRENDERSTATETYPE State, ref uint pValue);

        [PreserveSig]
        new HRESULT CreateStateBlock(_D3DSTATEBLOCKTYPE Type, out IDirect3DStateBlock9 ppSB);

        [PreserveSig]
        new HRESULT BeginStateBlock();

        [PreserveSig]
        new HRESULT EndStateBlock(out IDirect3DStateBlock9 ppSB);

        [PreserveSig]
        new HRESULT SetClipStatus(ref _D3DCLIPSTATUS9 pClipStatus);

        [PreserveSig]
        new HRESULT GetClipStatus(ref _D3DCLIPSTATUS9 pClipStatus);

        [PreserveSig]
        new HRESULT GetTexture(uint Stage, out IDirect3DBaseTexture9 ppTexture);

        [PreserveSig]
        new HRESULT SetTexture(uint Stage, IDirect3DBaseTexture9 pTexture);

        [PreserveSig]
        new HRESULT GetTextureStageState(uint Stage, _D3DTEXTURESTAGESTATETYPE Type, ref uint pValue);

        [PreserveSig]
        new HRESULT SetTextureStageState(uint Stage, _D3DTEXTURESTAGESTATETYPE Type, uint Value);

        [PreserveSig]
        new HRESULT GetSamplerState(uint Sampler, _D3DSAMPLERSTATETYPE Type, ref uint pValue);

        [PreserveSig]
        new HRESULT SetSamplerState(uint Sampler, _D3DSAMPLERSTATETYPE Type, uint Value);

        [PreserveSig]
        new HRESULT ValidateDevice(ref uint pNumPasses);

        [PreserveSig]
        new HRESULT SetPaletteEntries(uint PaletteNumber, ref tagPALETTEENTRY pEntries);

        [PreserveSig]
        new HRESULT GetPaletteEntries(uint PaletteNumber, ref tagPALETTEENTRY pEntries);

        [PreserveSig]
        new HRESULT SetCurrentTexturePalette(uint PaletteNumber);

        [PreserveSig]
        new HRESULT GetCurrentTexturePalette(ref uint PaletteNumber);

        [PreserveSig]
        new HRESULT SetScissorRect(ref tagRECT pRect);

        [PreserveSig]
        new HRESULT GetScissorRect(ref tagRECT pRect);

        [PreserveSig]
        new HRESULT SetSoftwareVertexProcessing(bool bSoftware);

        [PreserveSig]
        new bool GetSoftwareVertexProcessing();

        [PreserveSig]
        new HRESULT SetNPatchMode(float nSegments);

        [PreserveSig]
        new void GetNPatchMode();

        [PreserveSig]
        new HRESULT DrawPrimitive(_D3DPRIMITIVETYPE PrimitiveType, uint StartVertex, uint PrimitiveCount);

        [PreserveSig]
        new HRESULT DrawIndexedPrimitive(_D3DPRIMITIVETYPE unnamed__0, int BaseVertexIndex, uint MinVertexIndex, uint NumVertices, uint startIndex, uint primCount);

        [PreserveSig]
        new HRESULT DrawPrimitiveUP(_D3DPRIMITIVETYPE PrimitiveType, uint PrimitiveCount, IntPtr pVertexStreamZeroData, uint VertexStreamZeroStride);

        [PreserveSig]
        new HRESULT DrawIndexedPrimitiveUP(_D3DPRIMITIVETYPE PrimitiveType, uint MinVertexIndex, uint NumVertices, uint PrimitiveCount, IntPtr pIndexData, _D3DFORMAT IndexDataFormat, IntPtr pVertexStreamZeroData, uint VertexStreamZeroStride);

        [PreserveSig]
        new HRESULT ProcessVertices(uint SrcStartIndex, uint DestIndex, uint VertexCount, IDirect3DVertexBuffer9 pDestBuffer, IDirect3DVertexDeclaration9 pVertexDecl, uint Flags);

        [PreserveSig]
        new HRESULT CreateVertexDeclaration(ref _D3DVERTEXELEMENT9 pVertexElements, out IDirect3DVertexDeclaration9 ppDecl);

        [PreserveSig]
        new HRESULT SetVertexDeclaration(IDirect3DVertexDeclaration9 pDecl);

        [PreserveSig]
        new HRESULT GetVertexDeclaration(out IDirect3DVertexDeclaration9 ppDecl);

        [PreserveSig]
        new HRESULT SetFVF(uint FVF);

        [PreserveSig]
        new HRESULT GetFVF(ref uint pFVF);

        [PreserveSig]
        new HRESULT CreateVertexShader(IntPtr pFunction, out IDirect3DVertexShader9 ppShader);

        [PreserveSig]
        new HRESULT SetVertexShader(IDirect3DVertexShader9 pShader);

        [PreserveSig]
        new HRESULT GetVertexShader(out IDirect3DVertexShader9 ppShader);

        [PreserveSig]
        new HRESULT SetVertexShaderConstantF(uint StartRegister, IntPtr pConstantData, uint Vector4fCount);

        [PreserveSig]
        new HRESULT GetVertexShaderConstantF(uint StartRegister, IntPtr pConstantData, uint Vector4fCount);

        [PreserveSig]
        new HRESULT SetVertexShaderConstantI(uint StartRegister, IntPtr pConstantData, uint Vector4iCount);

        [PreserveSig]
        new HRESULT GetVertexShaderConstantI(uint StartRegister, IntPtr pConstantData, uint Vector4iCount);

        [PreserveSig]
        new HRESULT SetVertexShaderConstantB(uint StartRegister, IntPtr pConstantData, uint BoolCount);

        [PreserveSig]
        new HRESULT GetVertexShaderConstantB(uint StartRegister, IntPtr pConstantData, uint BoolCount);

        [PreserveSig]
        new HRESULT SetStreamSource(uint StreamNumber, IDirect3DVertexBuffer9 pStreamData, uint OffsetInBytes, uint Stride);

        [PreserveSig]
        new HRESULT GetStreamSource(uint StreamNumber, out IDirect3DVertexBuffer9 ppStreamData, ref uint pOffsetInBytes, ref uint pStride);

        [PreserveSig]
        new HRESULT SetStreamSourceFreq(uint StreamNumber, uint Setting);

        [PreserveSig]
        new HRESULT GetStreamSourceFreq(uint StreamNumber, ref uint pSetting);

        [PreserveSig]
        new HRESULT SetIndices(IDirect3DIndexBuffer9 pIndexData);

        [PreserveSig]
        new HRESULT GetIndices(out IDirect3DIndexBuffer9 ppIndexData);

        [PreserveSig]
        new HRESULT CreatePixelShader(IntPtr pFunction, out IDirect3DPixelShader9 ppShader);

        [PreserveSig]
        new HRESULT SetPixelShader(IDirect3DPixelShader9 pShader);

        [PreserveSig]
        new HRESULT GetPixelShader(out IDirect3DPixelShader9 ppShader);

        [PreserveSig]
        new HRESULT SetPixelShaderConstantF(uint StartRegister, IntPtr pConstantData, uint Vector4fCount);

        [PreserveSig]
        new HRESULT GetPixelShaderConstantF(uint StartRegister, IntPtr pConstantData, uint Vector4fCount);

        [PreserveSig]
        new HRESULT SetPixelShaderConstantI(uint StartRegister, IntPtr pConstantData, uint Vector4iCount);

        [PreserveSig]
        new HRESULT GetPixelShaderConstantI(uint StartRegister, IntPtr pConstantData, uint Vector4iCount);

        [PreserveSig]
        new HRESULT SetPixelShaderConstantB(uint StartRegister, IntPtr pConstantData, uint BoolCount);

        [PreserveSig]
        new HRESULT GetPixelShaderConstantB(uint StartRegister, IntPtr pConstantData, uint BoolCount);

        [PreserveSig]
        new HRESULT DrawRectPatch(uint Handle, ref float pNumSegs, ref _D3DRECTPATCH_INFO pRectPatchInfo);

        [PreserveSig]
        new HRESULT DrawTriPatch(uint Handle, ref float pNumSegs, ref _D3DTRIPATCH_INFO pTriPatchInfo);

        [PreserveSig]
        new HRESULT DeletePatch(uint Handle);

        [PreserveSig]
        new HRESULT CreateQuery(_D3DQUERYTYPE Type, out IDirect3DQuery9 ppQuery);

        [PreserveSig]
        HRESULT SetConvolutionMonoKernel(uint width, uint height, ref float rows, ref float columns);

        [PreserveSig]
        HRESULT ComposeRects(IDirect3DSurface9 pSrc, IDirect3DSurface9 pDst, IDirect3DVertexBuffer9 pSrcRectDescs, uint NumRects, IDirect3DVertexBuffer9 pDstRectDescs, _D3DCOMPOSERECTSOP Operation, int Xoffset, int Yoffset);

        [PreserveSig]
        HRESULT PresentEx(ref tagRECT pSourceRect, ref tagRECT pDestRect, IntPtr hDestWindowOverride, ref _RGNDATA pDirtyRegion, uint dwFlags);

        [PreserveSig]
        HRESULT GetGPUThreadPriority(ref int pPriority);

        [PreserveSig]
        HRESULT SetGPUThreadPriority(int Priority);

        [PreserveSig]
        HRESULT WaitForVBlank(uint iSwapChain);

        [PreserveSig]
        HRESULT CheckResourceResidency(out IDirect3DResource9 pResourceArray, uint NumResources);

        [PreserveSig]
        HRESULT SetMaximumFrameLatency(uint MaxLatency);

        [PreserveSig]
        HRESULT GetMaximumFrameLatency(ref uint pMaxLatency);

        [PreserveSig]
        HRESULT CheckDeviceState(IntPtr hDestinationWindow);

        [PreserveSig]
        HRESULT CreateRenderTargetEx(uint Width, uint Height, _D3DFORMAT Format, _D3DMULTISAMPLE_TYPE MultiSample, uint MultisampleQuality, bool Lockable, out IDirect3DSurface9 ppSurface, IntPtr pSharedHandle, uint Usage);

        [PreserveSig]
        HRESULT CreateOffscreenPlainSurfaceEx(uint Width, uint Height, _D3DFORMAT Format, _D3DPOOL Pool, out IDirect3DSurface9 ppSurface, IntPtr pSharedHandle, uint Usage);

        [PreserveSig]
        HRESULT CreateDepthStencilSurfaceEx(uint Width, uint Height, _D3DFORMAT Format, _D3DMULTISAMPLE_TYPE MultiSample, uint MultisampleQuality, bool Discard, out IDirect3DSurface9 ppSurface, IntPtr pSharedHandle, uint Usage);

        [PreserveSig]
        HRESULT ResetEx(IntPtr pPresentationParameters, ref D3DDISPLAYMODEEX pFullscreenDisplayMode);

        [PreserveSig]
        HRESULT GetDisplayModeEx(uint iSwapChain, ref D3DDISPLAYMODEEX pMode, ref D3DDISPLAYROTATION pRotation);
    }

}