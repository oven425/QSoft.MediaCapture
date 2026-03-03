using DirectN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace WpfApp_D3DImage
{
    public partial class WebCamD3D9
    {
        protected D3DImage m_D3DImage;
        public WebCamD3D9(D3DImage d3dimage, WindowInteropHelper helper)
        {
            this.m_Helper = helper;
            this.InitD3D9Ex();
            m_D3DImage = d3dimage;
        }

        virtual public async Task Start()
        {
            var hr = CreateVideoDeviceSource(out var pSource);
            //if (SUCCEEDED(hr))
            {
                IMFSourceReader m_pSourceReader;
                IMFAttributes pAttributes = null;
                hr = DirectN.MFFunctions.MFCreateAttributes(out pAttributes, 5);
                pAttributes.SetUINT32(DirectN.MFConstants.MF_READWRITE_ENABLE_HARDWARE_TRANSFORMS, 1);
                pAttributes.SetUINT32(DirectN.MFConstants.MF_SOURCE_READER_DISABLE_DXVA, 0);
                pAttributes.SetUINT32(DirectN.MFConstants.MF_SOURCE_READER_ENABLE_VIDEO_PROCESSING, 0);
                pAttributes.SetUINT32(DirectN.MFConstants.MF_SOURCE_READER_ENABLE_ADVANCED_VIDEO_PROCESSING, 1);
                pAttributes.SetUnknown(DirectN.MFConstants.MF_SOURCE_READER_D3D_MANAGER, pDeviceManager);


                hr = DirectN.Functions.MFCreateSourceReaderFromMediaSource(pSource, pAttributes, out m_pSourceReader);

                hr = m_pSourceReader.GetNativeMediaType(0xFFFFFFFC, 0, out var pMediaType);
                //GUID subtype;
                hr = pMediaType.GetGUID(DirectN.MFConstants.MF_MT_SUBTYPE, out var subtype);
                hr = pMediaType.SetGUID(DirectN.MFConstants.MF_MT_SUBTYPE, DirectN.MFConstants.MFVideoFormat_ARGB32);
                hr = DirectN.MFFunctions.MFCreateMediaType(out var tt);
                tt.SetGUID(DirectN.MFConstants.MF_MT_MAJOR_TYPE, DirectN.MFConstants.MFMediaType_Video);
                tt.SetGUID(DirectN.MFConstants.MF_MT_SUBTYPE, DirectN.MFConstants.MFVideoFormat_RGB32);
                hr = m_pSourceReader.SetCurrentMediaType(0xFFFFFFFC, IntPtr.Zero, tt);

                while (true)
                {

                    using (var index = new ComMemory(Marshal.SizeOf<uint>()))
                    using (var timestamp = new ComMemory(Marshal.SizeOf<long>()))
                    using (var streamflag = new ComMemory(Marshal.SizeOf<uint>()))
                    {
                        hr = m_pSourceReader.ReadSample(0, 0, index.Pointer, streamflag.Pointer, timestamp.Pointer, out var sample);
                        if (sample == null)
                        {
                            continue;
                        }

                        hr = sample.GetBufferByIndex(0, out var pBuffer);
                        //if (SUCCEEDED(hr))
                        {
                            hr = DirectN.MFFunctions.MFGetService(pBuffer, DirectN.MFConstants.MR_BUFFER_SERVICE, new Guid("0cfbaf3a-9ff6-429a-99b3-a2796af8b89b"), out var dd);
                            IDirect3DSurface9 surface = (IDirect3DSurface9)dd;
                            if (this.pRenderTexture == null)
                            {

                                CreateWPFCompatibleSurface(surface);

                                var ptr = Marshal.GetIUnknownForObject(pRenderSurface);
                                m_D3DImage.Dispatcher.Invoke(() =>
                                {
                                    if (m_D3DImage.IsFrontBufferAvailable)
                                    {
                                        this.m_D3DImage.Lock();
                                        this.m_D3DImage.SetBackBuffer(System.Windows.Interop.D3DResourceType.IDirect3DSurface9, ptr);
                                        this.m_D3DImage.Unlock();
                                    }
                                });

                            }
                            pDeviceEx.StretchRect(
    surface,
    new tagRECT(0, 0, 1280, 720), pRenderSurface, new tagRECT(0, 0, 1280, 720), (int)DirectN._D3DTEXTUREFILTERTYPE.D3DTEXF_NONE);
                            this.m_D3DImage.Dispatcher.Invoke(() =>
                            {
                                if (m_D3DImage.IsFrontBufferAvailable)
                                {
                                    this.m_D3DImage.Lock();
                                    this.m_D3DImage.AddDirtyRect(new Int32Rect(0, 0, 1280, 720));
                                    this.m_D3DImage.Unlock();
                                }
                            });

                            Marshal.ReleaseComObject(surface);
                            Marshal.ReleaseComObject(pBuffer);
                            Marshal.ReleaseComObject(sample);

                        }

                    }




                    await Task.Delay(30);
                }


                // Use the media source for capture or preview.
                //pSource->Release();
            }
        }

        IDirect3D9 pD3D = null;
        IDirect3D9Ex pD3D9Ex = null;
        IDirect3DDevice9 pDevice = null;
        protected IDirect3DDevice9Ex pDeviceEx = null;
        protected IDirect3DDeviceManager9 pDeviceManager = null;

        protected IDirect3DTexture9 pRenderTexture = null;
        protected IDirect3DSurface9 pRenderSurface = null;

        protected void CreateWPFCompatibleSurface(IDirect3DSurface9 surface)
        {
            _D3DSURFACE_DESC desc = new _D3DSURFACE_DESC();
            surface.GetDesc(ref desc);
            // 關鍵參數設定
            HRESULT hr = pDeviceEx.CreateTexture(
                desc.Width,
                desc.Height,
                1,
                DirectN.Constants.D3DUSAGE_RENDERTARGET,
                (uint)desc.Format,
                (uint)DirectN._D3DPOOL.D3DPOOL_DEFAULT,
                out pRenderTexture,
                IntPtr.Zero
            );

            pRenderTexture.GetSurfaceLevel(0, out pRenderSurface);
        }

        protected void CreateWPFCompatibleSurface(DirectN._D3DSURFACE_DESC desc)
        {
            // 關鍵參數設定
            HRESULT hr = pDeviceEx.CreateTexture(
                desc.Width,
                desc.Height,
                1,
                DirectN.Constants.D3DUSAGE_RENDERTARGET,
                (uint)desc.Format,
                D3DPOOL_DEFAULT,
                out pRenderTexture,
                IntPtr.Zero
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
                (int)DirectN._D3DDEVTYPE.D3DDEVTYPE_HAL,
                m_Helper.Handle,
                DirectN.Constants.D3DCREATE_HARDWARE_VERTEXPROCESSING | DirectN.Constants.D3DCREATE_MULTITHREADED | DirectN.Constants.D3DCREATE_FPU_PRESERVE,
                unmanagedAddr,
                IntPtr.Zero,
                out pDeviceEx
            );
            uint resetToken = 0;
            hr = DXVA2CreateDirect3DDeviceManager9(out resetToken, out pDeviceManager);
            //if (FAILED(hr))
            //{
            //    pDevice->Release();
            //    return;
            //}
            hr = pDeviceManager.ResetDevice(pDeviceEx, resetToken);
            //if (FAILED(hr))
            //{
            //    pDeviceManager->Release();
            //    pDevice->Release();
            //    return;
            //}
        }


        //void InitD3D9()
        //{
        //    pD3D = Direct3DCreate9(DirectN.Constants.D3D_SDK_VERSION);
        //    DirectN._D3DPRESENT_PARAMETERS_64 d3dpp = new DirectN._D3DPRESENT_PARAMETERS_64
        //    {
        //        Windowed = true,
        //        SwapEffect = _D3DSWAPEFFECT.D3DSWAPEFFECT_DISCARD,
        //        hDeviceWindow = DirectN.WindowsFunctions.GetDesktopWindow()
        //    };
        //    IntPtr unmanagedAddr = Marshal.AllocHGlobal(Marshal.SizeOf(d3dpp));
        //    Marshal.StructureToPtr<DirectN._D3DPRESENT_PARAMETERS_64>(d3dpp, unmanagedAddr, false);
        //    HRESULT hr = pD3D.CreateDevice(
        //        DirectN.Constants.D3DADAPTER_DEFAULT,
        //        DirectN._D3DDEVTYPE.D3DDEVTYPE_HAL,
        //        DirectN.WindowsFunctions.GetDesktopWindow(),
        //        DirectN.Constants.D3DCREATE_HARDWARE_VERTEXPROCESSING | DirectN.Constants.D3DCREATE_FPU_PRESERVE | DirectN.Constants.D3DCREATE_MULTITHREADED,
        //        unmanagedAddr,
        //        out pDevice
        //    );
        //    uint resetToken = 0;
        //    hr = DXVA2CreateDirect3DDeviceManager9(out resetToken, out pDeviceManager);
        //    //if (FAILED(hr))
        //    //{
        //    //    pDevice->Release();
        //    //    return;
        //    //}

        //    hr = pDeviceManager.ResetDevice(pDevice, resetToken);
        //    //if (FAILED(hr))
        //    //{
        //    //    pDeviceManager->Release();
        //    //    pDevice->Release();
        //    //    return;
        //    //}
        //}

        protected HRESULT CreateVideoDeviceSource(out IMFMediaSource ppSource)
        {
            ppSource = null;
            IMFMediaSource pSource = null;
            IMFAttributes pAttributes = null;
            //IMFActivate ppDevices = null;
            IntPtr ppDevices = IntPtr.Zero;
            // Create an attribute store to specify the enumeration parameters.
            HRESULT hr = DirectN.MFFunctions.MFCreateAttributes(out pAttributes, 1);

            // Source type: video capture devices
            hr = pAttributes.SetGUID(
                DirectN.MFConstants.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE,
                DirectN.MFConstants.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_GUID
            );


            // Enumerate devices.
            hr = DirectN.Functions.MFEnumDeviceSources(pAttributes, out ppDevices, out var count);

            if (count > 0)
            {
                IntPtr[] ptrs = new IntPtr[count];
                Marshal.Copy(ppDevices, ptrs, 0, (int)count);

                for (int i = 0; i < count; i++)
                {
                    // 將 IntPtr 轉換為受控介面
                    IMFActivate activate = (IMFActivate)Marshal.GetObjectForIUnknown(ptrs[i]);
                    ppSource = activate.ActivateObject<IMFMediaSource>().Object;
                    break;
                    // 這裡可以讀取設備名稱或啟動設備...

                    // 釋放單個介面
                    //Marshal.ReleaseComObject(activate);
                }
            }


            return hr;
        }
        //[ComImport]
        //[Guid("a0cade0f-06d5-4cf4-a1c7-f3cdd725aa75")]
        //[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        //public interface IDirect3DDeviceManager9
        //{
        //    int ResetDevice(IDirect3DDevice9 pDevice, uint resetToken);
        //    int OpenDeviceHandle(out IntPtr phDevice);
        //    int CloseDeviceHandle(IntPtr hDevice);
        //    int TestDevice(IntPtr hDevice);
        //    int LockDevice(IntPtr hDevice, out IntPtr ppDevice, bool fBlock);
        //    int UnlockDevice(IntPtr hDevice, bool fSaveState);
        //    int GetVideoService(IntPtr hDevice, ref Guid riid, out IntPtr ppService);
        //}
        //[DllImport("dxva2", ExactSpelling = true)]
        //public static extern int DXVA2CreateDirect3DDeviceManager9(out uint pResetToken, out IDirect3DDeviceManager9 ppDeviceManager);

    }
}
