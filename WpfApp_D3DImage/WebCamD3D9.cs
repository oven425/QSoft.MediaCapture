using DirectN;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace QSoft.MediaCapture.WPF
{
    public partial class WebCamD3D9: WebCamD3D9.IMFSourceReaderCallback
    {
        protected D3DImage m_D3DImage;
        public WebCamD3D9(D3DImage d3dimage, WindowInteropHelper helper)
        {
            this.m_Helper = helper;
            this.InitD3D9Ex();
            m_D3DImage = d3dimage;
        }


        public void Init()
        {
            this.CreateAudioSource(out var audioSource);
        }

        public void Start()
        {
            var hr = CreateVideoDeviceSource(out var pSource);
            hr = this.CreateAudioSource(out var audioSource);
            var sourcex = pSource as DirectN.IMFSourceReaderEx;
            //if (SUCCEEDED(hr))

            IMFAttributes pAttributes = null;
            hr = MFCreateAttributes(out pAttributes, 6);
            //pAttributes.SetUINT32(DirectN.MFConstants.MF_READWRITE_ENABLE_HARDWARE_TRANSFORMS, 1);
            pAttributes.SetUINT32(DirectN.MFConstants.MF_SOURCE_READER_DISABLE_DXVA, 0);
            pAttributes.SetUINT32(DirectN.MFConstants.MF_SOURCE_READER_ENABLE_VIDEO_PROCESSING, 0);
            pAttributes.SetUINT32(DirectN.MFConstants.MF_SOURCE_READER_ENABLE_ADVANCED_VIDEO_PROCESSING, 1);
            pAttributes.SetUnknown(DirectN.MFConstants.MF_SOURCE_READER_ASYNC_CALLBACK, this);
            pAttributes.SetUnknown(DirectN.MFConstants.MF_SOURCE_READER_D3D_MANAGER, pDeviceManager);


            hr = MFCreateSourceReaderFromMediaSource(pSource, pAttributes, out m_pSourceReader);
            List<(IMFMediaType, uint width, uint height)> mediaTypes = new List<(IMFMediaType, uint width, uint height)>();
            uint index = 0;
            while (true)
            {
                hr = m_pSourceReader.GetNativeMediaType(0xFFFFFFFC, index, out var pMediaType);
                if (hr == 0)
                {
                    pMediaType.TryGetSize(DirectN.MFConstants.MF_MT_FRAME_SIZE, out var width, out var height);
                    hr = pMediaType.GetGUID(DirectN.MFConstants.MF_MT_SUBTYPE, out var subtype);
                    System.Diagnostics.Trace.WriteLine($"MediaType {index}: subtype={subtype}, width={width}, height={height}");
                    mediaTypes.Add((pMediaType, width, height));
                }
                else
                {
                    break;
                }
                index++;
            }
            var pMediaType1 = mediaTypes.OrderBy(x => x.width * x.height).Last();
            hr = DirectN.MFFunctions.MFCreateMediaType(out var tt);
            tt.SetGUID(DirectN.MFConstants.MF_MT_MAJOR_TYPE, DirectN.MFConstants.MFMediaType_Video);
            tt.SetGUID(DirectN.MFConstants.MF_MT_SUBTYPE, DirectN.MFConstants.MFVideoFormat_RGB32);
            tt.SetSize(DirectN.MFConstants.MF_MT_FRAME_SIZE, pMediaType1.width, pMediaType1.height);
            hr = m_pSourceReader.SetCurrentMediaType(0xFFFFFFFC, IntPtr.Zero, tt);
            //IMFSourceReaderEx sourceReaderEx = m_pSourceReader as IMFSourceReaderEx;
            //var videoprocesstype = Type.GetTypeFromCLSID(DirectN.MFConstants.CLSID_VideoProcessorMFT);
            //var videoprocessmft = Activator.CreateInstance(videoprocesstype) as IMFVideoProcessorControl;
            //hr = videoprocessmft.SetMirror(_MF_VIDEO_PROCESSOR_MIRROR.MIRROR_HORIZONTAL);
            //if (videoprocessmft is IMFTransform mft)
            //{
            //    hr = mft.SetInputType(0, tt, 0);
            //    hr = mft.SetOutputType(0, tt, 0);
            //}
            //hr = sourceReaderEx.AddTransformForStream(0xFFFFFFFC, videoprocessmft);
            hr = m_pSourceReader.ReadSample(0xFFFFFFFC, 0, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);

        }

        IDirect3D9 pD3D = null;
        IDirect3D9Ex pD3D9Ex = null;
        IDirect3DDevice9 pDevice = null;
        protected IDirect3DDevice9Ex pDeviceEx = null;
        protected IDirect3DDeviceManager9 pDeviceManager = null;

        protected IDirect3DTexture9 pRenderTexture = null;
        protected IDirect3DSurface9 pRenderSurface = null;
        int m_Width = 1280;
        int m_Height = 720;
        public tagRECT m_Rect = new tagRECT(0, 0, 1280, 720);
        protected void CreateWPFCompatibleSurface(IDirect3DSurface9 surface)
        {
            _D3DSURFACE_DESC desc = new _D3DSURFACE_DESC();
            surface.GetDesc(ref desc);
            m_Rect = new tagRECT(0, 0, (int)desc.Width, (int)desc.Height);
            // 關鍵參數設定
            HRESULT hr = pDeviceEx.CreateTexture(
                desc.Width,
                desc.Height,
                1,
                DirectN.Constants.D3DUSAGE_RENDERTARGET,
                desc.Format,
                (uint)DirectN._D3DPOOL.D3DPOOL_DEFAULT,
                out pRenderTexture,
                out IntPtr aa
            );

            pRenderTexture.GetSurfaceLevel(0, out pRenderSurface);
        }



        WindowInteropHelper m_Helper;
        void InitD3D9Ex()
        {
            HRESULT hr = Direct3DCreate9Ex(DirectN.Constants.D3D_SDK_VERSION, out pD3D9Ex);
            DirectN._D3DPRESENT_PARAMETERS_64 d3dpp = new DirectN._D3DPRESENT_PARAMETERS_64
            {
                Windowed = true,
                SwapEffect = _D3DSWAPEFFECT.D3DSWAPEFFECT_DISCARD,
                hDeviceWindow = m_Helper.Handle
            };
            IntPtr unmanagedAddr = Marshal.AllocHGlobal(Marshal.SizeOf(d3dpp));
            Marshal.StructureToPtr<DirectN._D3DPRESENT_PARAMETERS_64>(d3dpp, unmanagedAddr, false);


            hr = pD3D9Ex.CreateDeviceEx(
                DirectN.Constants.D3DADAPTER_DEFAULT,
                _D3DDEVTYPE.D3DDEVTYPE_HAL,
                m_Helper.Handle,
                DirectN.Constants.D3DCREATE_HARDWARE_VERTEXPROCESSING | DirectN.Constants.D3DCREATE_MULTITHREADED | DirectN.Constants.D3DCREATE_FPU_PRESERVE,
                unmanagedAddr,
                IntPtr.Zero,
                out pDeviceEx
            );
            uint resetToken = 0;
            hr = DXVA2CreateDirect3DDeviceManager9(out resetToken, out pDeviceManager);
            hr = pDeviceManager.ResetDevice(pDeviceEx, resetToken);
        }

        HRESULT CreateAudioSource(out IMFMediaSource ppSource)
        {
            ppSource = null;
            IMFMediaSource pSource = null;
            IMFAttributes pAttributes = null;
            //IMFActivate ppDevices = null;
            IntPtr ppDevices = IntPtr.Zero;
            // Create an attribute store to specify the enumeration parameters.
            HRESULT hr = MFCreateAttributes(out pAttributes, 1);

            // Source type: video capture devices
            hr = pAttributes.SetGUID(
                DirectN.MFConstants.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE,
                DirectN.MFConstants.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_AUDCAP_GUID
            );


            // Enumerate devices.
            hr = MFEnumDeviceSources(pAttributes, out ppDevices, out var count);

            if (count > 0)
            {
                IntPtr[] ptrs = new IntPtr[count];
                Marshal.Copy(ppDevices, ptrs, 0, (int)count);

                for (int i = 0; i < count; i++)
                {
                    // 將 IntPtr 轉換為受控介面
                    IMFActivate activate = (IMFActivate)Marshal.GetObjectForIUnknown(ptrs[i]);


                    activate.ActivateObject(typeof(IMFMediaSource).GUID, out var obj);
                    ppSource = obj as IMFMediaSource;
                    //ppSource = activate.ActivateObject<IMFMediaSource>().Object;
                    break;
                    // 這裡可以讀取設備名稱或啟動設備...

                    // 釋放單個介面
                    //Marshal.ReleaseComObject(activate);
                }
            }


            return hr;
        }

        protected HRESULT CreateVideoDeviceSource(out IMFMediaSource ppSource)
        {
            ppSource = null;
            IMFMediaSource pSource = null;
            IMFAttributes pAttributes = null;
            //IMFActivate ppDevices = null;
            IntPtr ppDevices = IntPtr.Zero;
            // Create an attribute store to specify the enumeration parameters.
            HRESULT hr = MFCreateAttributes(out pAttributes, 1);

            // Source type: video capture devices
            hr = pAttributes.SetGUID(
                DirectN.MFConstants.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE,
                DirectN.MFConstants.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_GUID
            );


            // Enumerate devices.
            hr = MFEnumDeviceSources(pAttributes, out ppDevices, out var count);

            if (count > 0)
            {
                IntPtr[] ptrs = new IntPtr[count];
                Marshal.Copy(ppDevices, ptrs, 0, (int)count);

                for (int i = 0; i < count; i++)
                {
                    // 將 IntPtr 轉換為受控介面
                    IMFActivate activate = (IMFActivate)Marshal.GetObjectForIUnknown(ptrs[i]);

                    
                    activate.ActivateObject(typeof(IMFMediaSource).GUID, out var obj);
                    ppSource = obj as IMFMediaSource;
                    //ppSource = activate.ActivateObject<IMFMediaSource>().Object;
                    break;
                    // 這裡可以讀取設備名稱或啟動設備...

                    // 釋放單個介面
                    //Marshal.ReleaseComObject(activate);
                }
            }


            return hr;
        }

        public void Snapshot(string filename)
        {
            if (m_pSourceReader != null)
            {
                m_pSourceReader.ReadSample(0xFFFFFFFC, 0, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
            }
        }





        IMFSourceReader m_pSourceReader;
        object m_Lock = new object();
        public HRESULT OnReadSample(HRESULT hrStatus, uint dwStreamIndex, uint dwStreamFlags, long llTimestamp, IntPtr pSampleptr)
        {
            if (System.Threading.Monitor.TryEnter(m_Lock))
            {
                //hr = surface.LockRect(out var ptr1, this.m_Rect, 0);
                //var bmpSource = BitmapSource.Create((int)m_Rect.right, (int)m_Rect.bottom, 96, 96, System.Windows.Media.PixelFormats.Bgra32, null, ptr1.pBits, (int)(ptr1.Pitch * m_Rect.bottom), (int)ptr1.Pitch);
                //var encoder = new JpegBitmapEncoder { QualityLevel = 85 };
                //encoder.Frames.Add(BitmapFrame.Create(bmpSource));
                //using (var stream = new FileStream("123.jpg", FileMode.Create))
                //{
                //    encoder.Save(stream);
                //}
                //hr = surface.UnlockRect();
                try
                {
                    this.m_D3DImage.Dispatcher.Invoke(() =>
                    {
                        if (pSampleptr != IntPtr.Zero)
                        {
                            Marshal.AddRef(pSampleptr);
                            IDirect3DSurface9 surface = null;
                            var pSample = Marshal.GetObjectForIUnknown(pSampleptr) as IMFSample;
                            var hr = pSample.GetBufferByIndex(0, out var pBuffer);
                            hr = DirectN.MFFunctions.MFGetService(pBuffer, DirectN.MFConstants.MR_BUFFER_SERVICE, new Guid("0cfbaf3a-9ff6-429a-99b3-a2796af8b89b"), out var dd);
                            surface = (IDirect3DSurface9)dd;
                            if (this.pRenderTexture == null)
                            {
                                CreateWPFCompatibleSurface(surface);
                                var ptr = Marshal.GetIUnknownForObject(pRenderSurface);
                                this.m_D3DImage.Lock();
                                this.m_D3DImage.SetBackBuffer(System.Windows.Interop.D3DResourceType.IDirect3DSurface9, ptr);
                                this.m_D3DImage.Unlock();
                            }
                            
                            

                            pDeviceEx.StretchRect(surface, m_Rect, pRenderSurface, m_Rect, (uint)DirectN._D3DTEXTUREFILTERTYPE.D3DTEXF_NONE);
                            this.m_D3DImage.Lock();
                            this.m_D3DImage.AddDirtyRect(new Int32Rect(0, 0, m_Rect.right, m_Rect.bottom));
                            this.m_D3DImage.Unlock();
                            Marshal.ReleaseComObject(surface);
                            Marshal.ReleaseComObject(pBuffer);
                            Marshal.ReleaseComObject(pSample);
                            Marshal.Release(pSampleptr);

                        }
                        var hr1 = m_pSourceReader.ReadSample(0xFFFFFFFC, 0, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
                    });
                }
                finally
                {
                    System.Threading.Monitor.Exit(m_Lock);
                }
            }

            return HRESULTS.S_OK;
        }

        public HRESULT OnFlush(uint dwStreamIndex)
        {
            return HRESULTS.S_OK;
        }

        public HRESULT OnEvent(uint dwStreamIndex, IMFMediaEvent pEvent)
        {
            return HRESULTS.S_OK;
        }

    }
}
